module Glutinum.Converter.Reader.UtilityType

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils
open Glutinum.Converter.Reader.MemberTypes
open Glutinum.Converter.Reader.Deferred

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
                    Report.readerError ("Exclude", "Expected a string literal", typeReferenceNode)
                    |> reader.Warnings.Add

                    []

            | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                match typ with
                | Type.NumberLiteral.Int value -> [ GlueLiteral.Int value |> GlueType.Literal ]
                | Type.NumberLiteral.Float value -> [ GlueLiteral.Float value |> GlueType.Literal ]
                | Type.NumberLiteral.Other ->
                    Report.readerError ("Exclude", "Expected a number literal", typeReferenceNode)
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

    | HasTypeFlags Ts.TypeFlags.Conditional -> GlueType.Primitive GluePrimitive.Any

    | HasTypeFlags Ts.TypeFlags.Object ->
        match reader.checker.typeToTypeNode (typ, None, Some typeNodeBuilderFlags) with
        | Some typeNode -> reader.ReadTypeNode typeNode
        | None -> GlueType.Primitive GluePrimitive.Any

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

    withInProgress
        reader.InProgress.Landing
        contextNode
        (fun () ->
            match declarations with
            | [] -> readPropertyWithoutDeclaration reader contextNode property |> Option.toList
            | declarations ->
                declarations |> List.map (readInstantiatedMember reader contextNode property)
        )

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

    match reader.checker.typeToTypeNode (typ, None, Some typeNodeBuilderFlags) with
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

let readThisParameterType (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
    let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

    match reader.checker.typeToTypeNode (typ, None, Some typeNodeBuilderFlags) with
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

    // Keys named through a type parameter are known once it is bound
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

    // `Omit<HighlightResult, ...>` inside `HighlightResult` would read without end
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
