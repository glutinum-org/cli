module Glutinum.Converter.Reader.Deferred

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

/// A conditional type the checker can't resolve without its type arguments
let isDeferredConditional (typ: Ts.Type) =
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
let isAnyTypeParameter (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) =
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

/// An answer still naming one of the bound types stands for the binding, not the reference
let private isStillBound
    (checker: Ts.TypeChecker)
    (bound: Map<string, Ts.TypeNode>)
    (typ: Ts.Type)
    =
    // `never` or `string` in the answer says nothing, a named type does
    let substitutes =
        bound.Values
        |> Seq.filter (fun node -> node.kind = Ts.SyntaxKind.TypeReference)
        |> Seq.map checker.getTypeFromTypeNode
        |> Seq.toList

    let mentioned = mentionedTypes checker 0 typ

    substitutes
    |> List.exists (fun substitute ->
        mentioned |> List.exists (fun typ -> obj.ReferenceEquals(typ, substitute))
    )

/// The checker asked again with the type parameters the deferred conditionals name bound, until
/// nothing is deferred or nothing new binds
let rec private resolveWithBindings
    (checker: Ts.TypeChecker)
    (typeNode: Ts.TypeNode)
    (bindings: Map<string, Ts.TypeNode>)
    (bound: Map<string, Ts.TypeNode>)
    (typ: Ts.Type)
    : Ts.Type option
    =
    let conditionals = deferredConditionals checker 0 typ

    if conditionals.IsEmpty then
        if isStillBound checker bound typ then
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
                    | Some binding when not (bound.ContainsKey name) -> bound.Add(name, binding)
                    | _ -> bound
                )
            )

        // A synthesized conditional gets a new root each time, the loop ends on the bindings
        if bound.Count = alreadyBound || not (mentionsBoundTypeParameter bound typeNode) then
            None
        else
            checker.getTypeFromTypeNode (substituteBoundTypeParameters bound typeNode)
            |> resolveWithBindings checker typeNode bindings bound

/// A conditional node, or a reference to an alias whose body is one, alias chains followed
let rec private standsForConditional
    (checker: Ts.TypeChecker)
    (visited: Set<string>)
    (node: Ts.Node)
    : bool
    =
    match node.kind with
    | Ts.SyntaxKind.ConditionalType -> true
    | Ts.SyntaxKind.ParenthesizedType ->
        standsForConditional checker visited (node :?> Ts.ParenthesizedTypeNode).``type``
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
                            checker
                            (visited.Add key)
                            (declaration :?> Ts.TypeAliasDeclaration).``type``
                    )
                )
                |> Option.defaultValue false)
        | None -> false
    | _ -> false

/// Inside the constraint of a type parameter, a default is not a constraint
let rec private isWithinConstraint (node: Ts.Node) =
    if isNull node || isNull node.parent then
        false
    elif node.parent.kind = Ts.SyntaxKind.TypeParameter then
        match (node.parent :?> Ts.TypeParameterDeclaration).``constraint`` with
        | Some constraint_ -> obj.ReferenceEquals(constraint_, node)
        | None -> false
    else
        isWithinConstraint node.parent

/// `on<K>(event: Key<K, T>)` of `EventEmitter<T = DefaultEventMap>`: the checker defers
/// `Key<K, T>`, it resolves `Key<K, DefaultEventMap>`. The type parameters the condition names
/// are bound to their default or constraint, the ones the branches name only stay generic, and
/// the checker is asked again. A condition revealed by a branch is bound in turn.
let tryResolveDeferred (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType option =
    let checker = reader.checker
    let bindings = boundTypeParameters reader typeNode

    // The order of the checker's answers shapes its unions, it is asked about conditionals only
    if
        not (isParseTreeNode typeNode)
        || not (standsForConditional checker Set.empty typeNode)
        || isWithinConstraint typeNode
        || not (mentionsBoundTypeParameter bindings typeNode)
    then
        None
    else
        let typ = checker.getTypeAtLocation typeNode

        if (deferredConditionals checker 0 typ).IsEmpty then
            None
        else
            resolveWithBindings checker typeNode bindings Map.empty typ
            |> Option.bind (fun resolved ->
                checker.typeToTypeNode (
                    resolved,
                    enclosingDeclarationOf typeNode,
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

let rec inferTypeNodes (node: Ts.Node) : Ts.InferTypeNode list =
    [
        if node.kind = Ts.SyntaxKind.InferType then
            node :?> Ts.InferTypeNode

        for child in node.getChildren () do
            yield! inferTypeNodes child
    ]

/// `Options<infer DateType>` stands for the constraint declared at its position, `Date` for
/// `interface Options<DateType extends Date>`
let inferredConstraint (reader: ITypeScriptReader) (inferTypeNode: Ts.InferTypeNode) : GlueType =
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

let truncateToDeclaredArity
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

let isGlobalThisQuery (typeNode: Ts.TypeNode) =
    typeNode.kind = Ts.SyntaxKind.TypeQuery
    && entityNameText !!(typeNode :?> Ts.TypeQueryNode).exprName = "globalThis"

let readTypeUsingFlags (reader: ITypeScriptReader) (typ: Ts.Type) =

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
