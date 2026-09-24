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
    abstract hasRuntime: bool
    abstract dir: string
    abstract typesRoot: string
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
let private createProgramFromFiles
    (_host: Host, _entryFiles: string[], _options: {| withoutDomLib: bool; noLib: bool |})
    : Ts.Program
    =
    jsNative

/// The packages standing in for the DOM lib of TypeScript
let private domLibReplacements = set [ "@types/web"; "@typescript/lib-dom" ]

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
        HasRuntime = description.hasRuntime
        Dir = String.normalizePath description.dir + "/"
        TypesRoot = String.normalizePath description.typesRoot + "/"
        EntryFile = String.normalizePath description.entryFile
        SubpathEntries =
            description.subpathEntries
            |> Array.toList
            |> List.map (fun entry -> String.normalizePath entry.file, entry.subpath)
    }

let private isTypeScriptLibFile (fileName: string) =
    (String.normalizePath fileName).Contains "/typescript/lib/lib."

type GenerateOptions =
    {
        /// Reference `@types/node` and `@types/web` as the Glutinum.Node and Glutinum.Web
        /// bindings instead of generating them with the packages using them
        ExternalPackages: bool
        /// Other packages published as their own bindings: the package name and the module
        /// under `Glutinum`, derived from the package name when not given
        Externals: (string * string option) list
        /// The full name of the module of the generated package, `Glutinum.Types.TypeScript`,
        /// instead of `Glutinum.<Module>` derived from the package name
        ModuleName: string option
        /// The declarations of the generated package to keep, every one when empty
        Include: string list
        /// The package is the ES library itself, the program is created without it
        NoLib: bool
    }

let defaultOptions =
    {
        ExternalPackages = true
        Externals = []
        ModuleName = None
        Include = []
        NoLib = false
    }

/// `Glutinum.Types.TypeScript` is the namespace `Glutinum.Types` and the module `TypeScript`
let private splitModuleName (fullName: string) =
    match fullName.LastIndexOf '.' with
    | -1 -> "Glutinum", fullName
    | index -> fullName.Substring(0, index), fullName.Substring(index + 1)

let rec private declarationName (glueType: GlueAST.GlueType) =
    match glueType with
    | GlueAST.GlueType.Interface info -> Some info.Name
    | GlueAST.GlueType.ClassDeclaration info -> Some info.Name
    | GlueAST.GlueType.TypeAliasDeclaration info -> Some info.Name
    | GlueAST.GlueType.Enum info -> Some info.Name
    | GlueAST.GlueType.Variable info -> Some info.Name
    | GlueAST.GlueType.FunctionDeclaration info -> Some info.Name
    | GlueAST.GlueType.ExportDefault inner -> declarationName inner
    | _ -> None

let rec private isValue (glueType: GlueAST.GlueType) =
    match glueType with
    | GlueAST.GlueType.Variable _
    | GlueAST.GlueType.FunctionDeclaration _ -> true
    | GlueAST.GlueType.ExportDefault inner -> isValue inner
    | _ -> false

/// The declarations named in `include`, the modules holding them kept around them. A name
/// prefixed by `value:` keeps the variable or function only, `value:Number` keeps
/// `declare var Number` and not `interface Number`. A namespace named is kept whole.
let rec private keepIncluded (included: Set<string>) (types: GlueAST.GlueType list) =
    let values =
        included
        |> Set.filter (fun name -> name.StartsWith "value:")
        |> Set.map (fun name -> name.Substring "value:".Length)

    types
    |> List.choose (fun glueType ->
        match glueType with
        | GlueAST.GlueType.FileModule info ->
            match keepIncluded included info.Types with
            | [] -> None
            | kept -> Some(GlueAST.GlueType.FileModule { info with Types = kept })
        | GlueAST.GlueType.ModuleDeclaration info when included.Contains info.Name -> Some glueType
        | GlueAST.GlueType.ModuleDeclaration info ->
            match keepIncluded included info.Types with
            | [] -> None
            | kept -> Some(GlueAST.GlueType.ModuleDeclaration { info with Types = kept })
        | glueType ->
            match declarationName glueType with
            | Some name when included.Contains name -> Some glueType
            | Some name when values.Contains name && isValue glueType -> Some glueType
            | _ -> None
    )

