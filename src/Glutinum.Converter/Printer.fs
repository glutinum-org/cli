module Glutinum.Converter.Printer

open System
open Glutinum.Chalk
open Glutinum.Converter.FSharpAST
open Fable.Core
open System.Text.RegularExpressions

type Printer() =
    let buffer = new Text.StringBuilder()
    let mutable indentationLevel = 0
    let indentationText = "    " // 4 spaces

    member __.Indent = indentationLevel <- indentationLevel + 1

    member __.Unindent =
        // Safety measure so we don't have negative indentation space
        indentationLevel <- System.Math.Max(indentationLevel - 1, 0)

    /// <summary>Write the provided text prefixed with indentation</summary>
    /// <param name="text"></param>
    /// <returns></returns>
    member __.Write(text: string) =
        buffer.Append(String.replicate indentationLevel indentationText + text)
        |> ignore

    /// <summary>
    /// Write the provided text directly in the buffer
    ///
    /// Useful when you don't want to prefix the text with indentation
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    member __.WriteInline(text: string) = buffer.Append(text) |> ignore

    member __.NewLine = buffer.AppendLine() |> ignore

    override __.ToString() = buffer.ToString().Trim() + "\n"

    member __.ToStringWithoutTrailNewLine() = buffer.ToString().Trim()

module Naming =
    let (|Digit|_|) (digit: string) =
        if String.IsNullOrWhiteSpace digit then
            None
        elif Char.IsDigit(digit, 0) then
            Some digit
        else
            None

let private sanitizeEnumCaseName (name: string) =
    let name =
        name
        |> String.removeSingleQuote
        |> String.removeDoubleQuote
        |> String.capitalizeFirstLetter

    match name with
    | Naming.Digit _ ->
        // F# enums cannot start with a digit, so we escape it with backticks
        $"``{name}``"
    | _ -> name

let private hasParamArrayAttribute (attributes: FSharpAttribute list) =
    attributes
    |> List.exists (
        function
        | FSharpAttribute.ParamArray -> true
        | _ -> false
    )

let private attributeToText (fsharpAttribute: FSharpAttribute) =
    match fsharpAttribute with
    | FSharpAttribute.Text text -> $"[<%s{text}>]"
    | FSharpAttribute.EmitSelfInvoke -> "[<Emit(\"$0($1...)\")>]"
    | FSharpAttribute.Import(name, module_) ->
        $"[<Import(\"{name}\", \"{module_}\")>]"
    | FSharpAttribute.Erase -> "[<Erase>]"
    | FSharpAttribute.AllowNullLiteral -> "[<AllowNullLiteral>]"
    | FSharpAttribute.StringEnum caseRules ->
        let caseRulesText =
            match caseRules with
            | CaseRules.None -> "CaseRules.None"
            | CaseRules.LowerFirst -> "CaseRules.LowerFirst"
            | CaseRules.SnakeCase -> "CaseRules.SnakeCase"
            | CaseRules.SnakeCaseAllCaps -> "CaseRules.SnakeCaseAllCaps"
            | CaseRules.KebabCase -> "CaseRules.KebabCase"
            | _ -> failwith "Unsupported case rules: %A{caseRules}"

        $"[<StringEnum({caseRulesText})>]"
    | FSharpAttribute.CompiledName name -> $"[<CompiledName(\"{name}\")>]"
    | FSharpAttribute.RequireQualifiedAccess -> "[<RequireQualifiedAccess>]"
    | FSharpAttribute.EmitConstructor -> "[<EmitConstructor>]"
    | FSharpAttribute.EmitMacroConstructor className ->
        $"[<Emit(\"new $0.{className}($1...)\")>]"
    | FSharpAttribute.ImportAll module_ -> $"[<ImportAll(\"{module_}\")>]"
    | FSharpAttribute.ImportDefault module_ ->
        $"[<ImportDefault(\"{module_}\")>]"
    | FSharpAttribute.EmitIndexer -> "[<EmitIndexer>]"
    | FSharpAttribute.Global -> "[<Global>]"
    | FSharpAttribute.ParamObject -> "[<ParamObject>]"
    | FSharpAttribute.EmitSelf -> "[<Emit(\"$0\")>]"
    | FSharpAttribute.ParamArray -> "[<ParamArray>]"
    | FSharpAttribute.Interface -> "[<Interface>]"
    | FSharpAttribute.Obsolete message ->
        match message with
        | Some message -> $"[<Obsolete(\"%s{message}\")>]"
        | None -> "[<Obsolete>]"
    | FSharpAttribute.AbstractClass -> "[<AbstractClass>]"
    | FSharpAttribute.EmitMacroInvoke methodName ->
        $"[<Emit(\"$0.{methodName}($1...)\")>]"

