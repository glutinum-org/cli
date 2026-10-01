module Glutinum.Converter.Reader.TypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

type private IntersectionTypePropertyResult =
    | Single of Ts.Symbol * Ts.Declaration
    | WithoutDeclaration of Ts.Symbol
    | ForceAny

// Properties created by a mapped type (e.g. `{ [K in Keys]: string }`) have no declaration
let private readPropertyWithoutDeclaration
    (reader: ITypeScriptReader)
    (contextNode: Ts.Node)
    (property: Ts.Symbol)
    : GlueMember option
    =
    let typ = reader.checker.getTypeOfSymbol property

    let flags = typeNodeBuilderFlags

    match reader.checker.typeToTypeNode (typ, None, Some flags) with
    | Some typeNode ->
        let previousContext = reader.SyntheticContext
        reader.SyntheticContext <- Some contextNode

        let propertyType =
            try
                reader.ReadTypeNode typeNode
            finally
                reader.SyntheticContext <- previousContext

        ({
            Name = property.name
            Documentation = []
            Type = propertyType
            IsOptional =
                match property.flags with
                | HasSymbolFlags Ts.SymbolFlags.Optional -> true
                | _ -> false
            IsStatic = false
            Accessor = GlueAccessor.ReadWrite
            IsPrivate = false
        }
        : GlueProperty)
        |> GlueMember.Property
        |> Some
    | None ->
        Report.readerError (
            "type node",
            $"Could not resolve the type of the property '%s{property.name}'",
            contextNode
        )
        |> reader.Warnings.Add

        None

// The declaration of a member of an instantiated generic type (e.g. `Foo<string>`)
// only refers to the type parameters, the checker knows the instantiated type
let private readInstantiatedMember
    (reader: ITypeScriptReader)
    (contextNode: Ts.Node)
    (property: Ts.Symbol)
    (declaration: Ts.Declaration)
    : GlueMember
    =
    let checker = reader.checker
    let instantiatedType = checker.getTypeOfSymbol property

    let isInstantiated =
        not (isNull declaration?symbol)
        && not (obj.ReferenceEquals(instantiatedType, checker.getTypeOfSymbol declaration?symbol))

    let declaredMember = reader.ReadDeclaration declaration

    if not isInstantiated then
        declaredMember
    else
        let flags = typeNodeBuilderFlags

        // A node synthesized by `typeToTypeNode` can't be given back to the checker
        let enclosingDeclaration =
            if contextNode.pos < 0 then
                None
            else
                Some contextNode

        match checker.typeToTypeNode (instantiatedType, enclosingDeclaration, Some flags) with
        | None -> declaredMember
        | Some typeNode ->
            let previousContext = reader.SyntheticContext
            reader.SyntheticContext <- Some contextNode

            let instantiatedMember =
                try
                    reader.ReadTypeNode typeNode
                finally
                    reader.SyntheticContext <- previousContext

            match declaredMember, instantiatedMember with
            | GlueMember.Property info, typ -> GlueMember.Property { info with Type = typ }
            | GlueMember.MethodSignature info, GlueType.FunctionType functionType ->
                GlueMember.MethodSignature
                    { info with
                        Parameters = functionType.Parameters
                        Type = functionType.Type
                    }
            | GlueMember.Method info, GlueType.FunctionType functionType ->
                GlueMember.Method
                    { info with
                        Parameters = functionType.Parameters
                        Type = functionType.Type
                    }
            | declaredMember, _ -> declaredMember

let private isLibraryName (reader: ITypeScriptReader) (name: string) =
    match reader.PackageContext with
    | Some packageContext -> packageContext.IsLibraryName name
    | None -> false

/// A conditional type the checker can't resolve without its type arguments
let private isDeferredConditional (typ: Ts.Type) =
    match typ.flags with
    | HasTypeFlags Ts.TypeFlags.Conditional -> true
    | _ -> false

/// The type parameters of the enclosing declarations bound to their default, else to their
/// constraint, the innermost declaration first. A `keyof` constraint binds nothing, and the
/// parameters of a type alias are bound by its references, not by its declaration.
/// `typeToTypeNode` builds the other nodes, a copy of a declaration among them: the checker
/// can't be asked about those
let private isParseTreeNode (node: Ts.Node) =
    int node.flags &&& int Ts.NodeFlags.Synthesized = 0

/// The type parameters in scope of the node, innermost first. An alias's own are the arguments
/// of its reference and end the walk
let private typeParametersInScope (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) =
    let rec collect (node: Ts.Node) (acc: Ts.TypeParameterDeclaration list) =
        if isNull node then
            List.rev acc
        // A synthesized node has no parent, the node it was read for has
        elif
            isNull node.parent
            && node.pos < 0
            && reader.SyntheticContext.IsSome
            && not (obj.ReferenceEquals(node, reader.SyntheticContext.Value))
        then
            collect reader.SyntheticContext.Value acc
        elif node.kind = Ts.SyntaxKind.TypeAliasDeclaration then
            List.rev acc
        else
            let own: Ts.NodeArray<Ts.TypeParameterDeclaration> option =
                if isParseTreeNode node then
                    node?typeParameters
                else
                    None

            let own = own |> Option.map Seq.toList |> Option.defaultValue []

            collect node.parent (List.rev own @ acc)

    collect typeNode.parent []

/// A synthesized signature has type parameters without a parent
let private isDeclaredByType (typeParameter: Ts.TypeParameterDeclaration) =
    let parent: Ts.Node = !!typeParameter.parent

    not (isNull parent)
    && (
        match parent.kind with
        | Ts.SyntaxKind.ClassDeclaration
        | Ts.SyntaxKind.ClassExpression
        | Ts.SyntaxKind.InterfaceDeclaration -> true
        | _ -> false
    )

/// A type parameter of the declaring type stands for its default, else its constraint. One of a
/// signature is the caller's to choose, `mount<Story = never>` keeps `Story`: only its
/// constraint is known. `T = any` and `K extends keyof T` bind nothing
let private boundTypeParameters (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) =
    (Map.empty, typeParametersInScope reader typeNode)
    ||> List.fold (fun bound typeParameter ->
        let name = identifierText typeParameter.name

        let binding =
            match typeParameter.``default``, typeParameter.``constraint`` with
            | Some default_, _ when
                isDeclaredByType typeParameter && default_.kind <> Ts.SyntaxKind.AnyKeyword
                ->
                Some default_
            | Some _, _ -> None
            | None, Some constraint_ when constraint_.kind <> Ts.SyntaxKind.TypeOperator ->
                Some constraint_
            | _ -> None

        match binding with
        | Some binding when not (bound.ContainsKey name) -> bound.Add(name, binding)
        | _ -> bound
    )

/// `T` of `JSHandle<T = any>`
let private isAnyTypeParameter (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) =
    match typeNode.kind with
    | Ts.SyntaxKind.TypeReference ->
        let typeName: Ts.Node = !!(typeNode :?> Ts.TypeReferenceNode).typeName

        typeName.kind = Ts.SyntaxKind.Identifier
        && (typeParametersInScope reader typeNode
            |> List.tryFind (fun typeParameter ->
                identifierText typeParameter.name = identifierText typeName
            )
            |> Option.bind _.``default``
            |> Option.exists (fun default_ -> default_.kind = Ts.SyntaxKind.AnyKeyword))
    | _ -> false

let rec private mentionsBoundTypeParameter
    (bindings: Map<string, Ts.TypeNode>)
    (typeNode: Ts.TypeNode)
    =
    let mentions = mentionsBoundTypeParameter bindings

    match typeNode.kind with
    | Ts.SyntaxKind.TypeReference ->
        let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode
        let typeName: Ts.Node = !!typeReferenceNode.typeName

        (typeName.kind = Ts.SyntaxKind.Identifier
         && bindings.ContainsKey(identifierText typeName))
        || (typeReferenceNode.typeArguments
            |> Option.map (Seq.exists mentions)
            |> Option.defaultValue false)
    | Ts.SyntaxKind.ConditionalType ->
        let conditionalTypeNode = typeNode :?> Ts.ConditionalTypeNode

        mentions conditionalTypeNode.checkType
        || mentions conditionalTypeNode.extendsType
        || mentions conditionalTypeNode.trueType
        || mentions conditionalTypeNode.falseType
    | Ts.SyntaxKind.IndexedAccessType ->
        let indexedAccessTypeNode = typeNode :?> Ts.IndexedAccessTypeNode

        mentions indexedAccessTypeNode.objectType
        || mentions indexedAccessTypeNode.indexType
    | Ts.SyntaxKind.ArrayType -> mentions (typeNode :?> Ts.ArrayTypeNode).elementType
    | Ts.SyntaxKind.UnionType -> (typeNode :?> Ts.UnionTypeNode).types |> Seq.exists mentions
    | _ -> false

/// The type node with the bound type parameters replaced by their binding. The nodes rebuilt
/// are made parse tree nodes of the same place: the checker resolves a synthesized node to the
/// node it was made from, and the names keep their own scope.
let rec private substituteBoundTypeParameters
    (bindings: Map<string, Ts.TypeNode>)
    (typeNode: Ts.TypeNode)
    : Ts.TypeNode
    =
    let substitute = substituteBoundTypeParameters bindings

    // The factory parenthesizes a union it is given, `(A | B)[K]`: the wrapper is adopted too
    let adopt (synthesized: Ts.TypeNode) : Ts.TypeNode =
        emitJsExpr
            (ts, synthesized, typeNode, int Ts.NodeFlags.Synthesized)
            """(function (ts, node, like, flag) {
                Object.assign(node, { flags: node.flags & ~flag, pos: like.pos, end: like.end, parent: like.parent });
                ts.forEachChild(node, function (child) {
                    if ((child.flags & flag) !== 0 && !child.parent) {
                        Object.assign(child, { flags: child.flags & ~flag, pos: like.pos, end: like.end, parent: node });
                    }
                });
                return node;
            })($0, $1, $2, $3)"""

    match typeNode.kind with
    | Ts.SyntaxKind.TypeReference ->
        let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode
        let typeName: Ts.Node = !!typeReferenceNode.typeName

        match typeReferenceNode.typeArguments with
        | None when typeName.kind = Ts.SyntaxKind.Identifier ->
            bindings.TryFind(identifierText typeName) |> Option.defaultValue typeNode
        | None -> typeNode
        | Some typeArguments ->
            ts.factory.createTypeReferenceNode (
                unbox<Ts.Identifier> typeReferenceNode.typeName,
                ResizeArray(typeArguments |> Seq.map substitute)
            )
            :> Ts.TypeNode
            |> adopt
    | Ts.SyntaxKind.ConditionalType ->
        let conditionalTypeNode = typeNode :?> Ts.ConditionalTypeNode

        ts.factory.createConditionalTypeNode (
            substitute conditionalTypeNode.checkType,
            substitute conditionalTypeNode.extendsType,
            substitute conditionalTypeNode.trueType,
            substitute conditionalTypeNode.falseType
        )
        :> Ts.TypeNode
        |> adopt
    | Ts.SyntaxKind.IndexedAccessType ->
        let indexedAccessTypeNode = typeNode :?> Ts.IndexedAccessTypeNode

        ts.factory.createIndexedAccessTypeNode (
            substitute indexedAccessTypeNode.objectType,
            substitute indexedAccessTypeNode.indexType
        )
        :> Ts.TypeNode
        |> adopt
    | Ts.SyntaxKind.ArrayType ->
        ts.factory.createArrayTypeNode (substitute (typeNode :?> Ts.ArrayTypeNode).elementType)
        :> Ts.TypeNode
        |> adopt
    | Ts.SyntaxKind.UnionType ->
        ts.factory.createUnionTypeNode (
            ResizeArray((typeNode :?> Ts.UnionTypeNode).types |> Seq.map substitute)
        )
        :> Ts.TypeNode
        |> adopt
    | _ -> typeNode

