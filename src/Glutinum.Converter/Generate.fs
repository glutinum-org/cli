module Glutinum.Converter.Generate

open TypeScript
open Glutinum.Converter

let createProgramForCLI (fileName: string) (source: string) : Ts.Program =
    Hosting.Bootstrap.createProgramForCLI fileName source

/// Generate the binding of a single declaration file as the module <c>moduleName</c>
/// `isGlobal`: the file declares the globals of a script, nothing is imported
let private generateFile
    (moduleName: string)
    (isGlobal: bool)
    (maxOverloads: int)
    (filePath: string)
    =

    if Glutinum.Node.fs.Exports.existsSync filePath |> not then
        failwith $"File does not exist: {filePath}"

    let fileContent =
        Glutinum.Node.fs.Exports.readFileSync (filePath, Glutinum.Node.BufferEncoding.utf8)

    let program = createProgramForCLI filePath fileContent

    let checker = program.getTypeChecker ()

    let sourceFile = program.getSourceFile filePath

    let printer = new Printer.Printer()

    let readerResult = Read.readSourceFile checker sourceFile

    for warning in readerResult.Warnings do
        Log.warn warning

    let source =
        if isGlobal then
            Transformer.Context.ImportSource.Global
        else
            Transformer.Context.ImportSource.Module(
                readerResult.ImportSpecifier |> Option.defaultValue Naming.MODULE_PLACEHOLDER,
                Map.empty
            )

    let transformResult =
        Transform.applyWithOptions
            {
                Source = source
                MaxOverloads = maxOverloads
            }
            readerResult.TypeMemory
            readerResult.GlueAST

    for reporter in transformResult.Warnings do
        Log.warn reporter

    for reporter in transformResult.Errors do
        Log.error reporter

    Printer.printFileWith moduleName false [] printer transformResult

    printer.ToString()

let generateBindingFileWith (moduleName: string) (filePath: string) =
    generateFile moduleName false Transformer.UnionOverloads.defaultMaxOverloads filePath

/// `isGlobal`: the file declares the globals of a script, nothing is imported
let generateBindingFileWithOptions
    (moduleName: string)
    (isGlobal: bool)
    (maxOverloads: int)
    (filePath: string)
    =
    generateFile moduleName isGlobal maxOverloads filePath

let generateBindingFile (filePath: string) =
    generateBindingFileWith "Glutinum" filePath

let generatePackagesWithOptions
    (options: Packages.GenerateOptions)
    (host: Hosting.Host)
    (inputs: string list)
    =
    let result = Packages.generateWith options host inputs

    for warning in result.Warnings do
        Log.warn warning

    for error in result.Errors do
        Log.error error

    result.FSharpCode

/// <summary>
/// Generate a single binding file for the packages, and the packages they depend on.
/// An empty list generates every package installed in the nearest <c>node_modules</c>.
/// </summary>
let generatePackagesWith (host: Hosting.Host) (inputs: string list) =
    generatePackagesWithOptions Packages.defaultOptions host inputs

let generatePackagesFromDisk (options: Packages.GenerateOptions) (inputs: string list) =
    generatePackagesWithOptions
        options
        (Hosting.createNodeHost (Glutinum.Node.Exports.``process``.cwd ()))
        inputs

/// The packages installed on the disk, from the current directory
let generatePackages (inputs: string list) =
    generatePackagesFromDisk Packages.defaultOptions inputs