let private printInlineAttribute
    (printer: Printer)
    (fsharpAttribute: FSharpAttribute)
    =
    printer.WriteInline(attributeToText fsharpAttribute)
    printer.WriteInline(" ")

let private printInlineAttributes
    (printer: Printer)
    (fsharpAttributes: FSharpAttribute list)
    =
    if fsharpAttributes.Length > 0 then
        let attributesText =
            fsharpAttributes
            |> List.map attributeToText
            // Merge attributes:
            // [<CompiledName("UP")>][<CompiledValue(1)>]
            // becomes
            // [<CompiledName("UP"); CompiledValue(1)>]
            |> String.concat ""
            |> String.replace ">][<" "; "

        printer.WriteInline(attributesText)
        printer.WriteInline(" ")

let private printCompactAttributesAndNewLine
    (printer: Printer)
    (fsharpAttributes: FSharpAttribute list)
    =
    if fsharpAttributes.Length > 0 then
        let attributesText =
            fsharpAttributes
            |> List.map attributeToText
            // Merge attributes:
            // [<CompiledName("UP")>][<CompiledValue(1)>]
            // becomes
            // [<CompiledName("UP"); CompiledValue(1)>]
            |> String.concat ""
            |> String.replace ">][<" "; "

        printer.Write(attributesText)
        printer.NewLine

let private printAttributes
    (printer: Printer)
    (fsharpAttributes: FSharpAttribute list)
    =
    fsharpAttributes
    |> List.iter (fun fsharpAttribute ->
        printer.Write(attributeToText fsharpAttribute)
        printer.NewLine
    )

let rec private tryTransformTypeParametersToText
    (isDeclaration: bool)
    (typeParameters: FSharpTypeParameter list)
    =
    let printer = new Printer()

    if not typeParameters.IsEmpty then
        printer.WriteInline("<")

        typeParameters
        |> List.iteri (fun index typeParameter ->
            if index <> 0 then
                printer.WriteInline(", ")

            printer.WriteInline($"'{typeParameter.Name}")
        )

        if isDeclaration then
            typeParameters
            |> List.filter _.Constraint.IsSome
            |> List.iteri (fun index typeParameter ->
                match typeParameter.Constraint with
                | Some constraint_ ->
                    if index = 0 then
                        printer.WriteInline(" when ")
                    else
                        printer.WriteInline(" and ")

                    printer.WriteInline($"'{typeParameter.Name}")
                    printer.WriteInline(" :> ")
                    printer.WriteInline(printType constraint_)
                | None -> ()
            )

        printer.WriteInline(">")

        printer.ToStringWithoutTrailNewLine() |> Some

    else
        None

and printTypeParameters
    (printer: Printer)
    (typeParameters: FSharpTypeParameter list)
    =
    let isDeclaration = true

    match tryTransformTypeParametersToText isDeclaration typeParameters with
    | Some typeParameters -> printer.WriteInline(typeParameters)
    | None -> ()