/// The conditionals the checker has not resolved in the type: the type itself, or one it
/// takes as an argument
let rec private deferredConditionals (checker: Ts.TypeChecker) (depth: int) (typ: Ts.Type) =
    match typ.flags with
    | HasTypeFlags Ts.TypeFlags.Conditional -> [ typ :?> Ts.ConditionalType ]
    | HasTypeFlags Ts.TypeFlags.Union
    | HasTypeFlags Ts.TypeFlags.Intersection ->
        (typ :?> Ts.UnionOrIntersectionType).types
        |> Seq.toList
        |> List.collect (deferredConditionals checker depth)
    | HasTypeFlags Ts.TypeFlags.Object when
        depth < 8
        && int (typ :?> Ts.ObjectType).objectFlags &&& int Ts.ObjectFlags.Reference <> 0
        ->
        checker.getTypeArguments (typ :?> Ts.TypeReference)
        |> Seq.toList
        |> List.collect (deferredConditionals checker (depth + 1))
    | _ -> []

/// The types a type is made of, itself included: `[TPageParam]` is made of `TPageParam`
let rec private mentionedTypes
    (checker: Ts.TypeChecker)
    (depth: int)
    (typ: Ts.Type)
    : Ts.Type list
    =
    let mentioned = mentionedTypes checker (depth + 1)

    if depth > 8 then
        []
    else
        // `_DeepPartialObject<T>` is a mapped type, `T` is reached as its alias argument
        let aliasArguments =
            typ.aliasTypeArguments
            |> Option.map (Seq.toList >> List.collect mentioned)
            |> Option.defaultValue []

        typ :: aliasArguments
        @ match typ.flags with
          | HasTypeFlags Ts.TypeFlags.Union
          | HasTypeFlags Ts.TypeFlags.Intersection ->
              (typ :?> Ts.UnionOrIntersectionType).types
              |> Seq.toList
              |> List.collect mentioned
          | HasTypeFlags Ts.TypeFlags.Object when
              int (typ :?> Ts.ObjectType).objectFlags &&& int Ts.ObjectFlags.Reference <> 0
              ->
              checker.getTypeArguments (typ :?> Ts.TypeReference)
              |> Seq.toList
              |> List.collect mentioned
          | HasTypeFlags Ts.TypeFlags.Index -> mentioned !!(typ :?> Ts.IndexType).``type``
          | HasTypeFlags Ts.TypeFlags.IndexedAccess ->
              let indexedAccess = typ :?> Ts.IndexedAccessType
              mentioned indexedAccess.objectType @ mentioned indexedAccess.indexType
          | HasTypeFlags Ts.TypeFlags.Conditional ->
              let conditional = typ :?> Ts.ConditionalType
              mentioned conditional.checkType @ mentioned conditional.extendsType
          | _ -> []

let private typeParameterNamesOf (checker: Ts.TypeChecker) (typ: Ts.Type) =
    mentionedTypes checker 0 typ
    |> List.choose (fun mentioned ->
        match mentioned.flags with
        | HasTypeFlags Ts.TypeFlags.TypeParameter -> Some mentioned.symbol.name
        | _ -> None
    )

/// `on<K>(event: Key<K, T>)` of `EventEmitter<T = DefaultEventMap>`: the checker defers
/// `Key<K, T>`, it resolves `Key<K, DefaultEventMap>`. The type parameters the condition names
/// are bound to their default or constraint, the ones the branches name only stay generic, and
/// the checker is asked again. A condition revealed by a branch is bound in turn.
let private tryResolveDeferred
    (reader: ITypeScriptReader)
    (typeNode: Ts.TypeNode)
    : GlueType option
    =
    let checker = reader.checker

    let rec resolve
        (bindings: Map<string, Ts.TypeNode>)
        (bound: Map<string, Ts.TypeNode>)
        (typ: Ts.Type)
        =
        let conditionals = deferredConditionals checker 0 typ

        if conditionals.IsEmpty then
            // `ChartOptions<ChartType>` is not an answer for `ChartOptions<TType>`: an answer
            // still made of a bound type is dropped, the reference stays
            // `never` or `string` in the answer says nothing, a named type does
            let substitutes =
                bound.Values
                |> Seq.filter (fun node -> node.kind = Ts.SyntaxKind.TypeReference)
                |> Seq.map checker.getTypeFromTypeNode
                |> Seq.toList

            let mentioned = mentionedTypes checker 0 typ

            let stillBound =
                substitutes
                |> List.exists (fun substitute ->
                    mentioned |> List.exists (fun typ -> obj.ReferenceEquals(typ, substitute))
                )

            if stillBound then
                None
            else
                Some typ
        else
            let alreadyBound = bound.Count

            let bound =
                (bound, conditionals)
                ||> List.fold (fun bound conditional ->
                    (bound,
                     typeParameterNamesOf checker conditional.checkType
                     @ typeParameterNamesOf checker conditional.extendsType)
                    ||> List.fold (fun bound name ->
                        match bindings.TryFind name with
                        | Some binding when not (bound.ContainsKey name) ->
                            bound.Add(name, binding)
                        | _ -> bound
                    )
                )

            // Nothing new to bind: the condition needs a parameter nothing binds
            if bound.Count = alreadyBound || not (mentionsBoundTypeParameter bound typeNode) then
                None
            else
                checker.getTypeFromTypeNode (substituteBoundTypeParameters bound typeNode)
                |> resolve bindings bound

    // Only a conditional, or an alias standing for one, is deferred. A reference to any other
    // alias stays a reference, the checker is not asked: the order of its answers shapes its
    // unions
    let rec standsForConditional (visited: Set<string>) (node: Ts.Node) : bool =
        match node.kind with
        | Ts.SyntaxKind.ConditionalType -> true
        | Ts.SyntaxKind.ParenthesizedType ->
            standsForConditional visited (node :?> Ts.ParenthesizedTypeNode).``type``
        | Ts.SyntaxKind.TypeReference ->
            let typeReferenceNode = node :?> Ts.TypeReferenceNode

            match
                symbolAtLocation checker !!typeReferenceNode.typeName
                |> Option.bind (resolveAlias checker)
            with
            | Some symbol ->
                let key = checker.getFullyQualifiedName symbol

                not (visited.Contains key)
                && (symbol.declarations
                    |> Option.map (
                        Seq.exists (fun declaration ->
                            declaration.kind = Ts.SyntaxKind.TypeAliasDeclaration
                            && standsForConditional
                                (visited.Add key)
                                (declaration :?> Ts.TypeAliasDeclaration).``type``
                        )
                    )
                    |> Option.defaultValue false)
            | None -> false
        | _ -> false

    let isConditional = standsForConditional Set.empty typeNode

    // `B extends PipelineDestination<A, any>` stays the constraint of `'B`, the transform reads
    // it as written. A default is a value, `P = RouteParameters<Route>` is resolved
    let rec isWithinConstraint (node: Ts.Node) =
        if isNull node || isNull node.parent then
            false
        elif node.parent.kind = Ts.SyntaxKind.TypeParameter then
            match (node.parent :?> Ts.TypeParameterDeclaration).``constraint`` with
            | Some constraint_ -> obj.ReferenceEquals(constraint_, node)
            | None -> false
        else
            isWithinConstraint node.parent

    let bindings = boundTypeParameters reader typeNode

    // A node naming no bound type parameter has nothing to be resolved with
    if
        not (isParseTreeNode typeNode)
        || not isConditional
        || isWithinConstraint typeNode
        || not (mentionsBoundTypeParameter bindings typeNode)
    then
        None
    else
        let typ = checker.getTypeAtLocation typeNode

        if (deferredConditionals checker 0 typ).IsEmpty then
            None
        else
            resolve bindings Map.empty typ
            |> Option.bind (fun resolved ->
                checker.typeToTypeNode (
                    resolved,
                    Some(typeNode :> Ts.Node),
                    Some typeNodeBuilderFlags
                )
            )
            |> Option.map (fun resolvedNode ->
                let previousContext = reader.SyntheticContext
                reader.SyntheticContext <- Some(typeNode :> Ts.Node)

                try
                    reader.ReadTypeNode resolvedNode
                finally
                    reader.SyntheticContext <- previousContext
            )

let rec private inferTypeNodes (node: Ts.Node) : Ts.InferTypeNode list =
    [
        if node.kind = Ts.SyntaxKind.InferType then
            node :?> Ts.InferTypeNode

        for child in node.getChildren () do
            yield! inferTypeNodes child
    ]

/// `Options<infer DateType>` stands for the constraint declared at its position, `Date` for
/// `interface Options<DateType extends Date>`
let private inferredConstraint
    (reader: ITypeScriptReader)
    (inferTypeNode: Ts.InferTypeNode)
    : GlueType
    =
    let declaredTypeParameters (declaration: Ts.Declaration) =
        match declaration.kind with
        | Ts.SyntaxKind.InterfaceDeclaration ->
            (declaration :?> Ts.InterfaceDeclaration).typeParameters
        | Ts.SyntaxKind.ClassDeclaration -> (declaration :?> Ts.ClassDeclaration).typeParameters
        | Ts.SyntaxKind.TypeAliasDeclaration ->
            (declaration :?> Ts.TypeAliasDeclaration).typeParameters
        | _ -> None

    match inferTypeNode.typeParameter.``constraint`` with
    | Some constraintNode -> reader.ReadTypeNode constraintNode
    | None ->
        let atPosition =
            match inferTypeNode.parent.kind with
            | Ts.SyntaxKind.TypeReference ->
                let typeReferenceNode = inferTypeNode.parent :?> Ts.TypeReferenceNode

                let position =
                    typeReferenceNode.typeArguments
                    |> Option.bind (
                        Seq.tryFindIndex (fun typeArgument ->
                            obj.ReferenceEquals(typeArgument, inferTypeNode)
                        )
                    )

                reader.checker.getSymbolAtLocation !!typeReferenceNode.typeName
                |> Option.bind (resolveAlias reader.checker)
                |> Option.bind (fun symbol -> symbol.declarations)
                |> Option.bind Seq.tryHead
                |> Option.bind declaredTypeParameters
                |> Option.bind (fun typeParameters ->
                    position
                    |> Option.bind (fun position ->
                        if position < typeParameters.Count then
                            typeParameters.[position].``constraint``
                        else
                            None
                    )
                )
            | _ -> None

        match atPosition with
        | Some constraintNode -> reader.ReadTypeNode constraintNode
        | None -> GlueType.Unknown

