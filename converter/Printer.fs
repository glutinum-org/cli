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

let private removeSingleQuote (text: string) = text.Trim(''')

let private removeDoubleQuote (text: string) = text.Trim('"')

let private capitalizeFirstLetter (text: string) =
    (string text.[0]).ToUpper() + text.[1..]

let private lowercaseFirstLetter (text: string) =
    (string text.[0]).ToLower() + text.[1..]

/// <summary>
/// Check if the value is the same as the default Fable computed value computed from the name.
/// </summary>
/// <param name="name">Name to be transformed by Fable</param>
/// <param name="value">Value to compare to</param>
/// <returns>
/// <c>true</c> if provided value is the same as the default Fable computed value computed from the name.
/// <c>false</c> otherwise
/// </returns>
let private nameEqualsDefaultFableValue (name: string) (value: string) : bool =
    let defaultFableValue = lowercaseFirstLetter name

    defaultFableValue.Equals value

let printOutFile (printer: Printer) (outFile: FSharpOutFile) =
    match outFile.Name with
    | Some name ->
        printer.Write($"module {name} =")
        printer.NewLine
    | None -> ()

    outFile.Opens
    |> List.iter (fun o ->
        printer.Write($"open {o}")
        printer.NewLine
    )

module Naming =
    let (|Digit|_|) (digit: string) =
        if String.IsNullOrWhiteSpace digit then None
        elif Char.IsDigit(digit, 0) then Some digit
        else None

let private sanitizeEnumCaseName (name: string) =
    let name =
        name |> removeSingleQuote |> removeDoubleQuote |> capitalizeFirstLetter

    match name with
    | Naming.Digit _ ->
        // F# enums cannot start with a digit, so we escape it with backticks
        $"``{name}``"
    | _ -> name

let printAttribute (printer : Printer) (fsharpAttribute : FSharpAttribute) =
    match fsharpAttribute with
    | FSharpAttribute.Text text ->
        printer.Write(text)
    | FSharpAttribute.EmitSelfInvoke ->
        printer.Write("[<Emit(\"$0($1...)\")>]")
    | FSharpAttribute.Import (name, module_) ->
        printer.Write($"[<Import(\"{name}\", \"{module_}\")>]")
    | FSharpAttribute.Erase ->
        printer.Write("[<Erase>]")
    | FSharpAttribute.AllowNullLiteral ->
        printer.Write("[<AllowNullLiteral>]")

let printAttributes (printer : Printer) (fsharpAttributes : FSharpAttribute list) =
    fsharpAttributes
    |> List.iter (printAttribute printer)

let private printType (fsharpType : FSharpType) =
    printfn "printType: %A" fsharpType
    match fsharpType with
    | FSharpType.Mapped info ->
        info.Name
    | FSharpType.Union info ->
        info.Name
    | FSharpType.Enum info ->
        info.Name
    | _ ->
        "obj"

let private printInterface (printer: Printer) (interfaceInfo: FSharpInterface) =
    if interfaceInfo.Attributes.Length > 0 then
        printAttributes printer interfaceInfo.Attributes
        printer.NewLine

    printer.Write($"type {interfaceInfo.Name} =")
    printer.NewLine

    printer.Indent

    interfaceInfo.Members
    |> List.iter (fun m ->

        if m.Attributes.Length > 0 then
            printAttributes printer m.Attributes
            printer.NewLine

        if m.IsStatic then
            printer.Write("static ")
        else
            printer.Write("abstract ")

        printer.WriteInline($"member {m.Name}: ")

        m.Parameters
        |> List.iteri (fun index p ->
            if index <> 0 then
                printer.WriteInline(" -> ")

            printer.WriteInline($"{p.Name}: {printType p.Type}")
        )

        if m.Parameters.Length > 0 then
            printer.WriteInline(" -> ")

        printer.WriteInline(printType m.Type)

        if m.IsStatic then
            printer.WriteInline(" = nativeOnly")

        m.Accessor
        |> Option.map (
            function
            | FSharpAccessor.ReadOnly -> " with get"
            | FSharpAccessor.WriteOnly -> " with set"
            | FSharpAccessor.ReadWrite -> " with get, set"
        )
        |> Option.iter printer.WriteInline

        printer.NewLine
    )

    printer.Unindent

let private printEnum (printer: Printer) (enumInfo: FSharpEnum) =
    printer.Write("[<RequireQualifiedAccess>]")
    printer.NewLine

    match enumInfo.Type with
    | FSharpEnumType.String ->
        printer.Write("[<StringEnum>]")
        printer.NewLine
    | FSharpEnumType.Numeric
    | FSharpEnumType.Unknown -> ()

    printer.Write($"type {enumInfo.Name} =")
    printer.NewLine
    printer.Indent

    enumInfo.Cases
    |> List.iter (fun enumCaseInfo ->
        let enumCaseName = enumCaseInfo.Name |> sanitizeEnumCaseName

        match enumCaseInfo.Value with
        | FSharpLiteral.Int value ->
            printer.Write($"""| {enumCaseName} = %i{value}""")
        | FSharpLiteral.String value ->
            if nameEqualsDefaultFableValue enumCaseName value then
                printer.Write($"""| {enumCaseName}""")
            else
                printer.Write(
                    $"""| [<CompiledName("{value}")>] {enumCaseName}"""
                )

        | FSharpLiteral.Bool _ ->
            failwith
                $"""Boolean values are not supported inside of F# enums.

Errored enum: {enumInfo.Name}
"""
        | FSharpLiteral.Float value ->
            failwith
                $"""Float values are not supported inside of F# enums.

Errored enum: {enumInfo.Name}
"""

        printer.NewLine
    )

    printer.Unindent

let rec print (printer: Printer) (fsharpTypes: FSharpType list) =
    match fsharpTypes with
    | fsharpType :: tail ->
        printer.NewLine

        match fsharpType with
        | FSharpType.Union unionInfo ->
            printer.Write("[<RequireQualifiedAccess>]")
            printer.NewLine

            printer.Write("[<StringEnum>]")
            printer.NewLine

            printer.Write($"type {unionInfo.Name} =")
            printer.NewLine
            printer.Indent

            unionInfo.Cases
            |> List.iter (fun enumCaseInfo ->
                match enumCaseInfo.Value with
                | FSharpUnionCaseType.Named value ->
                    let caseValue =
                        value |> removeSingleQuote |> removeDoubleQuote

                    printer.Write(
                        $"""| [<CompiledName("{caseValue}")>] {enumCaseInfo.Name}"""
                    )
                | FSharpUnionCaseType.Literal value ->
                    let caseValue =
                        value |> removeSingleQuote |> capitalizeFirstLetter

                    printer.Write($"""| {caseValue}""")

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
            printer.Write($"module {moduleInfo.Name} =")
            printer.NewLine

            printer.Indent

        // TODO: Make print return the tail
        // Allowing module to eat they content and be able to unindent?
        // print printer tail

        // printer.Unindent

        | FSharpType.Mapped _
        | FSharpType.Discard -> ()

        print printer tail

    | [] -> Log.success "Done"