and printType (fsharpType: FSharpType) =
    let printTypeNameWithTypeParameters
        (name: string)
        (typeParameters: FSharpTypeParameter list)
        =
        let isDeclaration = false

        match tryTransformTypeParametersToText isDeclaration typeParameters with
        | Some typeParameters -> $"{name}{typeParameters}"
        | None -> name

    match fsharpType with
    | FSharpType.Object -> "obj"
    | FSharpType.Mapped info ->
        let isDeclaration = false

        match
            tryTransformTypeParametersToText isDeclaration info.TypeParameters
        with
        | Some typeParameters -> $"{info.Name}{typeParameters}"
        | None -> info.Name

    | FSharpType.SingleErasedCaseUnion info -> info.Name

    | FSharpType.Union info ->
        let cases =
            info.Cases
            |> List.map (fun case ->
                match case with
                | FSharpUnionCase.Named caseInfo -> caseInfo.Name
                | FSharpUnionCase.Typed typ -> printType typ
            )
            |> String.concat ", "

        let option =
            if info.IsOptional then
                " option"
            else
                ""

        $"{info.Name}<{cases}>{option}"

    | FSharpType.ThisType thisTypeInfo ->
        printTypeNameWithTypeParameters
            thisTypeInfo.Name
            thisTypeInfo.TypeParameters

    | FSharpType.Tuple types ->
        types |> List.map printType |> String.concat " * "

    | FSharpType.Function functionInfo ->
        match functionInfo.Parameters with
        | [] ->
            let suffix = printType functionInfo.ReturnType

            $"(unit -> {suffix})"

        | _ ->
            let parameters =
                functionInfo.Parameters
                |> List.map (fun p ->
                    let typ = printType p.Type

                    let option =
                        if p.IsOptional then
                            " option"
                        else
                            ""

                    $"{typ}{option}"
                )
                |> String.concat " -> "

            let returnType = printType functionInfo.ReturnType

            $"({parameters} -> {returnType})"

    | FSharpType.Enum info -> info.Name
    | FSharpType.Primitive info ->
        match info with
        | FSharpPrimitive.String -> "string"
        | FSharpPrimitive.Int -> "int"
        | FSharpPrimitive.Float -> "float"
        | FSharpPrimitive.Bool -> "bool"
        | FSharpPrimitive.Unit -> "unit"
        | FSharpPrimitive.Number -> "float"
        | FSharpPrimitive.Null -> "obj"
    | FSharpType.TypeReference typeReference ->
        if typeReference.TypeArguments.Length > 0 then
            let typeArguments =
                typeReference.TypeArguments
                |> List.map printType
                |> String.concat ", "

            $"{typeReference.Name}<{typeArguments}>"
        else
            typeReference.Name

    | FSharpType.TypeParameter name -> $"'{name}"
    | FSharpType.Option optionType -> printType optionType + " option"
    | FSharpType.ResizeArray arrayType -> $"ResizeArray<{printType arrayType}>"
    | FSharpType.JSApi apiInfo ->
        match apiInfo with
        | FSharpJSApi.ReadonlyArray typ -> $"ReadonlyArray<{printType typ}>"
    | FSharpType.Interface interfaceInfo ->
        printTypeNameWithTypeParameters
            interfaceInfo.Name
            interfaceInfo.TypeParameters
    | FSharpType.Class classInfo -> classInfo.Name
    | FSharpType.TypeAlias aliasInfo ->
        printTypeNameWithTypeParameters aliasInfo.Name aliasInfo.TypeParameters
    | FSharpType.Delegate delegateInfo ->
        printTypeNameWithTypeParameters
            delegateInfo.Name
            delegateInfo.TypeParameters
    | FSharpType.Module _
    | FSharpType.Unsupported _
    | FSharpType.Discard -> "obj"

module FSharpAccessibility =

    let print (printer: Printer) (accessibility: FSharpAccessibility) =
        match accessibility with
        // We print an empty string for public like that the indentation
        // matches the other accessibilities
        | FSharpAccessibility.Public -> printer.Write("")
        | FSharpAccessibility.Private -> printer.Write("private ")
        | FSharpAccessibility.Protected -> printer.Write("protected ")

    let printInline (printer: Printer) (accessibility: FSharpAccessibility) =
        match accessibility with
        // We print an empty string for public like that the indentation
        // matches the other accessibilities
        | FSharpAccessibility.Public -> printer.WriteInline("")
        | FSharpAccessibility.Private -> printer.WriteInline("private ")
        | FSharpAccessibility.Protected -> printer.WriteInline("protected ")

// Comment adaptation should be moved in the transform phase
let private codeInline (line: string) =
    Regex("`(?<code>[^`]*)`")
        .Replace(line, (fun m -> $"""<c>{m.Groups.["code"].Value}</c>"""))

let private codeBlock (text: string) =
    Regex("```(?<lang>\S*)(?<code>[^`]+)```", RegexOptions.Multiline)
        .Replace(
            text,
            (fun m ->
                let lang = m.Groups.["lang"].Value
                let code = m.Groups.["code"].Value

                if String.IsNullOrWhiteSpace lang then
                    $"""<code>{code}</code>"""
                else
                    $"""<code lang="{lang}">{code}</code>"""
            )
        )

