module Glutinum.Converter.Packages

open Fable.Core
open TypeScript
open Glutinum.Converter

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

/// The file system the packages are read from: the disk for the CLI, an in-memory one in the browser
[<AllowNullLiteral>]
type Host =
    abstract cwd: string

/// A host over a ts-morph in-memory file system
[<AllowNullLiteral>]
type InMemoryHost =
    inherit Host
    abstract fileSystem: obj

[<AllowNullLiteral>]
type InstalledPackage =
    abstract name: string
    abstract version: string

type GenerationResult =
    {
        GlueAST: GlueAST.GlueType list
        FSharpAST: FSharpAST.FSharpType list
        FSharpCode: string
        Warnings: string list
        Errors: string list
    }

[<Import("createInMemoryHost", "./js/host.js")>]
let createInMemoryHost (_cwd: string) : InMemoryHost = jsNative

/// <summary>
/// Download the declaration files of a package, and of the packages it depends on,
/// from jsDelivr into <c>/node_modules</c> of the file system.
/// </summary>
[<Import("installPackage", "./js/npm.js")>]
let installPackage
    (_fileSystem: obj, _spec: string, _options: {| onProgress: string -> unit |})
    : JS.Promise<InstalledPackage>
    =
    jsNative

[<Import("resolveInput", "./js/resolve.js")>]
let private resolveInput (_host: Host, _input: string) : ResolvedInput = jsNative

[<Import("describePackage", "./js/resolve.js")>]
let private describePackage (_host: Host, _packageDir: string) : PackageDescription = jsNative

[<Import("findPackageDir", "./js/resolve.js")>]
let private findPackageDir (_host: Host, _file: string) : string = jsNative

[<Import("listInstalledPackages", "./js/resolve.js")>]
let private listInstalledPackages (_host: Host) : string[] = jsNative

[<Import("createProgramFromFiles", "./js/bootstrap.js")>]
let private createProgramFromFiles (_host: Host, _entryFiles: string[]) : Ts.Program = jsNative

[<Import("reachableFiles", "./js/bootstrap.js")>]
let private reachableFiles
    (_host: Host, _program: Ts.Program, _entryFiles: string[], _excludedRuntimeNames: string[])
    : string[]
    =
    jsNative

let private moduleNameForPackage (runtimeName: string) =
    runtimeName.Split([| '@'; '/'; '-'; '.'; '_' |], System.StringSplitOptions.RemoveEmptyEntries)
    |> Array.map (fun part -> string (System.Char.ToUpper part.[0]) + part.Substring(1))
    |> String.concat ""

let private toPackageInfo (description: PackageDescription) : Reader.Types.PackageInfo =
    {
        ModuleName = moduleNameForPackage description.runtimeName
        RuntimeName = description.runtimeName
        Dir = String.normalizePath description.dir + "/"
        EntryFile = String.normalizePath description.entryFile
        SubpathEntries =
            description.subpathEntries
            |> Array.toList
            |> List.map (fun entry -> String.normalizePath entry.file, entry.subpath)
    }

let private isTypeScriptLibFile (fileName: string) =
    (String.normalizePath fileName).Contains "/typescript/lib/lib."

/// <summary>
/// Generate a single binding file for the packages, and the packages they depend on.
/// An empty list generates every package installed in the nearest <c>node_modules</c>.
/// </summary>
let generate (host: Host) (inputs: string list) : GenerationResult =
    let targetDirs =
        match inputs with
        | [] -> listInstalledPackages host |> Array.toList
        | inputs ->
            inputs
            |> List.map (fun input ->
                let resolved = resolveInput (host, input)

                match resolved.kind with
                | "package" -> resolved.packageDir
                | _ ->
                    match findPackageDir (host, resolved.file) with
                    | null -> failwith $"Could not find the package of {resolved.file}"
                    | packageDir -> packageDir
            )

    let targets =
        targetDirs
        |> List.map (fun dir ->
            match describePackage (host, dir) with
            | null -> failwith $"Could not find a declaration file for the package in {dir}"
            | description -> description
        )

    let entryFiles =
        targets
        |> List.collect (fun target ->
            target.entryFile :: (target.subpathEntries |> Array.toList |> List.map _.file)
        )
        |> List.toArray

    let program = createProgramFromFiles (host, entryFiles)

    let checker = program.getTypeChecker ()

    let targetDirs =
        targets |> List.map (fun target -> String.normalizePath target.dir) |> set

    // `@types/node` describes the runtime like `lib.dom.d.ts`, it is generated on request only
    let dependencies =
        reachableFiles (host, program, entryFiles, [| "node" |])
        |> Array.toList
        |> List.filter (fun fileName -> not (isTypeScriptLibFile fileName))
        |> List.choose (fun fileName ->
            match findPackageDir (host, fileName) with
            | null -> None
            | dir -> Some(String.normalizePath dir)
        )
        |> List.distinct
        |> List.filter (fun dir -> not (targetDirs.Contains dir))
        |> List.choose (fun dir ->
            match describePackage (host, dir) with
            | null -> None
            | description -> Some description
        )
        |> List.filter (fun description -> description.runtimeName <> "node")
        |> List.sortBy _.runtimeName

    let packageContext: Reader.Types.PackageContext =
        {
            Packages =
                (targets |> List.map toPackageInfo) @ (dependencies |> List.map toPackageInfo)
        }

    let sourceFiles =
        program.getSourceFiles ()
        |> Seq.toList
        |> List.filter (fun sourceFile ->
            (packageContext.TryFindPackage sourceFile.fileName).IsSome
        )

    let readerResult = Read.readPackages checker packageContext sourceFiles

    // Every package is a module with its own import specifier
    let transformResult =
        Transform.applyWith Naming.MODULE_PLACEHOLDER readerResult.TypeMemory readerResult.GlueAST

    let printer = new Printer.Printer()

    Printer.printFile printer transformResult

    {
        GlueAST = readerResult.GlueAST
        FSharpAST = transformResult.FSharpAST
        FSharpCode = printer.ToString()
        Warnings = [ yield! readerResult.Warnings; yield! transformResult.Warnings ]
        Errors = transformResult.Errors |> Seq.toList
    }
