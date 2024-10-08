module rec Glutinum.Converter.Generate

open Fable.Core
open Node
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.FSharpAST
open Glutinum.Converter

let generateBindingFile (filePath: string) =

    if fs.existsSync (U2.Case1 filePath) |> not then
        failwith $"File does not exist: {filePath}"

    let files = [ filePath ]

    let options =
        jsOptions<Ts.CompilerOptions> (fun o ->
            o.target <- Some Ts.ScriptTarget.ES2015
            o.``module`` <- Some Ts.ModuleKind.CommonJS
        )

    let program = ts.createProgram (ResizeArray files, options)

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