let private link (text: string) =
    // tsdoc link format is {@link link|customText}
    // However when getting the text after `getDocumentationComment` and `displayPartsToString`
    // the `|` seems to be replaced by a space
    // Regex below tries to handle both cases
    Regex("\{@link\s+(?<link>[^\s}|]+)\s*((\|)?\s*(?<customText>[^}]+))?\}")
        .Replace(
            text,
            (fun m ->
                let link = m.Groups.["link"].Value.Trim()
                let customText = m.Groups.["customText"].Value

                if String.IsNullOrWhiteSpace customText then
                    $"""<see href="{link}">{link}</see>"""
                else
                    let customText = customText.Trim()
                    $"""<see href="{link}">{customText}</see>" """.TrimEnd()
            )
        )

let private transformToXmlDoc (line: string) =
    line |> codeBlock |> codeInline |> link

let private printBlockTag
    (printer: Printer)
    (tagName: string)
    (attributes: (string * string) list)
    (content: string)
    =
    let printXmlDocLine (line: string) =
        if String.IsNullOrWhiteSpace line then
            printer.Write("///")
        else
            printer.Write($"/// {line}")

    let lines = content |> transformToXmlDoc |> String.splitLines

    let openTagText =
        match attributes with
        | [] -> tagName
        | attributes ->
            let attributesText =
                attributes
                |> List.map (fun (key, value) -> $"{key}=\"{value}\"")
                |> String.concat " "

            $"""{tagName} {attributesText}"""

    printXmlDocLine $"<%s{openTagText}>"
    printer.NewLine

    lines
    |> List.iter (fun line ->
        printXmlDocLine line
        printer.NewLine
    )

    printXmlDocLine $"</%s{tagName}>"
    printer.NewLine

let private printXmlDoc (printer: Printer) (elements: FSharpXmlDoc list) =
    let summary, others =
        elements
        |> List.partition (
            function
            | FSharpXmlDoc.Summary _
            | FSharpXmlDoc.DefaultValue _ -> true
            | FSharpXmlDoc.Returns _
            | FSharpXmlDoc.Param _
            | FSharpXmlDoc.Remarks _
            | FSharpXmlDoc.TypeParam _
            | FSharpXmlDoc.Example _ -> false
        )

    // Print the summary first
    let summaryLines =
        summary
        |> List.map (fun element ->
            match element with
            | FSharpXmlDoc.Summary lines -> lines
            | FSharpXmlDoc.DefaultValue content -> [ ""; content; "" ] // Add a new line before the default value
            | _ -> failwith "This element should not be in the summary list"
        )
        |> List.concat
        |> List.removeConsecutiveEmptyLines
        |> List.trimEmptyLines

    // Only generate the summary if there is content
    if summaryLines |> List.forall String.IsNullOrWhiteSpace |> not then
        summaryLines |> String.concat "\n" |> printBlockTag printer "summary" []

    others
    |> List.iter (fun element ->
        match element with
        | FSharpXmlDoc.Summary _
        | FSharpXmlDoc.DefaultValue _ ->
            failwith "This element should have been processed in the summary"

        | FSharpXmlDoc.Returns content ->
            printBlockTag printer "returns" [] content

        | FSharpXmlDoc.Param info ->
            printBlockTag printer "param" [ "name", info.Name ] info.Content

        | FSharpXmlDoc.Remarks content ->
            printBlockTag printer "remarks" [] content

        | FSharpXmlDoc.Example content ->
            printBlockTag printer "example" [] content

        | FSharpXmlDoc.TypeParam info ->
            printBlockTag
                printer
                "typeparam"
                [ "name", info.TypeName ]
                info.Content
    )

let printParameters (printer: Printer) (parameters: FSharpParameter list) =
    if parameters.Length = 0 then
        printer.WriteInline("unit")
    else
        parameters
        |> List.iteri (fun index p ->
            if index <> 0 then
                printer.WriteInline(" * ")

            printInlineAttributes printer p.Attributes

            if p.IsOptional then
                printer.WriteInline("?")

            printer.WriteInline($"{p.Name}: {printType p.Type}")

            if hasParamArrayAttribute p.Attributes then
                printer.WriteInline(" []")
        )

