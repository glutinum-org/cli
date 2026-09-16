module rec Glutinum.Converter.Generate

open Fable.Core
open Node
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter

let createProgramForCLI (_fileName: string) (_source: string) : Ts.Program =
    importDefault "./js/bootstrap.js"

[<Import("createNodeHost", "./js/host.js")>]
let private createNodeHost (_fs: obj, _path: obj, _cwd: string) : Packages.Host = jsNative

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
        Transform.applyWith
            (readerResult.ImportSpecifier |> Option.defaultValue Naming.MODULE_PLACEHOLDER)
            readerResult.TypeMemory
            readerResult.GlueAST

    // Log transform warnings and errors
    for reporter in transformResult.Warnings do
        Log.warn reporter

    for reporter in transformResult.Errors do
        Log.error reporter

    Printer.printFile printer transformResult

    printer.ToString()

/// <summary>
/// Generate a single binding file for the packages, and the packages they depend on.
/// An empty list generates every package installed in the nearest <c>node_modules</c>.
/// </summary>
let generatePackagesWith (host: Packages.Host) (inputs: string list) =
    let result = Packages.generate host inputs

    for warning in result.Warnings do
        Log.warn warning

    for error in result.Errors do
        Log.error error

    result.FSharpCode

/// The packages installed on the disk, from the current directory
let generatePackages (inputs: string list) =
    generatePackagesWith (createNodeHost (fs, path, ``process``.cwd ())) inputs