/// The packages published as their own bindings, with the TypeScript lib files standing for them
let private builtInExternalPackageNames =
    [
        "@types/node", Some "Node", []
        "@types/web", Some "Web", [ "/typescript/lib/lib.dom" ]
    ]

/// <summary>
/// Generate a single binding file for the packages, and the packages they depend on.
/// An empty list generates every package installed in the nearest <c>node_modules</c>.
/// </summary>
let generateWith (options: GenerateOptions) (host: Host) (inputs: string list) : GenerationResult =
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
            // `date-fns date-fns/locale` is one package
            |> List.distinct

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

    let withoutDomLib =
        targets |> List.exists (fun target -> domLibReplacements.Contains target.name)

    let program =
        createProgramFromFiles (
            host,
            entryFiles,
            {|
                withoutDomLib = withoutDomLib
                noLib = options.NoLib
            |}
        )

    let checker = program.getTypeChecker ()

    let targetDirs =
        targets |> List.map (fun target -> String.normalizePath target.dir) |> set

    let describeReachable (excludedRuntimeNames: string list) =
        reachableFiles (host, program, entryFiles, List.toArray excludedRuntimeNames)
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

    let externalPackageNames =
        [
            if options.ExternalPackages then
                yield! builtInExternalPackageNames

            for (name, moduleName) in options.Externals do
                yield name, moduleName, []
        ]

    // A package asked for is generated, even when published as its own binding
    let externals: Reader.Types.ExternalPackage list =
        if externalPackageNames.IsEmpty then
            []
        else
            let reachable = describeReachable []

            externalPackageNames
            |> List.filter (fun (name, _, _) ->
                targets |> List.exists (fun target -> target.name = name) |> not
            )
            |> List.map (fun (name, moduleName, libFilePrefixes) ->
                let package =
                    reachable |> List.tryFind (fun description -> description.name = name)

                {
                    ModuleName =
                        match moduleName, package with
                        | Some moduleName, _ -> moduleName
                        | None, Some package -> moduleNameForPackage package.runtimeName
                        | None, None -> moduleNameForPackage (name.Replace("@types/", ""))
                    Package = package |> Option.map toPackageInfo
                    LibFilePrefixes = libFilePrefixes
                }
            )

    let externalRuntimeNames =
        externals
        |> List.choose (fun external -> external.Package |> Option.map _.RuntimeName)

    // The packages only reachable through an external one are not needed either
    let dependencies =
        describeReachable externalRuntimeNames
        |> List.filter (fun description ->
            not (List.contains description.runtimeName externalRuntimeNames)
        )
        |> List.sortBy _.runtimeName

    let namespace_, moduleName =
        match options.ModuleName with
        | Some fullName -> splitModuleName fullName
        | None -> "Glutinum", ""

    let targetPackages =
        targets
        |> List.map toPackageInfo
        |> List.map (fun package ->
            if moduleName = "" then
                package
            else
                { package with ModuleName = moduleName }
        )

    let included = set options.Include

    let packageContext: Reader.Types.PackageContext =
        {
            Packages = targetPackages @ (dependencies |> List.map toPackageInfo)
            Externals = externals
            // Without the library, the declarations left out of the package stand for it
            IsLibraryName =
                fun name -> options.NoLib && not included.IsEmpty && not (included.Contains name)
        }

    let sourceFiles =
        program.getSourceFiles ()
        |> Seq.toList
        |> List.filter (fun sourceFile ->
            (packageContext.TryFindPackage sourceFile.fileName).IsSome
        )

    let readerResult = Read.readPackages checker packageContext sourceFiles

    let glueAst =
        match options.Include with
        | [] -> readerResult.GlueAST
        | included -> keepIncluded (set included) readerResult.GlueAST

    // Every package is a module with its own import specifier
    let transformResult =
        Transform.applyWith Naming.MODULE_PLACEHOLDER readerResult.TypeMemory glueAst

    let printer = new Printer.Printer()

    Printer.printFileWith
        namespace_
        true
        (externals |> List.map _.ModuleName)
        printer
        transformResult

    {
        GlueAST = readerResult.GlueAST
        FSharpAST = transformResult.FSharpAST
        FSharpCode = printer.ToString()
        Warnings = [ yield! readerResult.Warnings; yield! transformResult.Warnings ]
        Errors = transformResult.Errors |> Seq.toList
    }

let generate (host: Host) (inputs: string list) : GenerationResult =
    generateWith defaultOptions host inputs