let private printInterface (printer: Printer) (interfaceInfo: FSharpInterface) =
    printAttributes printer interfaceInfo.Attributes

    printer.Write($"type {interfaceInfo.Name}")
    printTypeParameters printer interfaceInfo.TypeParameters
    printer.WriteInline(" =")
    printer.NewLine

    printer.Indent

    interfaceInfo.Inheritance
    |> List.iter (fun typ ->
        printer.Write($"inherit %s{printType typ}")
        printer.NewLine
    )

    interfaceInfo.Members
    |> List.iter (
        function
        // TODO: Rewrite the code below to share more code
        // Right now there are a lots of duplication and special rules
        // Can these rules be represented in the AST to simplify the code?
        | FSharpMember.Method methodInfo ->
            printXmlDoc printer methodInfo.XmlDoc
            printCompactAttributesAndNewLine printer methodInfo.Attributes

            if methodInfo.IsStatic then
                printer.Write("static ")
            else
                printer.Write("abstract ")

            printer.WriteInline($"member {methodInfo.Name}")

            printTypeParameters printer methodInfo.TypeParameters

            if methodInfo.IsStatic then
                printer.WriteInline(" ")
                // Special case for functions with no parameters
                if methodInfo.Parameters.Length = 0 then
                    printer.WriteInline("()")
                else
                    printer.WriteInline("(")

                    methodInfo.Parameters
                    |> List.iteri (fun index p ->
                        if index <> 0 then
                            printer.WriteInline(", ")

                        printInlineAttributes printer p.Attributes

                        if p.IsOptional then
                            printer.WriteInline("?")

                        printer.WriteInline($"{p.Name}: {printType p.Type}")

                        if hasParamArrayAttribute p.Attributes then
                            printer.WriteInline(" []")
                    )

                    printer.WriteInline(")")
            else
                printer.WriteInline(": ")

                printParameters printer methodInfo.Parameters

            if methodInfo.IsStatic then
                printer.WriteInline(" : ")
            else
                printer.WriteInline(" -> ")

            printer.WriteInline(printType methodInfo.Type)

            if methodInfo.IsStatic then
                printer.WriteInline(" = nativeOnly")

            printer.NewLine

        | FSharpMember.Property propertyInfo ->

            printXmlDoc printer propertyInfo.XmlDoc
            printAttributes printer propertyInfo.Attributes

            if propertyInfo.IsStatic then
                printer.Write($"static member inline ")

                FSharpAccessibility.printInline
                    printer
                    propertyInfo.Accessibility

                printer.WriteInline($"{propertyInfo.Name}")

                let printGetter () =
                    printer.Indent

                    printer.Write(
                        $"with get () : {printType propertyInfo.Type} ="
                    )

                    printer.NewLine
                    printer.Indent

                    match propertyInfo.Body with
                    | FSharpMemberInfoBody.NativeOnly ->
                        printer.Write("nativeOnly")
                    | FSharpMemberInfoBody.JavaScriptStaticProperty ->
                        printer.Write
                            $"emitJsExpr () $$\"\"\"
import {{ %s{interfaceInfo.OriginalName} }} from \"{Naming.MODULE_PLACEHOLDER}\";
%s{interfaceInfo.OriginalName}.%s{propertyInfo.OriginalName}\"\"\""

                    printer.Unindent
                    printer.Unindent

                let printerSetter (useAndKeyword: bool) =
                    printer.Indent

                    if useAndKeyword then
                        printer.Write "and "
                    else
                        printer.Write "with "

                    printer.WriteInline(
                        $"set (value: {printType propertyInfo.Type}) ="
                    )

                    printer.NewLine
                    printer.Indent

                    match propertyInfo.Body with
                    | FSharpMemberInfoBody.NativeOnly ->
                        printer.Write("nativeOnly")
                    | FSharpMemberInfoBody.JavaScriptStaticProperty ->
                        printer.Write
                            $"emitJsExpr (value) $$\"\"\"
import {{ %s{interfaceInfo.OriginalName} }} from \"{Naming.MODULE_PLACEHOLDER}\";
%s{interfaceInfo.OriginalName}.%s{propertyInfo.OriginalName} = $0\"\"\""

                    printer.Unindent
                    printer.Unindent

                match propertyInfo.Accessor with
                | None ->
                    printer.WriteInline($": {printType propertyInfo.Type}")
                    printer.WriteInline(" = nativeOnly")
                | Some accessor ->
                    printer.NewLine

                    match accessor with
                    | FSharpAccessor.ReadOnly -> printGetter ()
                    | FSharpAccessor.WriteOnly ->
                        // I don't think this is possible in TypeScript but let's support it
                        // to not crash the code generator
                        printerSetter false
                    | FSharpAccessor.ReadWrite ->
                        printGetter ()
                        printer.NewLine
                        printerSetter true

            else
                printer.Write($"abstract member {propertyInfo.Name}")

                propertyInfo.Parameters
                |> List.iteri (fun index p ->
                    if index = 0 then
                        printer.WriteInline(": ")
                    else
                        printer.WriteInline(" -> ")

                    let option =
                        if p.IsOptional then
                            " option"
                        else
                            ""

                    printer.WriteInline(
                        $"{p.Name}: {printType p.Type}{option}"
                    )
                )

                if propertyInfo.Parameters.Length > 0 then
                    printer.WriteInline(" -> ")
                else
                    printer.WriteInline(": ")

                printer.WriteInline($"{printType propertyInfo.Type}")

                if propertyInfo.IsOptional then
                    printer.WriteInline(" option")

                propertyInfo.Accessor
                |> Option.map (
                    function
                    | FSharpAccessor.ReadOnly -> " with get"
                    | FSharpAccessor.WriteOnly -> " with set"
                    | FSharpAccessor.ReadWrite -> " with get, set"
                )
                |> Option.iter printer.WriteInline

            printer.NewLine

        | FSharpMember.StaticMember staticMemberInfo ->
            printCompactAttributesAndNewLine printer staticMemberInfo.Attributes

            printer.Write($"static member inline {staticMemberInfo.Name} ")

            printTypeParameters printer staticMemberInfo.TypeParameters

            if staticMemberInfo.Parameters.IsEmpty then
                printer.WriteInline("() : ")
                printer.WriteInline(printType staticMemberInfo.Type)
                printer.WriteInline(" =")
                printer.Indent
                printer.NewLine

                printer.Write
                    $"emitJsExpr () $$\"\"\"
import {{ %s{interfaceInfo.OriginalName} }} from \"{Naming.MODULE_PLACEHOLDER}\";
%s{interfaceInfo.OriginalName}.%s{staticMemberInfo.OriginalName}()\"\"\""

                printer.NewLine

                printer.Unindent

            else
                printer.WriteInline("(")

                staticMemberInfo.Parameters
                |> List.iteri (fun index p ->
                    if index <> 0 then
                        printer.WriteInline(", ")

                    printInlineAttributes printer p.Attributes

                    if p.IsOptional then
                        printer.WriteInline("?")

                    printer.WriteInline($"{p.Name}: {printType p.Type}")

                    if hasParamArrayAttribute p.Attributes then
                        printer.WriteInline(" []")
                )

                printer.WriteInline("): ")
                printer.WriteInline(printType staticMemberInfo.Type)
                printer.WriteInline(" =")
                printer.NewLine

                printer.Indent

                let forwardedArgments =
                    staticMemberInfo.Parameters
                    |> List.map (fun p -> p.Name)
                    |> String.concat ", "

                let macroArguments =
                    staticMemberInfo.Parameters
                    |> List.mapi (fun index _ -> $"$%i{index}")
                    |> String.concat ", "

                printer.Write
                    $"emitJsExpr (%s{forwardedArgments}) $$\"\"\"
import {{ %s{interfaceInfo.OriginalName} }} from \"{Naming.MODULE_PLACEHOLDER}\";
%s{interfaceInfo.OriginalName}.%s{staticMemberInfo.OriginalName}(%s{macroArguments})\"\"\""

                printer.NewLine
                printer.Unindent
    )

    if interfaceInfo.Members.IsEmpty && interfaceInfo.Inheritance.IsEmpty then
        printer.Write("interface end")
        printer.NewLine

    printer.Unindent

