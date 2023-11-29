module Glutinum.Converter.Printer

open System
open Glutinum.Chalk
open Glutinum.Converter.FSharpAST
open Fable.Core

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

    override __.ToString() = buffer.ToString()

let printOutFile (printer: Printer) (outFile: FSharpOutFile) =
    printer.Write($"module rec {outFile.Name}")
    printer.NewLine
    printer.NewLine

    outFile.Opens
    |> List.iter (fun o ->
        printer.Write($"open {o}")
        printer.NewLine
    )

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

let attributeToText (fsharpAttribute: FSharpAttribute) =
    match fsharpAttribute with
    | FSharpAttribute.Text text -> text
    | FSharpAttribute.EmitSelfInvoke -> "[<Emit(\"$0($1...)\")>]"
    | FSharpAttribute.Import(name, module_) ->
        $"[<Import(\"{name}\", \"{module_}\")>]"
    | FSharpAttribute.Erase -> "[<Erase>]"
    | FSharpAttribute.AllowNullLiteral -> "[<AllowNullLiteral>]"
    | FSharpAttribute.StringEnum -> "[<StringEnum>]"
    | FSharpAttribute.CompiledName name -> $"[<CompiledName(\"{name}\")>]"
    | FSharpAttribute.RequireQualifiedAccess -> "[<RequireQualifiedAccess>]"
    | FSharpAttribute.EmitConstructor -> "[<EmitConstructor>]"
    | FSharpAttribute.EmitMacroConstructor className ->
        $"[<Emit(\"new $0.{className}($1...)\")>]"
    | FSharpAttribute.ImportAll module_ -> $"[<ImportAll(\"{module_}\")>]"

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

let printAttributes
    (printer: Printer)
    (fsharpAttributes: FSharpAttribute list)
    =
    fsharpAttributes
    |> List.iter (fun fsharpAttribute ->
        printer.Write(attributeToText fsharpAttribute)
        printer.NewLine
    )

let rec private printType (fsharpType: FSharpType) =
    match fsharpType with
    | FSharpType.Mapped info -> info.Name
    | FSharpType.Union info ->
        let cases =
            info.Cases |> List.map (fun c -> c.Name) |> String.concat ", "

        let option =
            if info.IsOptional then
                " option"
            else
                ""

        $"{info.Name}<{cases}>{option}"

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
        typeReference.FullName
    | FSharpType.Option optionType ->
        printType optionType + " option"
    | FSharpType.Module _
    | FSharpType.Interface _
    | FSharpType.Unsupported _
    | FSharpType.Alias _
    | FSharpType.Discard -> "obj"

let private printInterface (printer: Printer) (interfaceInfo: FSharpInterface) =
    printAttributes printer interfaceInfo.Attributes

    printer.Write($"type {interfaceInfo.Name} =")
    printer.NewLine

    printer.Indent

    interfaceInfo.Members
    |> List.iter (
        function
        // TODO: Rewrite the code below to share more code
        // Right now there are a lots of duplication and special rules
        // Can these rules be represented in the AST to simplify the code?
        | FSharpMember.Method methodInfo ->
            printCompactAttributesAndNewLine printer methodInfo.Attributes

            if methodInfo.IsStatic then
                printer.Write("static ")
            else
                printer.Write("abstract ")

            printer.WriteInline($"member {methodInfo.Name}")

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

                        if p.IsOptional then
                            printer.WriteInline("?")

                        printer.WriteInline($"{p.Name}: {printType p.Type}")
                    )

                    printer.WriteInline(")")
            else
                printer.WriteInline(": ")

                methodInfo.Parameters
                |> List.iteri (fun index p ->
                    if index <> 0 then
                        printer.WriteInline(" -> ")

                    printer.WriteInline($"{p.Name}: {printType p.Type}")
                )

            if methodInfo.IsStatic then
                printer.WriteInline(" : ")
            else
                printer.WriteInline(" -> ")

            printer.WriteInline(printType methodInfo.Type)

            if methodInfo.IsStatic then
                printer.WriteInline(" = nativeOnly")

            printer.NewLine

        | FSharpMember.Property propertyInfo ->

            printAttributes printer propertyInfo.Attributes

            if propertyInfo.IsStatic then
                printer.Write("static ")
            else
                printer.Write("abstract ")

            printer.WriteInline($"member {propertyInfo.Name}")

            if propertyInfo.IsStatic then

                propertyInfo.Accessor
                |> Option.map (
                    function
                    | FSharpAccessor.ReadOnly -> " with get () "
                    | FSharpAccessor.WriteOnly -> failwithf " with set "
                    | FSharpAccessor.ReadWrite -> failwithf " with get, set "
                )
                |> Option.iter printer.WriteInline

                printer.WriteInline($": {printType propertyInfo.Type}")

                printer.WriteInline(" = nativeOnly")

            else
                printer.WriteInline($": {printType propertyInfo.Type}")

                propertyInfo.Accessor
                |> Option.map (
                    function
                    | FSharpAccessor.ReadOnly -> " with get"
                    | FSharpAccessor.WriteOnly -> " with set"
                    | FSharpAccessor.ReadWrite -> " with get, set"
                )
                |> Option.iter printer.WriteInline

            printer.NewLine
    )

    if interfaceInfo.Members.Length = 0 then
        printer.Write("interface end")
        printer.NewLine

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
        | FSharpLiteral.Float _ -> ()

        printer.NewLine
    )

    printer.Unindent

let rec print (printer: Printer) (fsharpTypes: FSharpType list) =
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

                printInlineAttributes printer enumCaseInfo.Attributes

                printer.WriteInline(enumCaseInfo.Name)

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
                printer.Write($"rec ")

            printer.Write($"{moduleInfo.Name} =")
            printer.NewLine

            printer.Indent
            print printer moduleInfo.Types

        // TODO: Make print return the tail
        // Allowing module to eat they content and be able to unindent?
        // print printer tail

        // printer.Unindent

        | FSharpType.Alias aliasInfo ->
            printer.Write($"type {aliasInfo.Name} =")
            printer.NewLine
            printer.Indent
            printer.Write(printType aliasInfo.Type)
            printer.NewLine
            printer.Unindent

        | FSharpType.Mapped _
        | FSharpType.Primitive _
        | FSharpType.TypeReference _
        | FSharpType.Option _
        | FSharpType.Discard -> ()

        print printer tail

    | [] -> Log.success "Done"
