module Glutinum.Web.Pages.Editors.FSharpAST.FSharpASTViewer

open Glutinum.Converter.FSharpAST
open Glutinum.Web.Components.AstViewer
open Feliz
open Elmish

module CaseRules =

    open Fable.Core

    let toText (caseRules: CaseRules) =
        match caseRules with
        | CaseRules.None -> "None"
        | CaseRules.LowerFirst -> "LowerFirst"
        | CaseRules.SnakeCase -> "SnakeCase"
        | CaseRules.SnakeCaseAllCaps -> "SnakeCaseAllCaps"
        | CaseRules.KebabCase -> "KebabCase"
        | _ -> "Unknown"

module FSharpAccessor =

    let toText (accessor: FSharpAccessor) =
        match accessor with
        | FSharpAccessor.ReadOnly -> "ReadOnly"
        | FSharpAccessor.WriteOnly -> "WriteOnly"
        | FSharpAccessor.ReadWrite -> "ReadWrite"

type FSharpASTViewer =

    static member private FSharpAttribute(fsharpAttribute: FSharpAttribute) =
        match fsharpAttribute with
        | FSharpAttribute.Text text ->
            ASTViewer.renderNode "Text" [ ASTViewer.renderValueOnly text ]

        | FSharpAttribute.EmitSelfInvoke ->
            ASTViewer.renderValueOnly "EmitSelfInvoke"

        | FSharpAttribute.Import(selector, from) ->
            ASTViewer.renderNode "Import" [
                ASTViewer.renderKeyValue "Selector" selector
                ASTViewer.renderKeyValue "From" from
            ]

        | FSharpAttribute.ImportAll from ->
            ASTViewer.renderNode "ImportAll" [
                ASTViewer.renderKeyValue "From" from
            ]

        | FSharpAttribute.Erase -> ASTViewer.renderValueOnly "Erase"

        | FSharpAttribute.AllowNullLiteral ->
            ASTViewer.renderValueOnly "AllowNullLiteral"

        | FSharpAttribute.StringEnum caseRules ->
            ASTViewer.renderNode "StringEnum" [
                ASTViewer.renderKeyValue
                    "CaseRules"
                    (CaseRules.toText caseRules)
            ]

        | FSharpAttribute.CompiledName name ->
            ASTViewer.renderNode "CompiledName" [ FSharpASTViewer.Name name ]

        | FSharpAttribute.EmitConstructor ->
            ASTViewer.renderValueOnly "EmitConstructor"

        | FSharpAttribute.EmitMacroConstructor text ->
            ASTViewer.renderNode "EmitMacroConstructor" [
                ASTViewer.renderKeyValue "ClassName" text
            ]

        | FSharpAttribute.RequireQualifiedAccess ->
            ASTViewer.renderValueOnly "RequireQualifiedAccess"

        | FSharpAttribute.EmitIndexer -> ASTViewer.renderValueOnly "EmitIndexer"

        | FSharpAttribute.Global -> ASTViewer.renderValueOnly "Global"

        | FSharpAttribute.ParamObject -> ASTViewer.renderValueOnly "ParamObject"

        | FSharpAttribute.EmitSelf -> ASTViewer.renderValueOnly "EmitSelf"

        | FSharpAttribute.ParamArray -> ASTViewer.renderValueOnly "ParamArray"

        | FSharpAttribute.Interface -> ASTViewer.renderValueOnly "Interface"

        | FSharpAttribute.Obsolete message ->
            ASTViewer.renderNode "Obsolete" [
                ASTViewer.renderKeyValueOption "Message" id message
            ]

    static member private Attributes(attributes: FSharpAttribute list) =
        attributes
        |> List.map FSharpASTViewer.FSharpAttribute
        |> ASTViewer.renderNode "Attributes"

    static member private Accessibility(accessibility: FSharpAccessibility) =
        let content =
            match accessibility with
            | FSharpAccessibility.Public -> "Public"
            | FSharpAccessibility.Protected -> "Protected"
            | FSharpAccessibility.Private -> "Private"

        ASTViewer.renderKeyValue "Accessibility" content

    static member private Type(fsharpType: FSharpType) =
        ASTViewer.renderNode "Type" [ FSharpASTViewer.FSharpType fsharpType ]

    static member private Types types =
        types
        |> List.map FSharpASTViewer.FSharpType
        |> ASTViewer.renderNode "Types"

    static member private ReturnType(fsharpType: FSharpType) =
        ASTViewer.renderNode "ReturnType" [
            FSharpASTViewer.FSharpType fsharpType
        ]

    static member private TypeArguments(types: FSharpType list) =
        types
        |> List.map FSharpASTViewer.Type
        |> ASTViewer.renderNode "TypeArguments"

    static member private Parameter(parameter: FSharpParameter) =
        ASTViewer.renderNode "Parameter" [
            FSharpASTViewer.Name parameter.Name
            FSharpASTViewer.IsOptional parameter.IsOptional
            FSharpASTViewer.Attributes parameter.Attributes
            FSharpASTViewer.Type parameter.Type
        ]

    static member private Parameters(parameters: FSharpParameter list) =
        parameters
        |> List.map FSharpASTViewer.Parameter
        |> ASTViewer.renderNode "Parameters"

    static member private TypeParameters
        (typeParameters: FSharpTypeParameter list)
        =
        typeParameters
        |> List.map (fun typeParameter ->
            ASTViewer.renderNode "TypeParameter" [
                FSharpASTViewer.Name typeParameter.Name

                ASTViewer.renderNodeOption
                    "Constraint"
                    FSharpASTViewer.FSharpType
                    typeParameter.Constraint

                ASTViewer.renderNodeOption
                    "Default"
                    FSharpASTViewer.FSharpType
                    typeParameter.Default
            ]
        )
        |> ASTViewer.renderNode "TypeParameters"

    static member private Name(name: string) =
        ASTViewer.renderKeyValue "Name" name

    static member private OriginalName(originalName: string) =
        ASTViewer.renderKeyValue "OriginalName" originalName

    static member private FullName(fullName: string) =
        ASTViewer.renderKeyValue "FullName" fullName

    static member private IsOptional(isOptional: bool) =
        ASTViewer.renderKeyValue "IsOptional" (string isOptional)

    static member private IsStatic(isStatic: bool) =
        ASTViewer.renderKeyValue "IsStatic" (string isStatic)

    static member private Accessor(accessor: FSharpAccessor option) =
        ASTViewer.renderKeyValueOption "Accessor" FSharpAccessor.toText accessor

    static member private XmlDoc(elements: FSharpXmlDoc list) =
        elements
        |> List.map (fun element ->
            match element with
            | FSharpXmlDoc.Summary lines ->
                lines
                |> List.map ASTViewer.renderValueOnly
                |> ASTViewer.renderNode "Summary"

            | FSharpXmlDoc.Returns content ->
                [ ASTViewer.renderValueOnly content ]
                |> ASTViewer.renderNode "Returns"

            | FSharpXmlDoc.Param param ->
                ASTViewer.renderNode "Param" [
                    FSharpASTViewer.Name param.Name
                    ASTViewer.renderKeyValue "Content" param.Content
                ]

            | FSharpXmlDoc.Remarks content ->
                [ ASTViewer.renderValueOnly content ]
                |> ASTViewer.renderNode "Remarks"

            | FSharpXmlDoc.DefaultValue content ->
                [ ASTViewer.renderValueOnly content ]
                |> ASTViewer.renderNode "DefaultValue"
        )
        |> ASTViewer.renderNode "XmlDoc"

    static member private Member(memberInfo: FSharpMember) =
        match memberInfo with
        | FSharpMember.Method methodInfo ->
            ASTViewer.renderNode "Method" [
                FSharpASTViewer.Name methodInfo.Name
                FSharpASTViewer.OriginalName methodInfo.OriginalName
                FSharpASTViewer.XmlDoc methodInfo.XmlDoc
                FSharpASTViewer.Accessibility methodInfo.Accessibility
                FSharpASTViewer.Accessor methodInfo.Accessor
                FSharpASTViewer.IsOptional methodInfo.IsOptional
                FSharpASTViewer.IsStatic methodInfo.IsStatic
                FSharpASTViewer.Attributes methodInfo.Attributes
                FSharpASTViewer.Parameters methodInfo.Parameters
                FSharpASTViewer.TypeParameters methodInfo.TypeParameters
                FSharpASTViewer.Type methodInfo.Type

            ]

        | FSharpMember.Property propertyInfo ->
            ASTViewer.renderNode "Property" [
                FSharpASTViewer.Name propertyInfo.Name
                FSharpASTViewer.XmlDoc propertyInfo.XmlDoc
                FSharpASTViewer.IsOptional propertyInfo.IsOptional
                FSharpASTViewer.IsStatic propertyInfo.IsStatic
                FSharpASTViewer.Accessibility propertyInfo.Accessibility
                FSharpASTViewer.Accessor propertyInfo.Accessor
                FSharpASTViewer.Attributes propertyInfo.Attributes
                FSharpASTViewer.TypeParameters propertyInfo.TypeParameters
                FSharpASTViewer.Parameters propertyInfo.Parameters
                FSharpASTViewer.Type propertyInfo.Type
            ]

        | FSharpMember.StaticMember staticMemberInfo ->
            ASTViewer.renderNode "StaticMember" [
                FSharpASTViewer.Name staticMemberInfo.Name
                FSharpASTViewer.OriginalName staticMemberInfo.OriginalName
                FSharpASTViewer.Accessibility staticMemberInfo.Accessibility
                FSharpASTViewer.Accessor staticMemberInfo.Accessor
                FSharpASTViewer.IsOptional staticMemberInfo.IsOptional
                FSharpASTViewer.Attributes staticMemberInfo.Attributes
                FSharpASTViewer.Parameters staticMemberInfo.Parameters
                FSharpASTViewer.TypeParameters staticMemberInfo.TypeParameters
                FSharpASTViewer.Type staticMemberInfo.Type
            ]

    static member private Members(members: FSharpMember list) =
        members
        |> List.map FSharpASTViewer.Member
        |> ASTViewer.renderNode "Members"

    static member private Declarations(declarations: FSharpType list) =
        declarations
        |> List.map FSharpASTViewer.FSharpType
        |> ASTViewer.renderNode "Declarations"

    static member private FSharpLiteral(literal: FSharpLiteral) =
        match literal with
        | FSharpLiteral.String value ->
            ASTViewer.renderNode "String" [ ASTViewer.renderValueOnly value ]

        | FSharpLiteral.Int value ->
            ASTViewer.renderNode "Int" [
                ASTViewer.renderValueOnly (string value)
            ]

        | FSharpLiteral.Float value ->
            ASTViewer.renderNode "Float" [
                ASTViewer.renderValueOnly (string value)
            ]

        | FSharpLiteral.Bool value ->
            ASTViewer.renderNode "Bool" [
                ASTViewer.renderValueOnly (string value)
            ]

        | FSharpLiteral.Null -> ASTViewer.renderValueOnly "Null"

    static member private EnumCases(cases: FSharpEnumCase list) =
        cases
        |> List.map (fun case_ ->
            ASTViewer.renderNode "FSharpEnumCase" [
                FSharpASTViewer.Name case_.Name
                ASTViewer.renderNode "Value" [
                    FSharpASTViewer.FSharpLiteral case_.Value
                ]
            ]
        )
        |> ASTViewer.renderNode "Cases"

    static member private UnionCases(cases: FSharpUnionCase list) =
        cases
        |> List.map (fun case_ ->
            ASTViewer.renderNode "FSharpUnionCase" [
                FSharpASTViewer.Name case_.Name
                FSharpASTViewer.Attributes case_.Attributes
            ]
        )
        |> ASTViewer.renderNode "Cases"

    static member private Constructor(constructor: FSharpConstructor) =
        ASTViewer.renderNode "Constructor" [
            FSharpASTViewer.Parameters constructor.Parameters
            FSharpASTViewer.Accessibility constructor.Accessibility
            FSharpASTViewer.Parameters constructor.Parameters
        ]

    static member private PrimaryConstructor(constructor: FSharpConstructor) =
        ASTViewer.renderNode "PrimaryConstructor" [
            FSharpASTViewer.Constructor constructor
        ]

    static member private SecondaryConstructors
        (constructors: FSharpConstructor list)
        =
        constructors
        |> List.map FSharpASTViewer.Constructor
        |> ASTViewer.renderNode "SecondaryConstructors"

    static member private ExplicitFields(fields: FSharpExplicitField list) =
        fields
        |> List.map (fun field ->
            ASTViewer.renderNode "Field" [
                FSharpASTViewer.Name field.Name
                FSharpASTViewer.Type field.Type
            ]
        )
        |> ASTViewer.renderNode "ExplicitFields"

    static member private FSharpType
        (fsharpType: FSharpType)
        (context: NodeContext<'Msg>)
        =
        match fsharpType with
        | FSharpType.Object -> ASTViewer.renderValueOnly "Object" context

        | FSharpType.Interface interfaceInfo ->
            ASTViewer.renderNode
                "Interface"
                [
                    FSharpASTViewer.Name interfaceInfo.Name
                    FSharpASTViewer.OriginalName interfaceInfo.OriginalName
                    FSharpASTViewer.Attributes interfaceInfo.Attributes
                    FSharpASTViewer.Members interfaceInfo.Members
                    FSharpASTViewer.TypeParameters interfaceInfo.TypeParameters
                ]
                context

        | FSharpType.Class classInfo ->
            ASTViewer.renderNode
                "Class"
                [
                    FSharpASTViewer.Name classInfo.Name
                    FSharpASTViewer.Attributes classInfo.Attributes
                    FSharpASTViewer.TypeParameters classInfo.TypeParameters
                    FSharpASTViewer.PrimaryConstructor
                        classInfo.PrimaryConstructor
                    FSharpASTViewer.SecondaryConstructors
                        classInfo.SecondaryConstructors
                    FSharpASTViewer.ExplicitFields classInfo.ExplicitFields
                ]
                context

        | FSharpType.Mapped mapped ->
            ASTViewer.renderNode
                "Mapped"
                [
                    FSharpASTViewer.Name mapped.Name
                    FSharpASTViewer.Declarations mapped.Declarations
                ]
                context

        | FSharpType.Discard -> ASTViewer.renderValueOnly "Discard" context

        | FSharpType.Primitive primitive ->
            let content =
                match primitive with
                | FSharpPrimitive.String -> Html.span "String"
                | FSharpPrimitive.Int -> Html.span "Int"
                | FSharpPrimitive.Float -> Html.span "Float"
                | FSharpPrimitive.Bool -> Html.span "Bool"
                | FSharpPrimitive.Unit -> Html.span "Unit"
                | FSharpPrimitive.Number -> Html.span "Number"
                | FSharpPrimitive.Null -> Html.span "Null"

            ASTViewer.renderNode
                "Primitive"
                [
                    // Discard the context
                    fun _ -> content
                ]
                context

        | FSharpType.Enum enumInfo ->
            let enumTypeText =
                match enumInfo.Type with
                | FSharpEnumType.Numeric -> "Numeric"
                | FSharpEnumType.String -> "String"
                | FSharpEnumType.Unknown -> "Unknown"

            ASTViewer.renderNode
                "Enum"
                [
                    FSharpASTViewer.Name enumInfo.Name
                    ASTViewer.renderKeyValue "FSharpEnumType" enumTypeText
                    FSharpASTViewer.EnumCases enumInfo.Cases
                ]
                context

        | FSharpType.TypeAlias typeAlias ->
            ASTViewer.renderNode
                "TypeAlias"
                [
                    FSharpASTViewer.Name typeAlias.Name
                    FSharpASTViewer.Type typeAlias.Type
                    FSharpASTViewer.TypeParameters typeAlias.TypeParameters
                ]
                context

        | FSharpType.Function functionType ->
            ASTViewer.renderNode
                "Function"
                [
                    FSharpASTViewer.Parameters functionType.Parameters
                    FSharpASTViewer.TypeArguments functionType.TypeArguments
                    FSharpASTViewer.Type functionType.ReturnType
                ]
                context

        | FSharpType.Module moduleInfo ->
            ASTViewer.renderNode
                "Module"
                [
                    FSharpASTViewer.Name moduleInfo.Name
                    ASTViewer.renderKeyValue
                        "IsRecursive"
                        (string moduleInfo.IsRecursive)

                    FSharpASTViewer.Types moduleInfo.Types
                ]
                context

        | FSharpType.Option optionType ->
            ASTViewer.renderNode
                "Option"
                [ FSharpASTViewer.Type optionType ]
                context

        | FSharpType.ResizeArray resizeArrayType ->
            ASTViewer.renderNode
                "ResizeArray"
                [ FSharpASTViewer.Type resizeArrayType ]
                context

        | FSharpType.ThisType typeName ->
            ASTViewer.renderNode
                "ThisType"
                [ FSharpASTViewer.Name typeName ]
                context

        | FSharpType.Tuple tupleTypes ->
            ASTViewer.renderNode
                "Tuple"
                [ FSharpASTViewer.Types tupleTypes ]
                context

        | FSharpType.TypeParameter typeParameter ->
            ASTViewer.renderNode
                "TypeParameter"
                [ FSharpASTViewer.Name typeParameter ]
                context

        | FSharpType.TypeReference typeReference ->
            ASTViewer.renderNode
                "TypeReference"
                [
                    FSharpASTViewer.Name typeReference.Name
                    FSharpASTViewer.FullName typeReference.FullName
                    FSharpASTViewer.TypeArguments typeReference.TypeArguments
                ]
                context

        | FSharpType.Union unionInfo ->
            ASTViewer.renderNode
                "Union"
                [
                    FSharpASTViewer.Name unionInfo.Name
                    FSharpASTViewer.UnionCases unionInfo.Cases
                ]
                context

        | FSharpType.Unsupported syntaxKind ->
            ASTViewer.renderNode
                "Unsupported"
                [ ASTViewer.renderKeyValue "SyntaxKind" (string syntaxKind) ]
                context

    static member Render
        (types: FSharpType list)
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

            FSharpASTViewer.FSharpType glueType context
        )