let private printPrimaryConstructor
    (printer: Printer)
    (constructor: FSharpConstructor)
    =
    printCompactAttributesAndNewLine printer constructor.Attributes

    FSharpAccessibility.print printer constructor.Accessibility

    printer.WriteInline($"(")
    printer.Indent

    constructor.Parameters
    |> List.iteri (fun index p ->
        if index <> 0 then
            printer.WriteInline(",")

        printer.NewLine

        if p.IsOptional then
            printer.Write("?")
        else
            printer.Write("") // Empty string to have the correct indentation

        printer.WriteInline($"{p.Name}: {printType p.Type}")

        if hasParamArrayAttribute p.Attributes then
            printer.WriteInline(" []")
    )

    printer.Unindent
    printer.NewLine
    printer.Write(") =")
    printer.NewLine

let private printClass (printer: Printer) (classInfo: FSharpClass) =
    printAttributes printer classInfo.Attributes

    printer.Write($"type {classInfo.Name}")
    printTypeParameters printer classInfo.TypeParameters
    printer.NewLine
    printer.Indent
    printPrimaryConstructor printer classInfo.PrimaryConstructor
    printer.NewLine

    if not classInfo.SecondaryConstructors.IsEmpty then
        classInfo.SecondaryConstructors
        |> List.iter (fun constructor ->
            printCompactAttributesAndNewLine printer constructor.Attributes
            FSharpAccessibility.print printer constructor.Accessibility
            printer.WriteInline($"new (")

            constructor.Parameters
            |> List.iteri (fun index p ->
                if index <> 0 then
                    printer.WriteInline(", ")

                if p.IsOptional then
                    printer.WriteInline("?")

                printer.WriteInline($"{p.Name}: {printType p.Type}")

                if hasParamArrayAttribute p.Attributes then
                    printer.WriteInline(" []")
            )

            printer.WriteInline(") =")
            printer.NewLine
            printer.Indent
            printer.Write($"%s{classInfo.Name}()")
            printer.NewLine
            printer.Unindent
            printer.NewLine
        )

    if
        classInfo.ExplicitFields.IsEmpty
        && classInfo.SecondaryConstructors.IsEmpty
    then
        printer.Write("class end")
        printer.NewLine

    if not classInfo.ExplicitFields.IsEmpty then
        classInfo.ExplicitFields
        |> List.iter (fun explicitField ->
            printer.Write($"member val {explicitField.Name} : ")
            printer.WriteInline(printType explicitField.Type)
            printer.WriteInline(" = nativeOnly with get, set")
            printer.NewLine
        )

    printer.Unindent