let private truncateToDeclaredArity
    (reader: ITypeScriptReader)
    (symbolOpt: Ts.Symbol option)
    (typeArguments: GlueType list)
    : GlueType list
    =
    let declaredArity =
        symbolOpt
        |> Option.bind (resolveAlias reader.checker)
        |> Option.bind (mainDeclaration reader.PackageContext)
        |> Option.bind (fun declaration ->
            match declaration.kind with
            | Ts.SyntaxKind.InterfaceDeclaration
            | Ts.SyntaxKind.ClassDeclaration
            | Ts.SyntaxKind.TypeAliasDeclaration ->
                let typeParameters: Ts.NodeArray<Ts.TypeParameterDeclaration> option =
                    declaration?typeParameters

                typeParameters |> Option.map _.Count |> Option.defaultValue 0 |> Some
            | _ -> None
        )

    match declaredArity with
    | Some arity when arity < typeArguments.Length -> List.truncate arity typeArguments
    | _ -> typeArguments

let private isGlobalThisQuery (typeNode: Ts.TypeNode) =
    typeNode.kind = Ts.SyntaxKind.TypeQuery
    && entityNameText !!(typeNode :?> Ts.TypeQueryNode).exprName = "globalThis"

let private readTypeUsingFlags (reader: ITypeScriptReader) (typ: Ts.Type) =

    match typ.flags with
    // An erroneous type has no symbol
    | HasTypeFlags Ts.TypeFlags.Object when isNull (box typ.symbol) ->
        GlueType.Primitive GluePrimitive.Any

    | HasTypeFlags Ts.TypeFlags.Object ->
        match typ.symbol.declarations with
        | Some declarations when declarations.Count > 0 ->
            let declaration = declarations.[0]

            match declaration.kind with
            | Ts.SyntaxKind.ClassDeclaration ->
                {
                    Documentation = []
                    FullName = reader.checker.getFullyQualifiedName typ.symbol
                    Name = typ.symbol.name
                    Constructors = []
                    Members = []
                    TypeParameters = []
                    HeritageClauses = []
                    IsExported = true
                }
                |> GlueType.ClassDeclaration

            // We don't support TypeQuery for ModuleDeclaration yet
            // See https://github.com/glutinum-org/cli/issues/70 for a possible solution
            | Ts.SyntaxKind.ModuleDeclaration -> GlueType.Discard
            // A reference keeps the module path of the declaration, its inlined declaration would not
            | Ts.SyntaxKind.InterfaceDeclaration
            | Ts.SyntaxKind.TypeAliasDeclaration ->
                let flags = typeNodeBuilderFlags

                match reader.checker.typeToTypeNode (typ, None, Some flags) with
                | Some typeNode -> reader.ReadTypeNode typeNode
                | None -> reader.ReadNode declaration
            | _ -> reader.ReadNode declaration

        | _ -> GlueType.Primitive GluePrimitive.Any
    | _ -> primitiveOfFlags typ

