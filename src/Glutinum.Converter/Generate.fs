module rec Glutinum.Converter.Generate

open Fable.Core
open Node
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter

let createProgramForCLI (_fileName: string) (_source: string) : Ts.Program =
    importDefault "./js/bootstrap.js"

type ResolvedInput =
    abstract kind: string
    abstract file: string
    abstract packageDir: string

type SubpathEntry =
    abstract subpath: string
    abstract file: string

[<AllowNullLiteral>]
type PackageDescription =
    abstract name: string
    abstract runtimeName: string
    abstract dir: string
    abstract entryFile: string
    abstract subpathEntries: SubpathEntry[]

[<Import("resolveInput", "./js/resolve.js")>]
let private resolveInput (_input: string) : ResolvedInput = jsNative

[<Import("describePackage", "./js/resolve.js")>]
let private describePackage (_packageDir: string) : PackageDescription = jsNative

[<Import("findPackageDir", "./js/resolve.js")>]
let private findPackageDir (_file: string) : string = jsNative

[<Import("listInstalledPackages", "./js/resolve.js")>]
let private listInstalledPackages () : string[] = jsNative

[<Import("createProgramFromFiles", "./js/bootstrap.js")>]
let private createProgramFromFiles (_entryFiles: string[]) : Ts.Program = jsNative

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

    let transformResult = Transform.apply readerResult.TypeMemory readerResult.GlueAST

    // Log transform warnings and errors
    for reporter in transformResult.Warnings do
        Log.warn reporter

    for reporter in transformResult.Errors do
        Log.error reporter

    Printer.printFile printer transformResult

    printer.ToString()

let private moduleNameForPackage (runtimeName: string) =
    runtimeName.Split([| '@'; '/'; '-'; '.'; '_' |], System.StringSplitOptions.RemoveEmptyEntries)
    |> Array.map (fun part -> string (System.Char.ToUpper part.[0]) + part.Substring(1))
    |> String.concat ""

let private toPackageInfo
    (isTarget: bool)
    (description: PackageDescription)
    : Reader.Types.PackageInfo
    =
    {
        ModuleName = moduleNameForPackage description.runtimeName
        RuntimeName = description.runtimeName
        Dir = String.normalizePath description.dir + "/"
        EntryFile = String.normalizePath description.entryFile
        SubpathEntries =
            description.subpathEntries
            |> Array.toList
            |> List.map (fun entry -> String.normalizePath entry.file, entry.subpath)
        IsTarget = isTarget
    }

let private isTypeScriptLibFile (fileName: string) =
    (String.normalizePath fileName).Contains "/typescript/lib/lib."

/// <summary>
/// Generate a single binding file for the packages, and the packages they depend on.
/// An empty list generates every package installed in the nearest <c>node_modules</c>.
/// </summary>
let generatePackages (inputs: string list) =
    let targetDirs =
        match inputs with
        | [] -> listInstalledPackages () |> Array.toList
        | inputs ->
            inputs
            |> List.map (fun input ->
                let resolved = resolveInput input

                match resolved.kind with
                | "package" -> resolved.packageDir
                | _ ->
                    match findPackageDir resolved.file with
                    | null -> failwith $"Could not find the package of {resolved.file}"
                    | packageDir -> packageDir
            )

    let targets =
        targetDirs
        |> List.map (fun dir ->
            match describePackage dir with
            | null -> failwith $"Could not find a declaration file for the package in {dir}"
            | description -> description
        )

    let program =
        targets
        |> List.collect (fun target ->
            target.entryFile :: (target.subpathEntries |> Array.toList |> List.map _.file)
        )
        |> List.toArray
        |> createProgramFromFiles

    let checker = program.getTypeChecker ()

    let targetDirs =
        targets |> List.map (fun target -> String.normalizePath target.dir) |> set

    let dependencies =
        program.getSourceFiles ()
        |> Seq.toList
        |> List.filter (fun sourceFile -> not (isTypeScriptLibFile sourceFile.fileName))
        |> List.choose (fun sourceFile ->
            match findPackageDir sourceFile.fileName with
            | null -> None
            | dir -> Some(String.normalizePath dir)
        )
        |> List.distinct
        |> List.filter (fun dir -> not (targetDirs.Contains dir))
        |> List.choose (fun dir ->
            match describePackage dir with
            | null -> None
            | description -> Some description
        )
        |> List.sortBy _.runtimeName

    let isSingleTarget = targets.Length = 1

    let packageContext: Reader.Types.PackageContext =
        {
            Packages =
                (targets |> List.map (toPackageInfo isSingleTarget))
                @ (dependencies |> List.map (toPackageInfo false))
        }

    let sourceFiles =
        program.getSourceFiles ()
        |> Seq.toList
        |> List.filter (fun sourceFile ->
            (packageContext.TryFindPackage sourceFile.fileName).IsSome
        )

    let readerResult = Read.readPackages checker packageContext sourceFiles

    for warning in readerResult.Warnings do
        Log.warn warning

    let importSpecifier =
        if isSingleTarget then
            targets.Head.runtimeName
        else
            Naming.MODULE_PLACEHOLDER

    let transformResult =
        Transform.applyWith importSpecifier readerResult.TypeMemory readerResult.GlueAST

    for reporter in transformResult.Warnings do
        Log.warn reporter

    for reporter in transformResult.Errors do
        Log.error reporter

    let printer = new Printer.Printer()

    Printer.printFile printer transformResult

    printer.ToString()
