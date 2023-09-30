module Glutinum.Converter.Printer

open System
open Glutinum.Chalk
open Glutinum.Converter.FSharpAST
open Fable.Core

type Printer () =
    let buffer = new Text.StringBuilder()
    let mutable indentationLevel = 0
    let indentationText = "    " // 4 spaces

    member __.Indent with get () =
        indentationLevel <- indentationLevel + 1

    member __.Unindent with get () =
        // Safety measure so we don't have negative indentation space
        indentationLevel <- System.Math.Max(indentationLevel + 1, 0)

    member __.Write(text : string) =
        buffer.Append(String.replicate indentationLevel indentationText + text)
        |> ignore

    member __.NewLine with get () =
        buffer.AppendLine()
        |> ignore

    override  __.ToString() =
        buffer.ToString()

let private removeSingleQuote (text : string) =
    text.Trim(''')

let private removeDoubleQuote (text : string) =
    text.Trim('"')

let private capitalizeFirstLetter (text : string) =
    (string text.[0]).ToUpper() + text.[1..]

let private lowercaseFirstLetter (text : string) =
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
let private nameEqualsDefaultFableValue (name: string) (value: string): bool =
    let defaultFableValue =
        lowercaseFirstLetter name

    defaultFableValue.Equals value

let printOutFile (printer : Printer) (outFile : FSharpOutFile) =
    match outFile.Name with
    | Some name ->
        printer.Write($"module {name} =")
        printer.NewLine
    | None ->
        ()

    outFile.Opens
    |> List.iter (fun o ->
        printer.Write($"open {o}")
        printer.NewLine
    )

    if outFile.Opens.Length > 0 then
        printer.NewLine

module Naming =
    let (|Digit|_|) (digit: string) =
        if String.IsNullOrWhiteSpace digit then None
        elif Char.IsDigit (digit, 0) then Some digit
        else None

let private sanitizeEnumCaseName (name : string) =
    let name =
        name
        |> removeSingleQuote
        |> removeDoubleQuote
        |> capitalizeFirstLetter

    match name with
    | Naming.Digit _ ->
        // F# enums cannot start with a digit, so we escape it with backticks
        $"``{name}``"
    | _ ->
        name

let private printEnum (printer : Printer) (enumInfo : FSharpEnum) =
    printer.Write("[<RequireQualifiedAccess>]")
    printer.NewLine

    match enumInfo.Type with
    | FSharpEnumType.String ->
        printer.Write("[<StringEnum>]")
        printer.NewLine
    | FSharpEnumType.Numeric
    | FSharpEnumType.Unknown ->
        ()

    printer.Write($"type {enumInfo.Name} =")
    printer.NewLine
    printer.Indent

    enumInfo.Cases
    |> List.iter (fun enumCaseInfo ->
        let enumCaseName =
            enumCaseInfo.Name
            |> sanitizeEnumCaseName

        match enumCaseInfo.Value with
        | FSharpLiteral.Int value ->
            printer.Write($"""| {enumCaseName} = %i{value}""")
        | FSharpLiteral.String value ->
            printfn $"enumCaseName: {enumCaseName} - value: {value}"
            printfn "%A" (nameEqualsDefaultFableValue enumCaseName value)
            if nameEqualsDefaultFableValue enumCaseName value then
                printer.Write($"""| {enumCaseName}""")
            else
                printer.Write($"""| [<CompiledName("{value}")>] {enumCaseName}""")

        | FSharpLiteral.Bool _ ->
            failwith $"""Boolean values are not supported inside of F# enums.

Errored enum: {enumInfo.Name}
"""
        | FSharpLiteral.Float value ->
            failwith $"""Float values are not supported inside of F# enums.

Errored enum: {enumInfo.Name}
"""

        printer.NewLine
    )

    printer.Unindent

let rec print (printer : Printer) (fsharpTypes : FSharpType list) =
    match fsharpTypes with
    | fsharpType::tail ->
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
                            value
                            |> removeSingleQuote
                            |> removeDoubleQuote

                        printer.Write($"""| [<CompiledName("{caseValue}")>] {enumCaseInfo.Name}""")
                    | FSharpUnionCaseType.Literal value ->
                        let caseValue =
                            value
                            |> removeSingleQuote
                            |> capitalizeFirstLetter

                        printer.Write($"""| {caseValue}""")

                    printer.NewLine
            )

            printer.Unindent

        | FSharpType.Enum enumInfo ->
            printEnum printer enumInfo

        | _ ->
            Log.warn $"{fsharpType} not implemented yet"

    | [] ->
        Log.success "Done"
