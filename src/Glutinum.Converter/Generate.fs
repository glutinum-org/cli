module rec Glutinum.Converter.Generate

open Fable.Core
open Node
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter

let createProgramForCLI (_fileName: string) (_source: string) : Ts.Program =
    importDefault "./js/bootstrap.js"

let generateBindingFile (filePath: string) =

    if fs.existsSync (U2.Case1 filePath) |> not then
        failwith $"File does not exist: {filePath}"

    let fileContent = fs.readFileSync filePath

    let program = createProgramForCLI filePath (fileContent.ToString())

    let checker = program.getTypeChecker ()

    let sourceFile = program.getSourceFile filePath

    let printer = new Printer.Printer()

    let readerResult = Read.readSourceFile checker sourceFile

    // Log reader warnings
    for warning in readerResult.Warnings do
        Log.warn warning

    let transformResult =
        Transform.apply readerResult.TypeMemory readerResult.GlueAST

    // Log transform warnings and errors
    for reporter in transformResult.Warnings do
        Log.warn reporter

    for reporter in transformResult.Errors do
        Log.error reporter

    Printer.printFile printer transformResult

    printer.ToString()
