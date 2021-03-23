module Glutinum.Chalk

open Fable.Core
open System
open Node
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Chalk
open FSharp

let info = chalk.blueBright
let warn = chalk.yellowBright
let error = chalk.redBright
let success = chalk.greenBright

let log x = JS.console.log(x)

let files =
    [
        "tests/fixtures/stringEnum.d.ts"
    ]

let options = jsOptions<Ts.CompilerOptions>(fun o ->
    o.target <- Some Ts.ScriptTarget.ES2015
    o.``module`` <- Some Ts.ModuleKind.CommonJS
)

let program = ts.createProgram(ResizeArray files, options)

let checker = program.getTypeChecker()

let sourceFile = program.getSourceFile "tests/fixtures/stringEnum.d.ts"

let transformUnionType (checker: Ts.TypeChecker) (unionNode: Ts.UnionTypeNode) : FSharpType =
    let unionTypes = unionNode.types |> List.ofSeq


    let enumCases, restCases =
        unionTypes
        |> List.partition ts.isLiteralTypeNode

    let stringCases =
        enumCases
        |> List.filter (fun typeNode ->
            let literalNode = typeNode :?> Ts.LiteralTypeNode

            ts.isStringLiteral(unbox literalNode.literal)
        )
        // |> List.map (fun typeNode ->
        //     let literalNode = typeNode :?> Ts.LiteralTypeNode

        //     {
        //         Name = literalNode.getText()
        //         Type : FSharpEnumCaseType
        //         Value : string
        //     }
        // )


    FSharpType.Enum
        {
            Name = ""
            Cases = []
        }

let transformTypeNode (checker : Ts.TypeChecker) (typeNode : Ts.TypeNode) =
    match typeNode.kind with
    | Ts.SyntaxKind.UnionType ->
        let unionTypeNode = typeNode :?> Ts.UnionTypeNode

        transformUnionType checker unionTypeNode






    | unkownKind ->
        log(warn.Invoke($"Unsupported TypeNode kind: {unkownKind}"))
        FSharpType.Discard



let transformTypeAliasDeclaration (declaration : Ts.TypeAliasDeclaration) : FSharpType =
    let typeNode = transformTypeNode checker (declaration.``type``)

    match typeNode with
    | FSharpType.Enum enumInfo ->
        FSharpType.Enum
            {
                Name = declaration.name.getText()
                Cases = enumInfo.Cases
            }

    | _ ->
        failwith "Default case in type alias declaration"

let res =
    sourceFile.Value.statements
    |> List.ofSeq
    |> List.collect (fun statement ->
        match statement.kind with
        | Ts.SyntaxKind.TypeAliasDeclaration ->
            let declaration = (statement :?> Ts.TypeAliasDeclaration)

            match declaration.``type``.kind with
            | Ts.SyntaxKind.UnionType ->
                let unionTypeNode = declaration.``type`` :?> Ts.UnionTypeNode

                let literalCases =
                    unionTypeNode.types
                    |> List.ofSeq
                    |> List.filter ts.isLiteralTypeNode
                    |> List.map (fun node ->
                        let literalTypeNode = node :?> Ts.LiteralTypeNode

                        if ts.isStringLiteral (unbox literalTypeNode.literal) then
                            {
                                Name = literalTypeNode.literal?getText()
                                Type = FSharpEnumCaseType.String
                                Value = literalTypeNode.literal?getText()
                            }
                            |> Some

                        else if ts.isNumericLiteral (unbox literalTypeNode.literal) then
                            {
                                Name = literalTypeNode.literal?getText()
                                Type = FSharpEnumCaseType.Numeric
                                Value = literalTypeNode.literal?getText()
                            }
                            |> Some

                        else
                            log(error.Invoke($"Expected a StringLiteral or NumericLiteral"))
                            None
                    )
                    |> List.choose id

                [
                    FSharpType.Enum
                        {
                            Name = declaration.name.getText()
                            Cases = literalCases
                        }
                ]

            // let x = transformTypeAliasDeclaration (statement :?> Ts.TypeAliasDeclaration)

            // [ "" ]

            | _ ->
                log(error.Invoke("Default case in type alias declaration"))
                [ ]

        | _ ->
            printfn "Unsupported Statement kind: %A" statement.kind
            [ ]

    )

type Printer () =
    let buffer = new System.Text.StringBuilder()
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

    member this.NewLine with get () =
        this.Write("\n")

    override  __.ToString() =
        buffer.ToString()

let printer = new Printer()

let removeSingleQuote (text : string) =
    text.Trim(''')

let capitalizeFirstLetter (text : string) =
    (string text.[0]).ToUpper() + text.[1..]



let rec print (fsharpTypes : FSharpType list) =
    match fsharpTypes with
    | fsharpType::tail ->
        match fsharpType with
        | FSharpType.Enum enumInfo ->
            printer.Write("[<RequireQualifiedAccess>]")
            printer.NewLine

            printer.Write($"type {enumInfo.Name} =")
            printer.NewLine
            printer.Indent

            enumInfo.Cases
            |> List.iter (fun enumCaseInfo ->
                match enumCaseInfo.Type with
                | FSharpEnumCaseType.String ->
                    let caseValue =
                        enumCaseInfo.Value
                        |> removeSingleQuote
                        |> capitalizeFirstLetter

                    // TODO: Escape special case
                    printer.Write($"| {caseValue}")
                    printer.NewLine

                | FSharpEnumCaseType.Numeric ->
                    printer.Write($"| {enumCaseInfo.Value}")
                    printer.NewLine
            )

            printer.Unindent

        | _ ->
            log(warn.Invoke($"{fsharpType} not implemented yet"))

    | [] ->
        log(success.Invoke("Done"))

print res

log(printer.ToString())