let private printEnum (printer: Printer) (enumInfo: FSharpEnum) =
    printer.Write("[<RequireQualifiedAccess>]")
    printer.NewLine

    printer.Write($"type {enumInfo.Name} =")
    printer.NewLine
    printer.Indent

    enumInfo.Cases
    |> List.iter (fun enumCaseInfo ->
        let enumCaseName = enumCaseInfo.Name |> sanitizeEnumCaseName

        match enumCaseInfo.Value with
        | FSharpLiteral.Int value ->
            printer.Write($"""| {enumCaseName} = %i{value}""")

        | FSharpLiteral.Bool _
        | FSharpLiteral.String _
        | FSharpLiteral.Float _
        | FSharpLiteral.Null -> ()

        printer.NewLine
    )

    printer.Unindent

let private printTypeAlias (printer: Printer) (aliasInfo: FSharpTypeAlias) =

    printXmlDoc printer aliasInfo.XmlDoc
    printAttributes printer aliasInfo.Attributes

    printer.Write($"type {aliasInfo.Name}")
    printTypeParameters printer aliasInfo.TypeParameters
    printer.WriteInline(" =")

    printer.NewLine
    printer.Indent
    printer.Write(printType aliasInfo.Type)
    printer.NewLine
    printer.Unindent

let private printDelegate (printer: Printer) (delegateInfo: FSharpDelegate) =
    printer.Write($"type {delegateInfo.Name}")
    printTypeParameters printer delegateInfo.TypeParameters
    printer.WriteInline(" =")

    printer.NewLine
    printer.Indent

    printer.Write("delegate of ")

    printParameters printer delegateInfo.Parameters

    printer.WriteInline $" -> {printType delegateInfo.ReturnType}"

    printer.NewLine
    printer.Unindent

