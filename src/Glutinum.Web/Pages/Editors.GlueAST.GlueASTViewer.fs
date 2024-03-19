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

    static member private IsOptional(isOptional: bool) =
        ASTViewer.renderKeyValue "IsOptional" (string isOptional)

    static member private IsStatic(isStatic: bool) =
        ASTViewer.renderKeyValue "IsStatic" (string isStatic)

    static member private Parameters(parameters: GlueParameter list) =
        parameters
        |> List.map (fun parameter ->
            ASTViewer.renderNode "Parameter" [
                GlueASTViewer.Name parameter.Name
                ASTViewer.renderNode "Type" [
                    GlueASTViewer.GlueType parameter.Type
                ]
            ]
        )
        |> ASTViewer.renderNode "Parameters"

    static member private Type(glueType: GlueType) =
        ASTViewer.renderNode "Type" [ GlueASTViewer.GlueType glueType ]

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

    static member private Constructors(constructors: GlueConstructor list) =
        constructors
        |> List.map (fun (GlueConstructor parameters) ->
            GlueASTViewer.Parameters parameters
        )
        |> ASTViewer.renderNode "Constructors"

    static member private GlueMember(glueMember: GlueMember) =
        match glueMember with
        | GlueMember.MethodSignature methodSignature ->
            ASTViewer.renderNode "MethodSignature" [
                GlueASTViewer.Name methodSignature.Name
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
            ]

        | GlueMember.Property propertyInfo ->
            ASTViewer.renderNode "Property" [
                GlueASTViewer.Name propertyInfo.Name
                GlueASTViewer.IsStatic propertyInfo.IsStatic
                ASTViewer.renderKeyValue
                    "Accessor"
                    (GlueAccessor.toText propertyInfo.Accessor)
                GlueASTViewer.Type propertyInfo.Type
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

    static member private GlueType
        (glueType: GlueType)
        (context: NodeContext<'Msg>)
        =
        match glueType with
        | GlueType.Variable variableInfo ->
            ASTViewer.renderNode
                "Variable"
                [
                    GlueASTViewer.Name variableInfo.Name
                    GlueASTViewer.Type variableInfo.Type
                ]
                context

        | GlueType.Interface interfaceInfo ->
            ASTViewer.renderNode
                "Interface"
                [
                    GlueASTViewer.Name interfaceInfo.Name
                    GlueASTViewer.Members interfaceInfo.Members
                ]
                context

        | GlueType.Array arrayInfo ->
            ASTViewer.renderNode
                "Array"
                [ GlueASTViewer.Name arrayInfo.Name ]
                context

        | GlueType.Discard -> ASTViewer.renderNode "Discard" [] context

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
                ]
                context

        | GlueType.FunctionDeclaration functionDeclaration ->
            ASTViewer.renderNode
                "FunctionDeclaration"
                [
                    GlueASTViewer.Name functionDeclaration.Name
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
                    GlueASTViewer.Type functionType.Type
                    GlueASTViewer.Parameters functionType.Parameters
                ]
                context

        | GlueType.IndexedAccessType indexedAccessType ->
            ASTViewer.renderNode
                "IndexedAccessType"
                [ GlueASTViewer.Type indexedAccessType ]
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

        | GlueType.Partial partialInfo ->
            ASTViewer.renderNode
                "Partial"
                [
                    GlueASTViewer.Name partialInfo.Name
                    GlueASTViewer.Members partialInfo.Members
                    GlueASTViewer.TypeParameters partialInfo.TypeParameters
                ]
                context

        | GlueType.ThisType typeName ->
            ASTViewer.renderNode
                "ThisType"
                [ GlueASTViewer.Name typeName ]
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
                    ASTViewer.renderKeyValue "FullName" typeReference.FullName
                    typeReference.TypeArguments
                    |> List.map GlueASTViewer.GlueType
                    |> ASTViewer.renderNode "TypeArguments"
                ]
                context

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
