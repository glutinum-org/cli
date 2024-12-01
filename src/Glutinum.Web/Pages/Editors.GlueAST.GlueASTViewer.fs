module Glutinum.Web.Pages.Editors.GlueAST.GlueASTViewer

open Glutinum.Converter.GlueAST
open Glutinum.Web.Components.AstViewer
open Feliz
open Elmish

module GlueAccessor =

    let toText (accessor: GlueAccessor) =
        match accessor with
        | GlueAccessor.ReadOnly -> "ReadOnly"
        | GlueAccessor.WriteOnly -> "WriteOnly"
        | GlueAccessor.ReadWrite -> "ReadWrite"

type GlueASTViewer =

    static member private Name(name: string) =
        ASTViewer.renderKeyValue "Name" name

    static member private FullName(fullName: string) =
        ASTViewer.renderKeyValue "FullName" fullName

    static member private IsStandardLibrary(isStandardLibrary: bool) =
        ASTViewer.renderKeyValue "IsStandardLibrary" (string isStandardLibrary)

    static member private IsOptional(isOptional: bool) =
        ASTViewer.renderKeyValue "IsOptional" (string isOptional)

    static member private IsReadOnly(isReadOnly: bool) =
        ASTViewer.renderKeyValue "IsReadOnly" (string isReadOnly)

    static member private IsStatic(isStatic: bool) =
        ASTViewer.renderKeyValue "IsStatic" (string isStatic)

    static member private IsSpread(isSpread: bool) =
        ASTViewer.renderKeyValue "IsSpread" (string isSpread)

    static member private IsPrivate(isPrivate: bool) =
        ASTViewer.renderKeyValue "IsPrivate" (string isPrivate)

    static member private Parameters(parameters: GlueParameter list) =
        parameters
        |> List.map (fun parameter ->
            ASTViewer.renderNode "Parameter" [
                GlueASTViewer.Name parameter.Name
                GlueASTViewer.IsOptional parameter.IsOptional
                GlueASTViewer.IsSpread parameter.IsSpread
                GlueASTViewer.Type parameter.Type
            ]
        )
        |> ASTViewer.renderNode "Parameters"

    static member private Type(glueType: GlueType) =
        ASTViewer.renderNode "Type" [ GlueASTViewer.GlueType glueType ]

    static member private HeritageClauses(heritageClauses: GlueType list) =
        heritageClauses
        |> List.map (fun heritageClause ->
            ASTViewer.renderNode "HeritageClause" [
                GlueASTViewer.GlueType heritageClause
            ]
        )
        |> ASTViewer.renderNode "HeritageClauses"

    static member private TypeParameters
        (typeParameters: GlueTypeParameter list)
        =
        typeParameters
        |> List.map (fun typeParameter ->
            ASTViewer.renderNode "TypeParameter" [
                GlueASTViewer.Name typeParameter.Name

                ASTViewer.renderNodeOption
                    "Constraint"
                    GlueASTViewer.GlueType
                    typeParameter.Constraint

                ASTViewer.renderNodeOption
                    "Default"
                    GlueASTViewer.GlueType
                    typeParameter.Default
            ]
        )
        |> ASTViewer.renderNode "TypeParameters"

    static member private Documentation(documentation: GlueComment list) =
        documentation
        |> List.map (fun comment ->
            match comment with
            | GlueComment.Summary lines ->
                lines
                |> List.map (fun line -> ASTViewer.renderValueOnly line)
                |> ASTViewer.renderNode "Summary"

            | GlueComment.Returns content ->
                [ ASTViewer.renderValueOnly content ]
                |> ASTViewer.renderNode "Returns"

            | GlueComment.DefaultValue value ->
                ASTViewer.renderKeyValue "DefaultValue" value

            | GlueComment.Param param ->
                ASTViewer.renderNode "Param" [
                    GlueASTViewer.Name param.Name
                    ASTViewer.renderKeyValueOption "Content" id param.Content
                ]

            | GlueComment.Remarks content ->
                [ ASTViewer.renderValueOnly content ]
                |> ASTViewer.renderNode "Remarks"

            | GlueComment.Example content ->
                [ ASTViewer.renderValueOnly content ]
                |> ASTViewer.renderNode "Example"

            | GlueComment.Deprecated content ->
                ASTViewer.renderKeyValueOption "Deprecated" id content

            | GlueComment.TypeParam typeParam ->
                ASTViewer.renderNode "TypeParam" [
                    ASTViewer.renderKeyValue "TypeName" typeParam.TypeName
                    ASTViewer.renderKeyValueOption
                        "Content"
                        id
                        typeParam.Content
                ]

            | GlueComment.Throws content ->
                [ ASTViewer.renderValueOnly content ]
                |> ASTViewer.renderNode "Throws"
        )
        |> ASTViewer.renderNode "Documentation"

    static member private Constructors(constructors: GlueConstructor list) =
        constructors
        |> List.map (fun constructorInfo ->
            ASTViewer.renderNode "Constructor" [
                GlueASTViewer.Documentation constructorInfo.Documentation
                GlueASTViewer.Parameters constructorInfo.Parameters
            ]
        )
        |> ASTViewer.renderNode "Constructors"

    static member private GlueMember(glueMember: GlueMember) =
        match glueMember with
        | GlueMember.MethodSignature methodSignature ->
            ASTViewer.renderNode "MethodSignature" [
                GlueASTViewer.Name methodSignature.Name
                GlueASTViewer.Documentation methodSignature.Documentation
                GlueASTViewer.Parameters methodSignature.Parameters
                GlueASTViewer.Type methodSignature.Type
            ]

        | GlueMember.CallSignature callSignature ->
            ASTViewer.renderNode "CallSignature" [
                GlueASTViewer.Parameters callSignature.Parameters
                GlueASTViewer.Type callSignature.Type
            ]

        | GlueMember.Method methodInfo ->
            ASTViewer.renderNode "Method" [
                GlueASTViewer.Name methodInfo.Name
                GlueASTViewer.IsOptional methodInfo.IsOptional
                GlueASTViewer.IsStatic methodInfo.IsStatic
                GlueASTViewer.Parameters methodInfo.Parameters
                GlueASTViewer.Type methodInfo.Type
            ]

        | GlueMember.IndexSignature indexSignature ->
            ASTViewer.renderNode "IndexSignature" [
                GlueASTViewer.Parameters indexSignature.Parameters
                GlueASTViewer.Type indexSignature.Type
                GlueASTViewer.IsReadOnly indexSignature.IsReadOnly
            ]

        | GlueMember.Property propertyInfo ->
            ASTViewer.renderNode "Property" [
                GlueASTViewer.Name propertyInfo.Name
                GlueASTViewer.Documentation propertyInfo.Documentation
                GlueASTViewer.IsStatic propertyInfo.IsStatic
                GlueASTViewer.IsOptional propertyInfo.IsOptional
                GlueASTViewer.IsPrivate propertyInfo.IsPrivate
                ASTViewer.renderKeyValue
                    "Accessor"
                    (GlueAccessor.toText propertyInfo.Accessor)
                GlueASTViewer.Type propertyInfo.Type
            ]

        | GlueMember.ConstructSignature constructSignature ->
            ASTViewer.renderNode "ConstructSignature" [
                GlueASTViewer.Parameters constructSignature.Parameters
                GlueASTViewer.Type constructSignature.Type
            ]

        | GlueMember.GetAccessor getAccessorInfo ->
            ASTViewer.renderNode "GetAccessor" [
                GlueASTViewer.Name getAccessorInfo.Name
                GlueASTViewer.Documentation getAccessorInfo.Documentation
                GlueASTViewer.IsStatic getAccessorInfo.IsStatic
                GlueASTViewer.IsPrivate getAccessorInfo.IsPrivate
                GlueASTViewer.Type getAccessorInfo.Type
            ]

        | GlueMember.SetAccessor setAccessorInfo ->
            ASTViewer.renderNode "SetAccessor" [
                GlueASTViewer.Name setAccessorInfo.Name
                GlueASTViewer.Documentation setAccessorInfo.Documentation
                GlueASTViewer.IsStatic setAccessorInfo.IsStatic
                GlueASTViewer.IsPrivate setAccessorInfo.IsPrivate
                ASTViewer.renderNode "ArgumentType" [
                    GlueASTViewer.GlueType setAccessorInfo.ArgumentType
                ]
            ]

    static member private Members(members: GlueMember list) =
        members
        |> List.map GlueASTViewer.GlueMember
        |> ASTViewer.renderNode "Members"

    static member private GlueLiteral(glueLiteral: GlueLiteral) =
        match glueLiteral with
        | GlueLiteral.String value ->
            ASTViewer.renderNode "String" [
                ASTViewer.renderKeyValue "Value" value
            ]
        | GlueLiteral.Bool value ->
            ASTViewer.renderNode "Bool" [
                ASTViewer.renderKeyValue "Value" (string value)
            ]
        | GlueLiteral.Float value ->
            ASTViewer.renderNode "Float" [
                ASTViewer.renderKeyValue "Value" (string value)
            ]
        | GlueLiteral.Int value ->
            ASTViewer.renderNode "Int" [
                ASTViewer.renderKeyValue "Value" (string value)
            ]

        | GlueLiteral.Null -> ASTViewer.renderNode "Null" []

    static member private GlueType
        (glueType: GlueType)
        (context: NodeContext<'Msg>)
        =
        match glueType with
        | GlueType.ExportDefault glueType ->
            ASTViewer.renderNode
                "ExportDefault"
                [ GlueASTViewer.GlueType glueType ]
                context

        | GlueType.Unknown -> ASTViewer.renderValueOnly "Unknown" context

        | GlueType.TemplateLiteral ->
            ASTViewer.renderValueOnly "TemplateLiteral" context

        | GlueType.ReadOnly readOnlyType ->
            ASTViewer.renderNode
                "ReadOnly"
                [ GlueASTViewer.Type readOnlyType ]
                context

        | GlueType.Variable variableInfo ->
            ASTViewer.renderNode
                "Variable"
                [
                    GlueASTViewer.Name variableInfo.Name
                    GlueASTViewer.Documentation variableInfo.Documentation
                    GlueASTViewer.Type variableInfo.Type
                ]
                context

        | GlueType.OptionalType optionalType ->
            ASTViewer.renderNode
                "OptionalType"
                [ GlueASTViewer.Type optionalType ]
                context

        | GlueType.Interface interfaceInfo ->
            ASTViewer.renderNode
                "Interface"
                [
                    GlueASTViewer.Name interfaceInfo.Name
                    GlueASTViewer.FullName interfaceInfo.FullName
                    GlueASTViewer.TypeParameters interfaceInfo.TypeParameters
                    GlueASTViewer.Members interfaceInfo.Members
                    GlueASTViewer.HeritageClauses interfaceInfo.HeritageClauses
                ]
                context

        | GlueType.Array arrayInfo ->
            ASTViewer.renderNode
                "Array"
                [ GlueASTViewer.Name arrayInfo.Name ]
                context

        | GlueType.Discard -> ASTViewer.renderNode "Discard" [] context

        | GlueType.NamedTupleType namedTupleType ->
            ASTViewer.renderNode
                "NamedTupleType"
                [
                    GlueASTViewer.Name namedTupleType.Name
                    GlueASTViewer.Type namedTupleType.Type
                ]
                context

        | GlueType.Enum enumInfo ->
            ASTViewer.renderNode
                "Enum"
                [
                    GlueASTViewer.Name enumInfo.Name
                    enumInfo.Members
                    |> List.map (fun memberInfo ->
                        [
                            GlueASTViewer.Name memberInfo.Name

                            memberInfo.Value
                            |> GlueASTViewer.GlueLiteral
                            |> List.singleton
                            |> ASTViewer.renderNode "Value"
                        ]
                        |> ASTViewer.renderNode "Member"
                    )
                    |> ASTViewer.renderNode "Members"
                ]
                context

        | GlueType.Primitive primitiveInfo ->
            let content =
                match primitiveInfo with
                | GluePrimitive.String -> Html.span "String"
                | GluePrimitive.Number -> Html.span "Number"
                | GluePrimitive.Int -> Html.span "Int"
                | GluePrimitive.Float -> Html.span "Float"
                | GluePrimitive.Bool -> Html.span "Bool"
                | GluePrimitive.Unit -> Html.span "Unit"
                | GluePrimitive.Any -> Html.span "Any"
                | GluePrimitive.Null -> Html.span "Null"
                | GluePrimitive.Undefined -> Html.span "Undefined"
                | GluePrimitive.Object -> Html.span "Object"
                | GluePrimitive.Symbol -> Html.span "Symbol"
                | GluePrimitive.Never -> Html.span "Never"

            ASTViewer.renderNode
                "Primitive"
                [
                    // Discard the context
                    fun _ -> content
                ]
                context

        | GlueType.TypeAliasDeclaration typeAliasInfo ->
            ASTViewer.renderNode
                "TypeAliasDeclaration"
                [
                    GlueASTViewer.Name typeAliasInfo.Name
                    GlueASTViewer.Documentation typeAliasInfo.Documentation
                    GlueASTViewer.Type typeAliasInfo.Type
                    GlueASTViewer.TypeParameters typeAliasInfo.TypeParameters
                ]
                context

        | GlueType.Union(GlueTypeUnion unionTypes) ->
            ASTViewer.renderNode
                "Union"
                (unionTypes |> List.map GlueASTViewer.GlueType)
                context

        | GlueType.Literal literalInfo ->
            ASTViewer.renderNode
                "Literal"
                [ GlueASTViewer.GlueLiteral literalInfo ]
                context

        | GlueType.ClassDeclaration classDeclaration ->
            ASTViewer.renderNode
                "ClassDeclaration"
                [
                    GlueASTViewer.Name classDeclaration.Name
                    GlueASTViewer.Constructors classDeclaration.Constructors
                    GlueASTViewer.TypeParameters classDeclaration.TypeParameters
                    classDeclaration.Members
                    |> List.map (fun glueMember ->
                        GlueASTViewer.GlueMember glueMember
                        |> List.singleton
                        |> ASTViewer.renderNode "Member"
                    )
                    |> ASTViewer.renderNode "Members"
                    GlueASTViewer.HeritageClauses
                        classDeclaration.HeritageClauses
                ]
                context

        | GlueType.FunctionDeclaration functionDeclaration ->
            ASTViewer.renderNode
                "FunctionDeclaration"
                [
                    GlueASTViewer.Name functionDeclaration.Name
                    GlueASTViewer.Documentation
                        functionDeclaration.Documentation
                    ASTViewer.renderKeyValue
                        "IsDeclared"
                        (string functionDeclaration.IsDeclared)
                    GlueASTViewer.Parameters functionDeclaration.Parameters
                    GlueASTViewer.Type functionDeclaration.Type
                    GlueASTViewer.TypeParameters
                        functionDeclaration.TypeParameters
                ]
                context

        | GlueType.FunctionType functionType ->
            ASTViewer.renderNode
                "FunctionType"
                [
                    GlueASTViewer.Documentation functionType.Documentation
                    GlueASTViewer.TypeParameters functionType.TypeParameters
                    GlueASTViewer.Parameters functionType.Parameters
                    GlueASTViewer.Type functionType.Type
                ]
                context

        | GlueType.IndexedAccessType indexedAccessType ->
            ASTViewer.renderNode
                "IndexedAccessType"
                [
                    GlueASTViewer.Type indexedAccessType.IndexType
                    GlueASTViewer.Type indexedAccessType.ObjectType
                ]
                context

        | GlueType.KeyOf keyOf ->
            ASTViewer.renderNode "KeyOf" [ GlueASTViewer.Type keyOf ] context

        | GlueType.ModuleDeclaration moduleDeclaration ->
            ASTViewer.renderNode
                "ModuleDeclaration"
                [
                    GlueASTViewer.Name moduleDeclaration.Name
                    ASTViewer.renderKeyValue
                        "IsNamespace"
                        (string moduleDeclaration.IsNamespace)
                    ASTViewer.renderKeyValue
                        "IsRecursive"
                        (string moduleDeclaration.IsRecursive)
                    moduleDeclaration.Types
                    |> List.map GlueASTViewer.GlueType
                    |> ASTViewer.renderNode "Types"
                ]
                context

        | GlueType.MappedType mappedType ->
            ASTViewer.renderNode
                "MappedType"
                [
                    match mappedType.Type with
                    | Some ty -> GlueASTViewer.Type ty
                    | None -> ()

                    GlueASTViewer.TypeParameters [ mappedType.TypeParameter ]
                ]
                context

        | GlueType.UtilityType utilityType ->
            match utilityType with
            | GlueUtilityType.Partial partialInfo ->
                ASTViewer.renderNode
                    "Partial"
                    [
                        GlueASTViewer.Name partialInfo.Name
                        GlueASTViewer.Members partialInfo.Members
                        GlueASTViewer.TypeParameters partialInfo.TypeParameters
                    ]
                    context

            | GlueUtilityType.Record recordInfo ->
                ASTViewer.renderNode
                    "Record"
                    [
                        ASTViewer.renderNode "KeyType" [
                            GlueASTViewer.GlueType recordInfo.KeyType
                        ]
                        ASTViewer.renderNode "ValueType" [
                            GlueASTViewer.GlueType recordInfo.ValueType
                        ]
                    ]
                    context

            | GlueUtilityType.ReturnType returnType ->
                ASTViewer.renderNode
                    "ReturnType"
                    [ GlueASTViewer.Type returnType ]
                    context

            | GlueUtilityType.ThisParameterType thisParameterType ->
                ASTViewer.renderNode
                    "ThisParameterType"
                    [ GlueASTViewer.Type thisParameterType ]
                    context

            | GlueUtilityType.Omit members ->
                ASTViewer.renderNode
                    "Omit"
                    (members |> List.map GlueASTViewer.GlueMember)
                    context

        | GlueType.ThisType thisTypeInfo ->
            ASTViewer.renderNode
                "ThisType"
                [
                    GlueASTViewer.Name thisTypeInfo.Name
                    GlueASTViewer.TypeParameters thisTypeInfo.TypeParameters
                ]
                context

        | GlueType.TupleType tupleTypes ->
            ASTViewer.renderNode
                "TupleType"
                (tupleTypes |> List.map GlueASTViewer.GlueType)
                context

        | GlueType.TypeParameter typeParameterName ->
            ASTViewer.renderNode
                "TypeParameter"
                [ GlueASTViewer.Name typeParameterName ]
                context

        | GlueType.TypeReference typeReference ->
            ASTViewer.renderNode
                "TypeReference"
                [
                    GlueASTViewer.Name typeReference.Name
                    GlueASTViewer.FullName typeReference.FullName
                    GlueASTViewer.IsStandardLibrary
                        typeReference.IsStandardLibrary

                    typeReference.TypeArguments
                    |> List.map GlueASTViewer.GlueType
                    |> ASTViewer.renderNode "TypeArguments"

                ]
                context

        | GlueType.IntersectionType intersectionType ->
            ASTViewer.renderNode
                "IntersectionType"
                (intersectionType |> List.map GlueASTViewer.GlueMember)
                context

        | GlueType.TypeLiteral typeLiteral ->
            ASTViewer.renderNode
                "TypeLiteral"
                [
                    typeLiteral.Members
                    |> List.map GlueASTViewer.GlueMember
                    |> ASTViewer.renderNode "Members"
                ]
                context

        | GlueType.ConstructorType ->
            ASTViewer.renderNode "ConstructorType" [] context

    static member Render
        (types: GlueType list)
        (dispatch: Dispatch<'Msg>)
        (collapsedNodes: Set<string>)
        (collapseMsg: string -> 'Msg)
        (expandMsg: string -> 'Msg)
        =
        types
        |> List.mapi (fun index glueType ->
            let context =
                {
                    Path = $"[{index}]"
                    CollapsedNodes = collapsedNodes
                    Dispatch = dispatch
                    CollapseMsg = collapseMsg
                    ExpandMsg = expandMsg
                }

            GlueASTViewer.GlueType glueType context
        )