let rec private print (printer: Printer) (fsharpTypes: FSharpType list) =
    match fsharpTypes with
    | fsharpType :: tail ->
        printer.NewLine

        match fsharpType with
        | FSharpType.Union unionInfo ->
            printAttributes printer unionInfo.Attributes

            printer.Write($"type {unionInfo.Name} =")
            printer.NewLine
            printer.Indent

            unionInfo.Cases
            |> List.iter (fun enumCaseInfo ->
                printer.Write($"""| """)

                match enumCaseInfo with
                | FSharpUnionCase.Named enumCaseInfo ->
                    printInlineAttributes printer enumCaseInfo.Attributes

                    printer.WriteInline(enumCaseInfo.Name)
                | FSharpUnionCase.Typed typ ->
                    printer.WriteInline(printType typ)
                    printer.NewLine

                printer.NewLine
            )

            printer.Unindent

        | FSharpType.Enum enumInfo -> printEnum printer enumInfo

        | FSharpType.Interface interfaceInfo ->
            printInterface printer interfaceInfo

        | FSharpType.Unsupported syntaxKind ->
            printer.Write($"obj // Unsupported syntax kind: %A{syntaxKind}")
            printer.NewLine

        | FSharpType.Module moduleInfo ->
            printer.Write($"module ")

            if moduleInfo.IsRecursive then
                printer.WriteInline($"rec ")

            printer.WriteInline($"{moduleInfo.Name} =")
            printer.NewLine

            printer.Indent
            print printer moduleInfo.Types
            printer.Unindent

        // TODO: Make print return the tail
        // Allowing module to eat they content and be able to unindent?
        // print printer tail

        // printer.Unindent

        | FSharpType.TypeAlias aliasInfo -> printTypeAlias printer aliasInfo
        | FSharpType.Class classInfo -> printClass printer classInfo
        | FSharpType.SingleErasedCaseUnion erasedCaseUnionInfo ->
            printXmlDoc printer erasedCaseUnionInfo.XmlDoc

            printAttributes
                printer
                (FSharpAttribute.Erase :: erasedCaseUnionInfo.Attributes)

            printer.Write($"type {erasedCaseUnionInfo.Name}")
            printTypeParameters printer [ erasedCaseUnionInfo.TypeParameter ]
            printer.WriteInline(" =")

            printer.NewLine
            printer.Indent

            printer.Write(
                $"| %s{erasedCaseUnionInfo.Name} of '%s{erasedCaseUnionInfo.TypeParameter.Name}"
            )

            printer.NewLine
            printer.NewLine
            printer.Write("member inline this.Value =")
            printer.NewLine
            printer.Indent
            printer.Write($"let (%s{erasedCaseUnionInfo.Name} output) = this")
            printer.NewLine
            printer.Write("output")
            printer.NewLine
            printer.Unindent
            printer.Unindent

        | FSharpType.Delegate delegateInfo -> printDelegate printer delegateInfo

        | FSharpType.Mapped _
        | FSharpType.Primitive _
        | FSharpType.Object
        | FSharpType.TypeReference _
        | FSharpType.Option _
        | FSharpType.ResizeArray _
        | FSharpType.JSApi _
        | FSharpType.TypeParameter _
        | FSharpType.Discard
        | FSharpType.Function _
        | FSharpType.Tuple _
        | FSharpType.ThisType _ -> ()

        print printer tail

    | [] -> ()

let printFile (printer: Printer) (transformResult: Transform.TransformResult) =

    let outFile =
        {
            Name = "Glutinum"
            Opens = [ "Fable.Core"; "Fable.Core.JsInterop"; "System" ]
        }

    printer.Write($"module rec {outFile.Name}")
    printer.NewLine
    printer.NewLine

    outFile.Opens
    |> List.iter (fun o ->
        printer.Write($"open {o}")
        printer.NewLine
    )

    if transformResult.IncludeReadonlyArrayAlias then
        printer.NewLine

        printer.Write
            "// You need to add Glutinum.Types NuGet package to your project"

        printer.NewLine
        printer.Write "open Glutinum.Types.TypeScript"
        printer.NewLine

    if transformResult.IncludeRegExpAlias then
        printer.NewLine
        printer.Write "type RegExp = Text.RegularExpressions.Regex"
        printer.NewLine

    print printer transformResult.FSharpAST