module UtilityType =
    let readExclude (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        let typ =
            reader.checker.getTypeFromTypeNode typeReferenceNode :?> Ts.UnionOrIntersectionType

        match typ.flags with
        | HasTypeFlags Ts.TypeFlags.StringLiteral
        | HasTypeFlags Ts.TypeFlags.NumberLiteral
        | HasTypeFlags Ts.TypeFlags.Union ->
            let cases =
                match typ.flags with
                | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                    match typ with
                    | Type.StringLiteral.String value ->
                        [ GlueLiteral.String value |> GlueType.Literal ]
                    | Type.StringLiteral.Other ->
                        Report.readerError (
                            "Exclude",
                            "Expected a string literal",
                            typeReferenceNode
                        )
                        |> reader.Warnings.Add

                        []

                | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                    match typ with
                    | Type.NumberLiteral.Int value -> [ GlueLiteral.Int value |> GlueType.Literal ]
                    | Type.NumberLiteral.Float value ->
                        [ GlueLiteral.Float value |> GlueType.Literal ]
                    | Type.NumberLiteral.Other ->
                        Report.readerError (
                            "Exclude",
                            "Expected a number literal",
                            typeReferenceNode
                        )
                        |> reader.Warnings.Add

                        []

                | _ ->
                    typ.types
                    |> Seq.toList
                    |> List.choose (fun typ ->
                        match typ.flags with
                        | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                            let literalType = typ :?> Ts.LiteralType

                            let value = unbox<string> literalType.value

                            GlueLiteral.String value |> GlueType.Literal |> Some
                        | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                            match typ with
                            | Type.NumberLiteral.Int value ->
                                GlueLiteral.Int value |> GlueType.Literal |> Some
                            | Type.NumberLiteral.Float value ->
                                GlueLiteral.Float value |> GlueType.Literal |> Some
                            | Type.NumberLiteral.Other ->
                                Report.readerError (
                                    "Exclude",
                                    "Expected a number literal",
                                    typeReferenceNode
                                )
                                |> reader.Warnings.Add

                                None
                        | _ -> None
                    )

            cases |> GlueTypeUnion |> GlueType.Union

        // `Exclude<T, undefined>` and `Exclude<T[K], undefined>` with `T` unknown are `T` and `T[K]`
        | HasTypeFlags Ts.TypeFlags.Conditional
        | HasTypeFlags Ts.TypeFlags.Any when
            (match typeReferenceNode.typeArguments with
             | Some typeArguments when typeArguments.Count > 1 ->
                 let excluded = typeArguments.[0]

                 let rec isNullish (typeNode: Ts.TypeNode) =
                     match typeNode.kind with
                     | Ts.SyntaxKind.UndefinedKeyword -> true
                     | Ts.SyntaxKind.LiteralType ->
                         (unbox<Ts.Node> (typeNode :?> Ts.LiteralTypeNode).literal).kind =
                             Ts.SyntaxKind.NullKeyword
                     | Ts.SyntaxKind.UnionType ->
                         (typeNode :?> Ts.UnionTypeNode).types |> Seq.forall isNullish
                     | _ -> false

                 isNullish typeArguments.[1]
                 || excluded.kind = Ts.SyntaxKind.IndexedAccessType
                 || (excluded.kind = Ts.SyntaxKind.TypeReference
                     && (
                         match
                             reader.checker.getSymbolAtLocation
                                 !!(excluded :?> Ts.TypeReferenceNode).typeName
                         with
                         | Some symbol ->
                             match symbol.flags with
                             | HasSymbolFlags Ts.SymbolFlags.TypeParameter -> true
                             | _ -> false
                         | None -> false
                     ))
             | _ -> false)
            ->
            reader.ReadTypeNode typeReferenceNode.typeArguments.Value.[0]

        // `Exclude<ComponentOption["type"], undefined>` widened to `string`
        | HasTypeFlags Ts.TypeFlags.String -> GlueType.Primitive GluePrimitive.String
        | HasTypeFlags Ts.TypeFlags.Number -> GlueType.Primitive GluePrimitive.Number
        | HasTypeFlags Ts.TypeFlags.Boolean -> GlueType.Primitive GluePrimitive.Bool

        // `Exclude<DeepPartial<Options<TType>>, ...>`: the branch depends on the type arguments
        | HasTypeFlags Ts.TypeFlags.Conditional -> GlueType.Primitive GluePrimitive.Any

        | _ ->
            Report.readerError (
                "Exclude",
                "Was expecting the resolved type to be a literal or a union",
                typeReferenceNode
            )
            |> reader.Warnings.Add

            GlueType.Primitive GluePrimitive.Any

    /// The members of the type from their declarations, `contextNode` locates the errors
    /// `Pick<typeof globalThis, 'DocumentFragment'>`: the global is a variable merged with an
    /// interface, only a declaration standing for a member is read as one
    let private isMemberDeclaration (declaration: Ts.Declaration) =
        match declaration.kind with
        | Ts.SyntaxKind.InterfaceDeclaration
        | Ts.SyntaxKind.ClassDeclaration
        | Ts.SyntaxKind.TypeAliasDeclaration
        | Ts.SyntaxKind.EnumDeclaration
        | Ts.SyntaxKind.ModuleDeclaration -> false
        | _ -> true

    /// The members of a property: one per declaration standing for a member, else from its type
    let private readPropertyMembers
        (reader: ITypeScriptReader)
        (contextNode: Ts.Node)
        (property: Ts.Symbol)
        : GlueMember list
        =
        let declarations =
            property.declarations
            |> Option.map (Seq.filter isMemberDeclaration >> Seq.toList)
            |> Option.defaultValue []

        match declarations with
        | [] -> readPropertyWithoutDeclaration reader contextNode property |> Option.toList
        | declarations ->
            declarations |> List.map (readInstantiatedMember reader contextNode property)

    let private readMembers (reader: ITypeScriptReader) (contextNode: Ts.Node) (typ: Ts.Type) =
        typ
        |> reader.checker.getPropertiesOfType
        |> Seq.toList
        |> List.collect (readPropertyMembers reader contextNode)
        |> List.distinct

    /// <summary>
    /// When a generic type reference is applied with concrete type arguments
    /// (e.g. a custom utility type like <c>Picked&lt;User, "id"&gt;</c>), the
    /// TypeChecker resolves it to an anonymous object type. We read its resolved
    /// members (via <see cref="readMembers"/>) and expand it into a TypeLiteral
    /// so it is generated as a concrete interface instead of an unusable
    /// reference to the (generic) utility.
    /// </summary>
    let tryExpandAnonymousObjectApplication
        (reader: ITypeScriptReader)
        (typeReferenceNode: Ts.TypeReferenceNode)
        : GlueType option
        =
        let hasTypeArguments =
            match typeReferenceNode.typeArguments with
            | Some typeArguments -> typeArguments.Count > 0
            | None -> false

        if not hasTypeArguments then
            None
        else
            let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

            // Only expand *anonymous* object types synthesized by the utility
            // (e.g. a TypeLiteral or a mapped type like `Pick`). References to
            // named interfaces/classes (e.g. `Foo<string>`) must stay references.
            let isNamedDeclaration =
                if isNull (box typ.symbol) then
                    false
                else
                    match typ.symbol.declarations with
                    // `namespace Selection {}` merged with `interface Selection {}` is declared first
                    | Some declarations ->
                        declarations
                        |> Seq.exists (fun declaration ->
                            match declaration.kind with
                            | Ts.SyntaxKind.InterfaceDeclaration
                            | Ts.SyntaxKind.ClassDeclaration -> true
                            | _ -> false
                        )
                    | None -> false

            let isTypeAliasApplication =
                match reader.checker.getSymbolAtLocation !!typeReferenceNode.typeName with
                | Some symbol ->
                    match symbol.flags with
                    | HasSymbolFlags Ts.SymbolFlags.TypeAlias -> true
                    | _ -> false
                | None -> false

            let isInProgress =
                reader.InProgress.Expansions
                |> Seq.exists (fun inProgress -> obj.ReferenceEquals(inProgress, typ))

            // `ProxiedObject<P>` of `P extends Array<Node>`: the members of a mapped type applied
            // to a type parameter are the ones of its constraint
            let isGenericMapped =
                match typ.flags with
                | HasTypeFlags Ts.TypeFlags.Object ->
                    int (typ :?> Ts.ObjectType).objectFlags &&& int Ts.ObjectFlags.Mapped <> 0
                    && typeReferenceNode.typeArguments.Value
                       |> Seq.exists (fun argument ->
                           match (reader.checker.getTypeFromTypeNode argument).flags with
                           | HasTypeFlags Ts.TypeFlags.TypeParameter -> true
                           | _ -> false
                       )
                | _ -> false

            // `readonly [...ObservableInputTuple<A>]` is an array too
            let isArrayLike: bool = reader.checker?isArrayLikeType (typ)

            match typ.flags with
            // `Parameters<F>` is a tuple, not the object made of the array members
            | HasTypeFlags Ts.TypeFlags.Object when
                (isTupleType typ || (isArrayLike && not isNamedDeclaration)) && not isInProgress
                ->
                let flags = typeNodeBuilderFlags

                reader.checker.typeToTypeNode (typ, None, Some flags)
                |> Option.map reader.ReadTypeNode
            // A recursive application (e.g. `swap(): Pair<S, C>` inside `Pair<C, S>`) stays a reference
            | HasTypeFlags Ts.TypeFlags.Object when
                not isNamedDeclaration && not isInProgress && not isGenericMapped
                ->
                let members =
                    withInProgress
                        reader.InProgress.Expansions
                        typ
                        (fun () -> readMembers reader typeReferenceNode typ)

                if members.IsEmpty then
                    None
                else
                    ({ Members = members; Id = None }: GlueTypeLiteral)
                    |> GlueType.TypeLiteral
                    |> Some
            | HasTypeFlags Ts.TypeFlags.String
            | HasTypeFlags Ts.TypeFlags.Number
            | HasTypeFlags Ts.TypeFlags.Boolean when isTypeAliasApplication ->
                readTypeUsingFlags reader typ |> Some
            | _ -> None

    /// A node synthesized by `typeToTypeNode` is unknown to the checker, its identifier still
    /// carries the symbol: the declared type stands in, `defaultTypeArguments` binds its parameters
    let private baseTypeOf (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : Ts.Type =
        let typ = reader.checker.getTypeFromTypeNode typeNode

        match typ.flags with
        | HasTypeFlags Ts.TypeFlags.Any when
            typeNode.pos < 0 && typeNode.kind = Ts.SyntaxKind.TypeReference
            ->
            symbolAtLocation reader.checker !!(typeNode :?> Ts.TypeReferenceNode).typeName
            |> Option.bind (resolveAlias reader.checker)
            |> Option.map reader.checker.getDeclaredTypeOfSymbol
            |> Option.defaultValue typ
        | _ -> typ

    /// `"a" | "b"` written by `typeToTypeNode`, the checker can't type the node
    let rec private literalKeysOf (typeNode: Ts.TypeNode) : string list =
        match typeNode.kind with
        | Ts.SyntaxKind.LiteralType ->
            let literal: Ts.Node = !!(typeNode :?> Ts.LiteralTypeNode).literal

            if literal.kind = Ts.SyntaxKind.StringLiteral then
                [ (literal :?> Ts.StringLiteral).text ]
            else
                []
        | Ts.SyntaxKind.UnionType ->
            (typeNode :?> Ts.UnionTypeNode).types
            |> Seq.toList
            |> List.collect literalKeysOf
        | Ts.SyntaxKind.ParenthesizedType ->
            literalKeysOf (typeNode :?> Ts.ParenthesizedTypeNode).``type``
        | _ -> []

    /// `Partial<{ padding: Scriptable<key> }>` written by `typeToTypeNode` names the variable of
    /// the mapped type it was taken from, unknown where the members end up
    let private withoutForeignTypeParameters
        (reader: ITypeScriptReader)
        (bound: Collections.Map<string, GlueType>)
        (members: GlueMember list)
        =
        let rec declaredAbove (node: Ts.Node) =
            if isNull node then
                []
            else
                let typeParameters: Ts.NodeArray<Ts.TypeParameterDeclaration> option =
                    node?typeParameters

                (match typeParameters with
                 | Some typeParameters ->
                     typeParameters |> Seq.toList |> List.map (fun p -> identifierText p.name)
                 | None -> [])
                @ declaredAbove node.parent

        let inScope =
            match reader.SyntheticContext with
            | Some context -> declaredAbove context
            | None -> []

        let signatureMentions (returnType: GlueType) (parameters: GlueParameter list) =
            GlueSubstitution.mentionedTypeParameters returnType
            @ (parameters
               |> List.collect (fun parameter ->
                   GlueSubstitution.mentionedTypeParameters parameter.Type
               ))

        let mentioned (glueMember: GlueMember) =
            match glueMember with
            | GlueMember.Property property -> GlueSubstitution.mentionedTypeParameters property.Type
            | GlueMember.Method method -> signatureMentions method.Type method.Parameters
            | GlueMember.MethodSignature method -> signatureMentions method.Type method.Parameters
            | _ -> []

        let foreign =
            members
            |> List.collect mentioned
            |> List.distinct
            |> List.filter (fun name ->
                not (List.contains name inScope) && not (bound.ContainsKey name)
            )
            |> List.map (fun name -> name, GlueType.Primitive GluePrimitive.Any)
            |> Collections.Map.ofList

        if foreign.IsEmpty then
            members
        else
            members |> List.map (GlueSubstitution.substituteMember foreign)

    let readPartial (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        let baseType = baseTypeOf reader typeReferenceNode.typeArguments.Value[0]

        if
            reader.InProgress.Partials
            |> Seq.exists (fun typ -> obj.ReferenceEquals(typ, baseType))
        then
            Report.readerError (
                "Partial",
                "Recursive Partial is not supported, defaulting to obj",
                typeReferenceNode
            )
            |> reader.Warnings.Add

            GlueType.Primitive GluePrimitive.Any
        else
            withInProgress
                reader.InProgress.Partials
                baseType
                (fun () ->
                    // `Partial<[x: number, order?: Order]>` is the tuple with optional elements
                    if isTupleType baseType then
                        let flags = typeNodeBuilderFlags

                        reader.checker.typeToTypeNode (
                            reader.checker.getTypeFromTypeNode typeReferenceNode,
                            None,
                            Some flags
                        )
                        |> reader.ReadTypeNode
                    else

                        let baseNode = typeReferenceNode.typeArguments.Value[0]

                        let members =
                            match baseType.flags with
                            // `Partial<any>`
                            | HasTypeFlags Ts.TypeFlags.Any when
                                baseNode.kind = Ts.SyntaxKind.AnyKeyword
                                ->
                                []
                            // `Partial<{ padding: number }>` written by `typeToTypeNode`
                            | HasTypeFlags Ts.TypeFlags.Any when
                                baseNode.pos < 0 && baseNode.kind = Ts.SyntaxKind.TypeLiteral
                                ->
                                match reader.ReadTypeNode baseNode with
                                | GlueType.TypeLiteral typeLiteral -> typeLiteral.Members
                                | _ -> []
                            | HasTypeFlags Ts.TypeFlags.Any ->
                                Report.readerError (
                                    "partial inner type",
                                    "Was not able to resolve the inner type, and defaulting to any. If the base type is defined, in another file, please make sure to include it in the input files",
                                    typeReferenceNode
                                )
                                |> reader.Warnings.Add

                                []

                            | _ -> baseType |> readMembers reader typeReferenceNode

                        let defaults =
                            defaultTypeArguments reader typeReferenceNode.typeArguments.Value[0]

                        let members =
                            members
                            |> List.map (GlueSubstitution.substituteMember defaults)
                            |> fun members ->
                                if typeReferenceNode.pos < 0 then
                                    withoutForeignTypeParameters reader defaults members
                                else
                                    members

                        ({
                            Documentation = []
                            FullName = getFullNameOrEmpty reader.checker typeReferenceNode
                            Name = entityNameText !!typeReferenceNode.typeName
                            Members = members
                            TypeParameters = []
                            HeritageClauses = []
                        }
                        : GlueInterface)
                        |> GlueUtilityType.Partial
                        |> GlueType.UtilityType
                )

    let readRecord (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        let typeArguments = readTypeArguments reader typeReferenceNode

        ({
            KeyType = typeArguments.[0]
            ValueType = typeArguments.[1]
        }
        : GlueRecord)
        |> GlueUtilityType.Record
        |> GlueType.UtilityType

    let readReturnType (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        let rawTyp = reader.checker.getTypeFromTypeNode typeReferenceNode

        // When the type is still deferred (e.g. `ReturnType<this["clone"]>` where
        // `this` is polymorphic), resolve it to its apparent type so we get a
        // concrete type instead of an unusable reference. We only do this for
        // deferred types to avoid widening primitives (`string` -> `String`) or
        // type parameters.
        let typ =
            match rawTyp.flags with
            | HasTypeFlags Ts.TypeFlags.Conditional
            | HasTypeFlags Ts.TypeFlags.IndexedAccess
            | HasTypeFlags Ts.TypeFlags.Substitution
            | HasTypeFlags Ts.TypeFlags.Index -> reader.checker.getApparentType rawTyp
            | _ -> rawTyp

        // `typeToTypeNode` synthesizes the node, the identity is the literal the type is declared by
        let declaredId () =
            match typ.getSymbol () with
            | Some symbol ->
                match symbol.declarations with
                | Some declarations when declarations.Count > 0 -> typeLiteralId declarations.[0]
                | _ -> None
            | None -> None

        match reader.checker.typeToTypeNode (typ, None, None) with
        | Some typeNode ->
            (match reader.ReadTypeNode typeNode with
             | GlueType.TypeLiteral info when info.Id.IsNone ->
                 GlueType.TypeLiteral { info with Id = declaredId () }
             | glueType -> glueType)
            |> GlueUtilityType.ReturnType
            |> GlueType.UtilityType
        | None ->
            readTypeUsingFlags reader typ
            |> GlueUtilityType.ReturnType
            |> GlueType.UtilityType

    let readThisParameterType
        (reader: ITypeScriptReader)
        (typeReferenceNode: Ts.TypeReferenceNode)
        =
        let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

        match reader.checker.typeToTypeNode (typ, None, None) with
        | Some typeNode ->
            reader.ReadTypeNode typeNode
            |> GlueUtilityType.ThisParameterType
            |> GlueType.UtilityType
        | None ->
            readTypeUsingFlags reader typ
            |> GlueUtilityType.ThisParameterType
            |> GlueType.UtilityType

    let readOmitOrPick
        (reader: ITypeScriptReader)
        (typeReferenceNode: Ts.TypeReferenceNode)
        (keepListedKeys: bool)
        =

        let keysToOmitType =
            typeReferenceNode.typeArguments.Value[1] |> reader.checker.getTypeFromTypeNode

        // `Pick<Locale, LocaleFields>` names its keys through a type parameter, the members
        // it stands for are only known once the parameter is bound
        let hasUnboundKeys =
            match keysToOmitType.flags with
            | HasTypeFlags Ts.TypeFlags.TypeParameter
            | HasTypeFlags Ts.TypeFlags.Index
            | HasTypeFlags Ts.TypeFlags.IndexedAccess -> true
            | _ -> false

        let tryReadValueOfKeys (typ: Ts.Type) =
            match typ with
            | Type.StringLiteral.String value -> Some value
            | Type.StringLiteral.Other ->
                Report.readerError ("keysToOmit", "Expected a string literal", typeReferenceNode)
                |> reader.Warnings.Add

                None

        let keysToOmit =
            match keysToOmitType.flags with
            | _ when hasUnboundKeys -> Seq.empty
            | HasTypeFlags Ts.TypeFlags.Any when typeReferenceNode.pos < 0 ->
                literalKeysOf typeReferenceNode.typeArguments.Value[1] |> Seq.ofList
            | _ ->
                if keysToOmitType.isUnion () then
                    (keysToOmitType :?> Ts.UnionOrIntersectionType).types
                    |> Seq.choose tryReadValueOfKeys
                else
                    tryReadValueOfKeys keysToOmitType
                    |> Option.map Seq.singleton
                    |> Option.defaultValue []

        let baseType = baseTypeOf reader typeReferenceNode.typeArguments.Value[0]

        // `Omit<{ uid: string } & { type: T }, "uid">` written by `typeToTypeNode`
        let literalMembers =
            match baseType.flags with
            | HasTypeFlags Ts.TypeFlags.Any when typeReferenceNode.pos < 0 ->
                match reader.ReadTypeNode typeReferenceNode.typeArguments.Value[0] with
                | GlueType.IntersectionType members -> Some members
                | GlueType.TypeLiteral typeLiteral -> Some typeLiteral.Members
                | _ -> None
            | _ -> None

        let baseProperties =
            match baseType.flags with
            | HasTypeFlags Ts.TypeFlags.Any when literalMembers.IsSome -> ResizeArray []
            | HasTypeFlags Ts.TypeFlags.Any ->
                Report.readerError (
                    "omit base type",
                    "Was not able to resolve the base type, and defaulting to any. If the base type is defined, in another file, please make sure to include it in the input files",
                    typeReferenceNode
                )
                |> reader.Warnings.Add

                ResizeArray []
            | _ -> baseType |> reader.checker.getPropertiesOfType

        let filteredProperties =
            baseProperties
            |> Seq.filter (fun prop -> keysToOmit |> Seq.contains prop.name |> (=) keepListedKeys)
            |> Seq.toList

        // `secondBest?: Omit<HighlightResult, 'second_best'>` names its own interface, the
        // inner one is `any`
        let isInProgress =
            reader.InProgress.Expansions
            |> Seq.exists (fun inProgress -> obj.ReferenceEquals(inProgress, baseType))

        let members =
            if isInProgress then
                []
            else
                withInProgress
                    reader.InProgress.Expansions
                    baseType
                    (fun () ->
                        filteredProperties
                        |> List.collect (readPropertyMembers reader typeReferenceNode)
                    )

        let defaults = defaultTypeArguments reader typeReferenceNode.typeArguments.Value[0]

        let memberName (glueMember: GlueMember) =
            match glueMember with
            | GlueMember.Property property -> Some property.Name
            | GlueMember.Method method -> Some method.Name
            | GlueMember.MethodSignature methodSignature -> Some methodSignature.Name
            | GlueMember.GetAccessor accessor -> Some accessor.Name
            | GlueMember.SetAccessor accessor -> Some accessor.Name
            | GlueMember.CallSignature _
            | GlueMember.IndexSignature _
            | GlueMember.ConstructSignature _ -> None

        let members =
            match literalMembers with
            | Some literalMembers ->
                literalMembers
                |> List.filter (fun glueMember ->
                    match memberName glueMember with
                    | Some name -> keysToOmit |> Seq.contains name |> (=) keepListedKeys
                    | None -> not keepListedKeys
                )
            | None -> members

        members
        |> List.map (GlueSubstitution.substituteMember defaults)
        |> fun members ->
            if typeReferenceNode.pos < 0 then
                withoutForeignTypeParameters reader defaults members
            else
                members
        |> fun members ->
            if hasUnboundKeys || isInProgress then
                GlueType.Primitive GluePrimitive.Any
            else
                members
                |> (if keepListedKeys then
                        GlueUtilityType.Pick
                    else
                        GlueUtilityType.Omit)
                |> GlueType.UtilityType

    let readOmit (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        readOmitOrPick reader typeReferenceNode false

    let readPick (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        readOmitOrPick reader typeReferenceNode true

    let readReadonly (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =

        let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

        match typ.flags with
        | HasTypeFlags Ts.TypeFlags.Object
        | HasTypeFlags Ts.TypeFlags.Intersection ->
            typ
            |> readMembers reader typeReferenceNode
            |> GlueReadonly.Members
            |> GlueUtilityType.Readonly
            |> GlueType.UtilityType
        | HasTypeFlags Ts.TypeFlags.Union ->
            let unionType = typ :?> Ts.UnionOrIntersectionType

            try

                let interfaces =
                    unionType.types
                    |> Seq.choose (fun innerType ->
                        if innerType.flags.HasFlag Ts.TypeFlags.Object then

                            ({
                                Documentation = []
                                Name = innerType.aliasTypeArguments.Value[0].symbol.name
                                FullName =
                                    reader.checker.getFullyQualifiedName
                                        innerType.aliasTypeArguments.Value[0].symbol
                                Members = readMembers reader typeReferenceNode innerType
                                TypeParameters = []
                                HeritageClauses = []
                            }
                            : GlueInterface)
                            |> Some
                        else
                            None
                    )
                    |> Seq.toList

                interfaces
                |> GlueReadonly.Union
                |> GlueUtilityType.Readonly
                |> GlueType.UtilityType
            with _ ->
                Report.readerError (
                    "Readonly",
                    "Unable to read the members of the union type",
                    typeReferenceNode
                )
                |> reader.Warnings.Add

                GlueType.Primitive GluePrimitive.Any

        | _ -> GlueType.Primitive GluePrimitive.Any

let private readTypeReference (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode

    let symbolOpt = symbolAtLocation checker !!typeReferenceNode.typeName

    // `IfDefaultsTrue<true, A, B>`: the checker resolves a conditional alias applied to concrete arguments
    let tryReadResolvedConditional () =
        let isConditionalAlias =
            match symbolOpt |> Option.bind (resolveAlias checker) with
            | Some symbol ->
                match symbol.declarations with
                | Some declarations when declarations.Count > 0 ->
                    let declaration = declarations.[0]

                    declaration.kind = Ts.SyntaxKind.TypeAliasDeclaration
                    && (declaration :?> Ts.TypeAliasDeclaration).``type``.kind =
                        Ts.SyntaxKind.ConditionalType
                | _ -> false
            | None -> false

        if typeReferenceNode.pos < 0 || not isConditionalAlias then
            None
        else
            let typ = checker.getTypeFromTypeNode typeReferenceNode

            let isUnknown =
                match typ.flags with
                | HasTypeFlags Ts.TypeFlags.Any
                | HasTypeFlags Ts.TypeFlags.Never -> true
                | _ -> false

            if isDeferredConditional typ || isUnknown then
                None
            else
                let flags = typeNodeBuilderFlags

                checker.typeToTypeNode (typ, Some(typeReferenceNode :> Ts.Node), Some flags)
                |> Option.map reader.ReadTypeNode

    let readTypeReference (isStandardLibrary: bool) =

        let isTypeParameter =
            match symbolOpt with
            | Some symbol ->
                match symbol.flags with
                | HasSymbolFlags Ts.SymbolFlags.TypeParameter -> true
                | _ -> false
            | None -> false

        if isTypeParameter then
            symbolOpt.Value.name |> GlueType.TypeParameter
        else

            match
                UtilityType.tryExpandAnonymousObjectApplication reader typeReferenceNode
                |> Option.orElseWith tryReadResolvedConditional
                |> Option.orElseWith (fun () -> tryResolveDeferred reader typeNode)
            with
            | Some glueType -> glueType
            | None ->
                let isQualified = typeReferenceNode.typeName?kind = Ts.SyntaxKind.QualifiedName

                // The namespaces of a qualified name are part of the module path
                let writtenName () =
                    if isQualified then
                        (unbox<Ts.QualifiedName> typeReferenceNode.typeName).right.text
                    else
                        identifierText !!typeReferenceNode.typeName

                let name =
                    match symbolOpt with
                    | Some symbol ->
                        importedName checker symbol
                        |> Option.orElse (
                            symbol.valueDeclaration
                            |> Option.map (fun valueDeclaration ->
                                // If the type reference an enum member,
                                // we need to find the name of the Enum type, not the name of the member
                                match valueDeclaration.kind with
                                | Ts.SyntaxKind.EnumMember ->
                                    valueDeclaration?symbol?parent?getName()
                                | Ts.SyntaxKind.EnumDeclaration when isQualified -> symbol.name
                                | _ -> writtenName ()
                            )
                        )
                        |> Option.defaultValue (writtenName ())
                    | None -> writtenName ()

                // A name TypeScript itself can't resolve has no declaration to generate
                let isUnresolved =
                    reader.PackageContext.IsSome && symbolOpt.IsNone && typeReferenceNode.pos >= 0

                let isExternal = isExternalToPackages checker reader.PackageContext symbolOpt

                // `InferIssue<ReturnType<TReference>>`: the checker elides a type too deep
                // to write out as `...`, neither it nor the application taking it is usable
                let isElided =
                    name = "..."
                    || (
                        match typeReferenceNode.typeArguments with
                        | Some typeArguments ->
                            typeArguments
                            |> Seq.exists (fun typeArgument ->
                                typeArgument.kind = Ts.SyntaxKind.TypeReference
                                && typeArgument?typeName?escapedText = "..."
                            )
                        | None -> false
                    )

                if
                    isElided
                    || isUnresolved
                    || (isExternal && not (knownExternalTypeNames.Contains name))
                then
                    GlueType.Primitive GluePrimitive.Any
                else
                    ({
                        Name =
                            if name.Contains "." then
                                name
                            else
                                Naming.sanitizeTypeName name
                        FullName = getFullNameOrEmpty checker (!!typeReferenceNode.typeName)
                        ModulePath =
                            if isLibraryName reader name then
                                []
                            else
                                modulePathForSymbol
                                    checker
                                    reader.PackageContext
                                    isQualified
                                    symbolOpt
                        TypeArguments =
                            // `MessageEvent<T>` of the DOM lib merged with a non-generic
                            // `interface MessageEvent` of a package: the arguments of the
                            // declaration read are kept
                            readTypeArguments reader typeReferenceNode
                            |> truncateToDeclaredArity reader symbolOpt
                        // `Uint8Array` from `lib.es2015` is mapped like the `lib.es5` types
                        IsStandardLibrary =
                            isStandardLibrary || isExternal || isLibraryName reader name
                    })
                    |> GlueType.TypeReference

    // `Uppercase<"abc">`: the literals the checker resolves an intrinsic type to, else `string`
    let readIntrinsicString () =
        let rec literals (typ: Ts.Type) : GlueType list option =
            match typ.flags with
            | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                match typ with
                | Type.StringLiteral.String value ->
                    Some [ GlueLiteral.String value |> GlueType.Literal ]
                | Type.StringLiteral.Other -> None
            | HasTypeFlags Ts.TypeFlags.Union ->
                (typ :?> Ts.UnionType).types
                |> Seq.toList
                |> List.map literals
                |> List.fold
                    (fun acc cases ->
                        match acc, cases with
                        | Some acc, Some cases -> Some(acc @ cases)
                        | _ -> None
                    )
                    (Some [])
            | _ -> None

        if typeReferenceNode.pos < 0 then
            GlueType.Primitive GluePrimitive.String
        else
            match literals (checker.getTypeFromTypeNode typeReferenceNode) with
            | Some [ single ] -> single
            | Some(_ :: _ as cases) -> cases |> GlueTypeUnion |> GlueType.Union
            | _ -> GlueType.Primitive GluePrimitive.String

    // `NoInfer<T>` only changes the inference of `T`, its intrinsic symbol has no declaration
    let isNoInfer =
        entityNameText !!typeReferenceNode.typeName = "NoInfer"
        && typeReferenceNode.typeArguments.IsSome
        && (symbolOpt |> Option.bind (fun symbol -> symbol.declarations) |> Option.isNone
            || isFromEs5Lib symbolOpt)

    if isNoInfer then
        reader.ReadTypeNode typeReferenceNode.typeArguments.Value.[0]
    elif isFromEs5Lib symbolOpt then
        match getFullNameOrEmpty checker (!!typeReferenceNode.typeName) with
        | "Exclude" -> UtilityType.readExclude reader typeReferenceNode
        | "Uppercase"
        | "Lowercase"
        | "Capitalize"
        | "Uncapitalize" -> readIntrinsicString ()
        | "Partial" -> UtilityType.readPartial reader typeReferenceNode
        | "Record" -> UtilityType.readRecord reader typeReferenceNode
        | "ReturnType" -> UtilityType.readReturnType reader typeReferenceNode
        | "ThisParameterType" -> UtilityType.readThisParameterType reader typeReferenceNode
        // The checker resolves the mapped type, the reader stands in for a synthesized node
        | "Omit" ->
            UtilityType.tryExpandAnonymousObjectApplication reader typeReferenceNode
            |> Option.defaultWith (fun () -> UtilityType.readOmit reader typeReferenceNode)
        | "Pick" ->
            UtilityType.tryExpandAnonymousObjectApplication reader typeReferenceNode
            |> Option.defaultWith (fun () -> UtilityType.readPick reader typeReferenceNode)
        | "Readonly" -> UtilityType.readReadonly reader typeReferenceNode
        | _ -> readTypeReference true
    else
        readTypeReference (isFromEsLib symbolOpt)

let private readFunctionType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let functionTypeNode = typeNode :?> Ts.FunctionTypeNode

    let typeParameters =
        // The delegate generated for the function needs the type parameters of the
        // enclosing declarations too (e.g. the class of a method taking a callback)
        let rec collectEnclosing (node: Ts.Node) (acc: Ts.TypeParameterDeclaration list) =
            if isNull node then
                acc
            // `FacetConfig<Input, Output>` expanded by the checker: the members have no parent
            elif
                isNull node.parent
                && node.pos < 0
                && reader.SyntheticContext.IsSome
                && not (obj.ReferenceEquals(node, reader.SyntheticContext.Value))
            then
                collectEnclosing reader.SyntheticContext.Value acc
            // A function type used as a constraint is read while reading the type parameters
            elif node.kind = Ts.SyntaxKind.TypeParameter then
                []
            else
                let ownTypeParameters: Ts.NodeArray<Ts.TypeParameterDeclaration> option =
                    node?typeParameters

                let acc =
                    match ownTypeParameters with
                    | Some ownTypeParameters -> acc @ Seq.toList ownTypeParameters
                    | None -> acc

                collectEnclosing node.parent acc

        // `static define<Input, Output>` of `class Facet<Input, Output>`: the innermost wins
        match
            collectEnclosing functionTypeNode []
            |> List.distinctBy (fun typeParameter -> identifierText typeParameter.name)
        with
        | [] -> []
        | typParameters ->
            reader.ReadTypeParameters(Some(ts.factory.createNodeArray (ResizeArray typParameters)))

    {
        Documentation = reader.ReadDocumentationFromNode typeNode
        Type = reader.ReadTypeNode functionTypeNode.``type``
        TypeParameters = typeParameters
        OwnTypeParameterNames =
            match functionTypeNode.typeParameters with
            | Some own ->
                own
                |> Seq.map (fun typeParameter -> identifierText typeParameter.name)
                |> Seq.toList
            | None -> []
        Parameters = reader.ReadParameters functionTypeNode.parameters
    }
    |> GlueType.FunctionType

/// `import("./file").Foo<T>`, produced by `typeToTypeNode` for a type not imported in the current file
let private readImportType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let importTypeNode = typeNode :?> Ts.ImportTypeNode

    let unresolvedModule =
        // A node synthesized by `typeToTypeNode` has no position to resolve its module from
        if importTypeNode.pos < 0 then
            None
        else
            match importTypeNode.argument.kind with
            | Ts.SyntaxKind.LiteralType ->
                let literal = (importTypeNode.argument :?> Ts.LiteralTypeNode).literal

                if (symbolAtLocation checker !!literal).IsSome then
                    None
                else
                    Some(!!literal?text: string)
            | _ -> None

    match unresolvedModule, importTypeNode.qualifier with
    // `import("three").WebGLRenderer` of a package that is not installed
    | Some moduleName, _ ->
        let warning =
            $"'%s{moduleName}' is not installed, the types imported from it are generated as 'obj'"

        if not (reader.Warnings.Contains warning) then
            reader.Warnings.Add warning

        GlueType.Primitive GluePrimitive.Any
    // `typeof import("./file").fn` is the type of the value
    | None, Some qualifier when importTypeNode.isTypeOf ->
        ts.factory.createTypeQueryNode (unbox<Ts.Identifier> qualifier)
        |> reader.ReadTypeNode
    | None, Some qualifier ->
        ts.factory.createTypeReferenceNode (
            unbox<Ts.Identifier> qualifier,
            ?typeArguments = unbox<ResizeArray<Ts.TypeNode> option> importTypeNode.typeArguments
        )
        |> reader.ReadTypeNode
    // `typeof import("./file")`, the module object
    | None, None -> GlueType.Primitive GluePrimitive.Any

let private readThisType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let thisTypeNode = typeNode :?> Ts.ThisTypeNode

    // Probably a naive implementation but hopefully it will cover
    // most of the cases
    // We can't use the reader to get the fulltype because we would end
    // up in a infinite loop
    let typ = checker.getTypeAtLocation thisTypeNode

    // An erroneous type has no symbol
    let declarations =
        if isNull (box typ.symbol) then
            None
        else
            typ.symbol.declarations

    let typParameters =
        match declarations with
        | Some declarations ->
            // The interface merged with a namespace, or augmented by another module,
            // declares its type parameters on one of its declarations
            declarations
            |> Seq.choose (fun declaration ->
                match declaration.kind with
                | Ts.SyntaxKind.ClassDeclaration
                | Ts.SyntaxKind.InterfaceDeclaration ->
                    let classDeclaration = declaration :?> Ts.InterfaceDeclaration

                    classDeclaration.typeParameters
                | _ -> None
            )
            |> Seq.sortByDescending (fun typeParameters -> typeParameters.Count)
            |> Seq.tryHead
            |> Option.map (Some >> reader.ReadTypeParameters)
            |> Option.defaultValue []
        | None -> []

    if isNull (box typ.symbol) then
        GlueType.Primitive GluePrimitive.Any
    else

        ({
            Name = declaredName typ.symbol |> Option.defaultValue typ.symbol.name
            TypeParameters = typParameters
        }
        : GlueThisType)
        |> GlueType.ThisType

let private readIntersectionConstituents
    (reader: ITypeScriptReader)
    (typeNode: Ts.TypeNode)
    : GlueType
    =
    let checker = reader.checker

    let intersectionTypeNode = typeNode :?> Ts.IntersectionTypeNode

    let unionOrIntersectionType =
        checker.getTypeAtLocation intersectionTypeNode :?> Ts.UnionOrIntersectionType

    reader.InProgress.Intersections.Add unionOrIntersectionType

    use _guard =
        { new System.IDisposable with
            member _.Dispose() =
                reader.InProgress.Intersections.RemoveAt(reader.InProgress.Intersections.Count - 1)
        }

    let properties =
        let computedProperties =
            if unionOrIntersectionType.isUnion () then
                unionOrIntersectionType.types
                |> Seq.toList
                |> List.map (checker.getPropertiesOfType >> Seq.toList)
                |> List.concat
                |> List.distinct

            else
                match unionOrIntersectionType.getProperties () |> Seq.toList with
                // `DeepPartial<Registry[T]> & Properties<T>`: the checker gives up on the
                // deferred part, the members of the others are still known
                | [] when (unbox<Ts.Node> intersectionTypeNode).pos >= 0 ->
                    intersectionTypeNode.types
                    |> Seq.toList
                    |> List.collect (fun constituent ->
                        checker.getTypeAtLocation (constituent :> Ts.Node)
                        |> checker.getPropertiesOfType
                        |> Seq.toList
                    )
                    |> List.distinctBy (fun property -> property.name)
                | properties -> properties

        computedProperties
        |> List.choose (fun property ->
            match property.declarations with
            | Some declarations ->
                if declarations.Count = 1 then
                    Some(Single(property, declarations.[0]))
                // `type` declared by the dataset options of every chart type: the checker
                // knows the type of the merged property
                elif
                    declarations
                    |> Seq.forall (fun declaration ->
                        declaration.kind = Ts.SyntaxKind.PropertySignature
                        || declaration.kind = Ts.SyntaxKind.PropertyDeclaration
                    )
                then
                    Some(WithoutDeclaration property)
                else
                    Some ForceAny
            | None -> Some(WithoutDeclaration property)
        )

    // `{ new (...args: any[]): any } & typeof Class` describes the class itself, reading its
    // members would end up in an infinite loop
    let intersectsATypeQuery =
        intersectionTypeNode.types
        |> Seq.exists (fun constituent -> constituent.kind = Ts.SyntaxKind.TypeQuery)

    // An interface inheriting every constituent keeps their overloads, flattening the members
    // into one type does not, so it is only worth it when a constituent can't be inherited
    let everyConstituentIsAReference =
        intersectionTypeNode.types
        |> Seq.forall (fun constituent -> constituent.kind = Ts.SyntaxKind.TypeReference)

    // We can't create a contract for some of the properties
    // they would eiher end-up in a infinite loop or they are don't
    // have a equivalent in F#
    let hasUnsupportedProperties =
        intersectsATypeQuery
        || properties
           |> List.exists (fun property ->
               match property with
               | ForceAny -> true
               | WithoutDeclaration _ -> false
               | Single(_, declaration) ->
                   everyConstituentIsAReference
                   && declaration.kind = Ts.SyntaxKind.MethodDeclaration
           )

    // `IRouterHandler<T> & ((...handlers: Handler[]) => T)`: the intersection is callable
    let callSignatures =
        if unionOrIntersectionType.isUnion () then
            []
        else
            checker.getSignaturesOfType (unionOrIntersectionType, Ts.SignatureKind.Call)
            |> Seq.toList
            |> List.choose (fun signature ->
                // The signature of `IRouterHandler<this>` is the declared one with `T` substituted
                let flags = typeNodeBuilderFlags

                let synthesized: Ts.Node option =
                    checker?signatureToSignatureDeclaration (
                        signature,
                        Ts.SyntaxKind.CallSignature,
                        typeNode,
                        flags
                    )

                match synthesized with
                | Some declaration ->
                    // The type parameters of the signature enclose the callbacks of its
                    // parameters, the intersection encloses the signature
                    declaration?parent <- typeNode
                    let previousContext = reader.SyntheticContext
                    reader.SyntheticContext <- Some declaration

                    try
                        Some(reader.ReadDeclaration(declaration :?> Ts.Declaration))
                    finally
                        reader.SyntheticContext <- previousContext
                | None -> None
            )

    if hasUnsupportedProperties then
        // F# has no intersection, an interface can still inherit each constituent
        let references =
            intersectionTypeNode.types
            |> Seq.toList
            |> List.map (fun constituent ->
                if constituent.kind = Ts.SyntaxKind.TypeReference then
                    reader.ReadTypeNode constituent
                else
                    GlueType.Discard
            )

        let isReference =
            function
            | GlueType.TypeReference _ -> true
            | _ -> false

        let literalMembers =
            intersectionTypeNode.types
            |> Seq.toList
            |> List.collect (fun constituent ->
                if constituent.kind = Ts.SyntaxKind.TypeLiteral then
                    match reader.ReadTypeNode constituent with
                    | GlueType.TypeLiteral typeLiteral -> typeLiteral.Members
                    | _ -> []
                else
                    []
            )

        let inheritable =
            references |> List.filter (fun reference -> not (reference = GlueType.Discard))

        if not inheritable.IsEmpty && inheritable |> List.forall isReference then
            GlueType.IntersectionOfReferences(inheritable, literalMembers)
        else
            GlueType.Primitive GluePrimitive.Any
    else
        let members =
            properties
            |> List.choose (
                function
                | Single(property, declaration) ->
                    Some(readInstantiatedMember reader typeNode property declaration)
                | WithoutDeclaration property ->
                    readPropertyWithoutDeclaration reader typeNode property
                | ForceAny -> failwith "Should not happen here"
            )

        // `getProperties` leaves the index signatures out, the type literals declare them
        let indexSignatures =
            intersectionTypeNode.types
            |> Seq.toList
            |> List.collect (fun constituent ->
                if constituent.kind = Ts.SyntaxKind.TypeLiteral then
                    (constituent :?> Ts.TypeLiteralNode).members
                    |> Seq.toList
                    |> List.filter (fun element -> element.kind = Ts.SyntaxKind.IndexSignature)
                    |> List.map reader.ReadDeclaration
                else
                    []
            )

        GlueType.IntersectionType(members @ callSignatures @ indexSignatures)

/// `DateArg<Date> & {}` is `DateArg<Date>`, the empty type literal only bars `null`
let private readIntersectionType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let isEmptyTypeLiteral (constituent: Ts.TypeNode) =
        constituent.kind = Ts.SyntaxKind.TypeLiteral
        && (constituent :?> Ts.TypeLiteralNode).members.Count = 0

    let constituents =
        (typeNode :?> Ts.IntersectionTypeNode).types
        |> Seq.filter (not << isEmptyTypeLiteral)
        |> Seq.toList

    match constituents with
    | [ single ] -> reader.ReadTypeNode single
    | _ -> readIntersectionConstituents reader typeNode

let private readExpressionWithTypeArguments
    (reader: ITypeScriptReader)
    (typeNode: Ts.TypeNode)
    : GlueType
    =
    let checker = reader.checker

    let expression = typeNode :?> Ts.ExpressionWithTypeArguments

    let typ = checker.getTypeFromTypeNode expression

    // Getting the type from the expression seems more robust for getting a Symbol resolved
    // than using:
    //
    // let symbolOpt = checker.getSymbolAtLocation (expression.expression)
    let symbolOpt =
        // Alias symbol give us better result for utility types like Omit, Partial, etc...
        typ.aliasSymbol
        // If not available, we fallback to the symbol of the type
        |> Option.orElse (
            if isNull (box typ.symbol) then
                None
            else
                Some typ.symbol
        )
        // An erroneous type (`Uint8Array<T>` with an older lib) has no symbol, its name has
        |> Option.orElse (checker.getSymbolAtLocation expression.expression)

    // Specialize the utility types we know how to resolve so they work in
    // heritage clauses too (e.g. `interface Y extends Omit<X, "a">`). The
    // node is structurally compatible with a `TypeReferenceNode` for the
    // properties the reader needs (`typeArguments`).
    let isFromEs5 = isFromEs5Lib symbolOpt

    match isFromEs5, getFullNameOrEmpty checker expression.expression with
    | true, "Omit" -> UtilityType.readOmit reader (unbox<Ts.TypeReferenceNode> expression)
    | true, "Pick" -> UtilityType.readPick reader (unbox<Ts.TypeReferenceNode> expression)
    | _ ->
        let isQualified =
            expression.expression.kind = Ts.SyntaxKind.PropertyAccessExpression

        // The module path is computed from the resolved symbol, so the name must be its name too
        let name =
            match symbolOpt with
            // `export default class DatasetController` is the `default` symbol
            | Some symbol when symbol.name = "default" ->
                declaredName symbol |> Option.defaultValue symbol.name
            | Some symbol when not isFromEs5 -> symbol.name
            | _ ->
                if isQualified then
                    (unbox<Ts.PropertyAccessExpression> expression.expression).name?text
                else
                    expression.expression.getText ()

        let isExternal = isExternalToPackages checker reader.PackageContext symbolOpt

        // `extends ReturnType<...>` resolves to an anonymous type, it has no declaration to inherit
        let isAnonymous =
            match symbolOpt with
            | Some symbol -> symbol.name = "__type" || symbol.name = "__object"
            | None -> false

        // An external base type can't be inherited, `inherit obj` is invalid, `Partial` is
        // expanded by the transform
        if
            isAnonymous
            || (isExternal
                && not (knownExternalTypeNames.Contains name)
                && not (isFromEs5 && name = "Partial"))
        then
            GlueType.Discard
        else
            ({
                Name =
                    if name.Contains "." then
                        name
                    else
                        Naming.sanitizeTypeName name
                // The name of the declaration, not of an import alias
                FullName =
                    match symbolOpt |> Option.bind (resolveAlias checker) with
                    | Some symbol -> checker.getFullyQualifiedName symbol
                    | None -> getFullNameOrEmpty checker expression.expression
                ModulePath =
                    if isLibraryName reader name then
                        []
                    else
                        modulePathForSymbol checker reader.PackageContext isQualified symbolOpt
                TypeArguments =
                    match readTypeArguments reader expression with
                    | [] ->
                        resolvedBaseTypeArguments checker expression
                        |> List.map (fun argument ->
                            let flags = typeNodeBuilderFlags

                            checker.typeToTypeNode (argument, None, Some flags)
                            |> reader.ReadTypeNode
                        )
                    | typeArguments -> typeArguments
                IsStandardLibrary = isFromEs5 || isExternal || isLibraryName reader name
            })
            |> GlueType.TypeReference

let private readConditionalType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let conditionalTypeNode = typeNode :?> Ts.ConditionalTypeNode

    let typ = checker.getTypeAtLocation conditionalTypeNode

    // The branch depends on the type arguments: resolved with the defaults of the enclosing
    // declaration, else kept for the transform
    if isDeferredConditional typ then
        // `JSHandle<T = any>`: `T extends Node ? ElementHandle<T> : null` is both branches
        let bothBranches () =
            let branch (node: Ts.TypeNode) =
                match reader.ReadTypeNode node with
                | GlueType.Literal GlueLiteral.Null -> GlueType.Primitive GluePrimitive.Null
                | glueType -> glueType

            let trueType = branch conditionalTypeNode.trueType
            let falseType = branch conditionalTypeNode.falseType

            if trueType = falseType then
                trueType
            else
                GlueType.Union(GlueTypeUnion [ trueType; falseType ])

        let resolved =
            if isAnyTypeParameter reader conditionalTypeNode.checkType then
                Some(bothBranches ())
            else
                tryResolveDeferred reader typeNode

        match resolved with
        | Some resolved -> resolved
        | None ->

            let warningsCount = reader.Warnings.Count

            let inferred =
                inferTypeNodes conditionalTypeNode.extendsType
                |> List.map (fun inferTypeNode ->
                    ({
                        Name = inferTypeNode.typeParameter.name.getText ()
                        Constraint = Some(inferredConstraint reader inferTypeNode)
                        Default = None
                    }
                    : GlueTypeParameter)
                )
                |> List.distinctBy _.Name

            let conditionalType =
                ({
                    CheckType = reader.ReadTypeNode conditionalTypeNode.checkType
                    ExtendsType = reader.ReadTypeNode conditionalTypeNode.extendsType
                    TrueType = reader.ReadTypeNode conditionalTypeNode.trueType
                    FalseType = reader.ReadTypeNode conditionalTypeNode.falseType
                    Inferred = inferred
                }
                : GlueConditionalType)

            // A branch with `infer` is not read, without a warning
            if reader.Warnings.Count > warningsCount then
                reader.Warnings.RemoveRange(warningsCount, reader.Warnings.Count - warningsCount)
                GlueType.Primitive GluePrimitive.Any
            else
                GlueType.ConditionalType conditionalType
    else

        // If we resolved the type to Any, we fallback to the generic type
        // This is because in F#, we can write
        // type ReturnType<'T> = obj
        // because 'T is not used in the type
        // This is perhaps a bit aggressive, so if needed we can re-visit `readTypeUsingFlags`
        // usage by inlining the logic here and make it more specific
        match typ.flags with
        // `Flag extends true ? Value : Fallback` is its branch
        | HasTypeFlags Ts.TypeFlags.TypeParameter -> GlueType.TypeParameter typ.symbol.name
        | HasTypeFlags Ts.TypeFlags.BooleanLiteral ->
            GlueType.Literal(GlueLiteral.Bool(typ?intrinsicName = "true"))
        | _ ->
            match readTypeUsingFlags reader typ with
            | GlueType.Primitive GluePrimitive.Any ->
                reader.ReadTypeNode conditionalTypeNode.checkType
            | forward -> forward

let private readTemplateLiteralType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let templateLiteralTypeNode = typeNode :?> Ts.TemplateLiteralTypeNode

    // Ask the type checker to resolve the template literal type.
    // When the template is made of finite parts (e.g. unions of string
    // literals), TypeScript expands it into a union of string literals
    // that we can represent as a StringEnum.
    // Otherwise (e.g. `section-${string}`) the type stays a template
    // literal / string and we fallback to `string`.
    let typ = checker.getTypeAtLocation templateLiteralTypeNode

    let rec readResolvedLiterals (typ: Ts.Type) : GlueType list =
        match typ.flags with
        | HasTypeFlags Ts.TypeFlags.StringLiteral ->
            match typ with
            | Type.StringLiteral.String value -> [ GlueLiteral.String value |> GlueType.Literal ]
            | Type.StringLiteral.Other -> []
        | HasTypeFlags Ts.TypeFlags.Union ->
            (typ :?> Ts.UnionType).types |> Seq.toList |> List.collect readResolvedLiterals
        | _ -> []

    match readResolvedLiterals typ with
    // The type checker couldn't resolve the template to a finite set of
    // string literals, so we fallback to a plain `string`.
    | [] -> GlueType.TemplateLiteral
    | [ single ] -> single
    | cases -> cases |> GlueTypeUnion |> GlueType.Union

let readTypeNode (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    match typeNode.kind with
    | Ts.SyntaxKind.NumberKeyword -> GlueType.Primitive GluePrimitive.Number
    | Ts.SyntaxKind.StringKeyword -> GlueType.Primitive GluePrimitive.String
    | Ts.SyntaxKind.VoidKeyword -> GlueType.Primitive GluePrimitive.Unit
    | Ts.SyntaxKind.BooleanKeyword -> GlueType.Primitive GluePrimitive.Bool
    | Ts.SyntaxKind.AnyKeyword -> GlueType.Primitive GluePrimitive.Any
    | Ts.SyntaxKind.NullKeyword -> GlueType.Primitive GluePrimitive.Null
    | Ts.SyntaxKind.UndefinedKeyword -> GlueType.Primitive GluePrimitive.Undefined
    | Ts.SyntaxKind.UnionType -> reader.ReadUnionTypeNode(typeNode :?> Ts.UnionTypeNode)

    | Ts.SyntaxKind.TypeReference -> readTypeReference reader typeNode

    | Ts.SyntaxKind.ArrayType ->
        let arrayTypeNode = typeNode :?> Ts.ArrayTypeNode

        let elementType = reader.ReadTypeNode arrayTypeNode.elementType

        GlueType.Array elementType

    | Ts.SyntaxKind.TypePredicate ->
        // `asserts x is T` returns nothing, it throws
        match (typeNode :?> Ts.TypePredicateNode).assertsModifier with
        | Some _ -> GlueType.Primitive GluePrimitive.Unit
        | None -> GlueType.Primitive GluePrimitive.Bool

    | Ts.SyntaxKind.FunctionType -> readFunctionType reader typeNode

    | Ts.SyntaxKind.TypeQuery ->
        let typeQueryNode = typeNode :?> Ts.TypeQueryNode
        TypeQueryNode.readTypeQueryNode reader typeQueryNode

    | Ts.SyntaxKind.MappedType -> reader.ReadMappedTypeNode(typeNode :?> Ts.MappedTypeNode)

    | Ts.SyntaxKind.ImportType -> readImportType reader typeNode

    | Ts.SyntaxKind.LiteralType ->
        let literalTypeNode = typeNode :?> Ts.LiteralTypeNode

        let literalExpression = unbox<Ts.LiteralExpression> literalTypeNode.literal

        match tryReadLiteral checker literalExpression with
        | Some literal -> GlueType.Literal literal
        | None ->
            Report.readerError (
                "type node - literal type",
                $"Could not read literal type",
                typeNode
            )
            |> failwith

    | Ts.SyntaxKind.ThisType -> readThisType reader typeNode

    | Ts.SyntaxKind.TupleType ->
        let tupleTypeNode = typeNode :?> Ts.TupleTypeNode

        let elements = tupleTypeNode.elements |> Seq.toList |> List.map unbox<Ts.TypeNode>

        // `[number, ...T, string]` has no fixed length, it is an array
        if elements |> List.exists (fun element -> element.kind = Ts.SyntaxKind.RestType) then
            let elementTypes =
                elements
                |> List.map (fun element ->
                    if element.kind = Ts.SyntaxKind.RestType then
                        match reader.ReadTypeNode (element :?> Ts.RestTypeNode).``type`` with
                        | GlueType.Array elementType -> elementType
                        | _ -> GlueType.Primitive GluePrimitive.Any
                    else
                        reader.ReadTypeNode element
                )
                |> List.distinct

            match elementTypes with
            | [ elementType ] -> GlueType.Array elementType
            | _ -> GlueType.Array(GlueType.Primitive GluePrimitive.Any)
        else
            elements |> List.map reader.ReadTypeNode |> GlueType.TupleType

    | Ts.SyntaxKind.RestType -> reader.ReadTypeNode (typeNode :?> Ts.RestTypeNode).``type``

    // `class ProgressEvent { __proto__: Event & ProgressEvent }` reads itself forever
    | Ts.SyntaxKind.IntersectionType when
        reader.InProgress.Intersections
        |> Seq.exists (fun inProgress ->
            obj.ReferenceEquals(inProgress, checker.getTypeAtLocation typeNode)
        )
        ->
        GlueType.Primitive GluePrimitive.Any

    // `Window & typeof globalThis`: the globals add nothing to the type
    | Ts.SyntaxKind.IntersectionType when
        (typeNode :?> Ts.IntersectionTypeNode).types |> Seq.exists isGlobalThisQuery
        ->
        match
            (typeNode :?> Ts.IntersectionTypeNode).types
            |> Seq.filter (not << isGlobalThisQuery)
            |> Seq.toList
        with
        | [ single ] -> reader.ReadTypeNode single
        | _ -> GlueType.Primitive GluePrimitive.Any

    | Ts.SyntaxKind.IntersectionType -> readIntersectionType reader typeNode

    | Ts.SyntaxKind.TypeLiteral ->
        let typeLiteralNode = typeNode :?> Ts.TypeLiteralNode

        let members =
            typeLiteralNode.members |> Seq.toList |> List.map reader.ReadDeclaration

        ({
            Members = members
            Id = typeLiteralId typeLiteralNode
        }
        : GlueTypeLiteral)
        |> GlueType.TypeLiteral

    | Ts.SyntaxKind.ParenthesizedType ->
        let parenthesizedTypeNode = typeNode :?> Ts.ParenthesizedTypeNode

        reader.ReadTypeNode parenthesizedTypeNode.``type``

    | Ts.SyntaxKind.OptionalType ->
        let optionalTypeNode = typeNode :?> Ts.OptionalTypeNode

        reader.ReadTypeNode optionalTypeNode.``type`` |> GlueType.OptionalType

    | Ts.SyntaxKind.TypeOperator ->
        let typeOperatorNode = typeNode :?> Ts.TypeOperatorNode

        reader.ReadTypeOperatorNode typeOperatorNode

    | Ts.SyntaxKind.UnknownKeyword -> GlueType.Unknown

    | Ts.SyntaxKind.ObjectKeyword -> GlueType.Primitive GluePrimitive.Object

    | Ts.SyntaxKind.NamedTupleMember ->
        reader.ReadNamedTupleMember(typeNode :?> Ts.NamedTupleMember)

    | Ts.SyntaxKind.SymbolKeyword -> GlueType.Primitive GluePrimitive.Symbol

    | Ts.SyntaxKind.BigIntKeyword -> GlueType.Primitive GluePrimitive.BigInt

    | Ts.SyntaxKind.ExpressionWithTypeArguments -> readExpressionWithTypeArguments reader typeNode

    // `infer T` names a type parameter of the conditional, the transform binds it
    | Ts.SyntaxKind.InferType ->
        (typeNode :?> Ts.InferTypeNode).typeParameter.name.getText()
        |> GlueType.TypeParameter

    | Ts.SyntaxKind.ConditionalType -> readConditionalType reader typeNode

    | Ts.SyntaxKind.TemplateLiteralType -> readTemplateLiteralType reader typeNode

    | Ts.SyntaxKind.IndexedAccessType ->
        let indexedAccessType = typeNode :?> Ts.IndexedAccessType
        reader.ReadIndexedAccessType indexedAccessType

    | Ts.SyntaxKind.ConstructorType ->
        let constructorTypeNode = typeNode :?> Ts.ConstructorTypeNode

        ({
            Parameters = reader.ReadParameters constructorTypeNode.parameters
            Type = reader.ReadTypeNode constructorTypeNode.``type``
        }
        : GlueConstructSignature)
        |> GlueType.ConstructorType

    | Ts.SyntaxKind.NeverKeyword -> GlueType.Primitive GluePrimitive.Never

    | _ ->
        Report.readerError ("type node", $"Unsupported kind %s{typeNode.kind.Name}", typeNode)
        |> reader.Warnings.Add

        GlueType.Primitive GluePrimitive.Any
