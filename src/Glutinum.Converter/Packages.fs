module Glutinum.Converter.Packages

open Fable.Core
open Fable.Core.JsInterop
open TypeScript
open TsMorph
open Glutinum.Converter

open Glutinum.Converter.Hosting
open Glutinum.Converter.Reader.Utils

/// Re-exported so consumers keep referring to them as `Packages.<Type>`
type Host = Hosting.Host
type InMemoryHost = Hosting.InMemoryHost
type ResolvedInput = Hosting.ResolvedInput
type SubpathEntry = Hosting.SubpathEntry
type PackageDescription = Hosting.PackageDescription
type InstalledPackage = Hosting.InstalledPackage

type GenerationResult =
    {
        GlueAST: GlueAST.GlueType list
        FSharpAST: FSharpAST.FSharpType list
        FSharpCode: string
        Warnings: string list
        Errors: string list
    }

let createInMemoryHost (cwd: string) : InMemoryHost = Hosting.createInMemoryHost cwd

/// <summary>
/// Download the declaration files of a package, and of the packages it depends on,
/// from jsDelivr into <c>/node_modules</c> of the file system.
/// </summary>
let installPackage
    (fileSystem: FileSystemHost, spec: string, options: {| onProgress: string -> unit |})
    : JS.Promise<InstalledPackage>
    =
    Hosting.Npm.installPackage fileSystem spec options

let private resolveInput (host: Host, input: string) : ResolvedInput =
    Hosting.Resolve.resolveInput host input

let private describePackage (host: Host, packageDir: string) : PackageDescription =
    Hosting.Resolve.describePackage host packageDir |> Option.toObj

let private findPackageDir (host: Host, file: string) : string =
    Hosting.Resolve.findPackageDir host file |> Option.toObj

let private listInstalledPackages (host: Host) : string[] =
    Hosting.Resolve.listInstalledPackages host

let private createProgramFromFiles
    (host: Host, entryFiles: string[], options: {| withoutDomLib: bool; noLib: bool |})
    : Ts.Program
    =
    Hosting.Bootstrap.createProgramFromFiles host entryFiles options.withoutDomLib options.noLib

let private reachableFiles
    (host: Host, program: Ts.Program, entryFiles: string[], excludedRuntimeNames: string[])
    : string[]
    =
    Hosting.Bootstrap.reachableFiles host program entryFiles excludedRuntimeNames

/// The packages standing in for the DOM lib of TypeScript
let private domLibReplacements = set [ "@types/web"; "@typescript/lib-dom" ]

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
        ReExportedSymbols = Map.empty
        ReExportedFiles = Map.empty
        ReExportedNames = Map.empty
        HasExportsMap = false
        HoistableDeclarations = Set.empty
    }

/// The names a public entry of the package exports, with the subpath to import each from
let private reExportedNames
    (program: Ts.Program)
    (checker: Ts.TypeChecker)
    (hasExportsMap: bool)
    (package: Reader.Types.PackageInfo)
    : Map<string, string>
    =
    if not hasExportsMap then
        Map.empty
    else

        ((Map.empty, (package.EntryFile, "") :: package.SubpathEntries)
         ||> List.fold (fun acc (entryFile, subpath) ->
             match program.getSourceFile entryFile with
             | None -> acc
             | Some sourceFile ->
                 match checker.getSymbolAtLocation (unbox<Ts.Node> sourceFile) with
                 | None -> acc
                 | Some moduleSymbol ->
                     (acc, checker.getExportsOfModule moduleSymbol)
                     ||> Seq.fold (fun acc exportedSymbol ->
                         if acc |> Map.containsKey exportedSymbol.name then
                             acc
                         else
                             acc.Add(exportedSymbol.name, subpath)
                     )
         ))

/// The declarations a public entry of the package exports, keyed by the declaring file and the
/// exported name, with the subpath to import each from
let private reExportedSymbols
    (program: Ts.Program)
    (checker: Ts.TypeChecker)
    (hasExportsMap: bool)
    (package: Reader.Types.PackageInfo)
    : Map<string * string, string>
    =
    if not hasExportsMap then
        Map.empty
    else

        let entries = (package.EntryFile, "") :: package.SubpathEntries
        let entryFiles = entries |> List.map fst |> set

        (Map.empty, entries)
        ||> List.fold (fun acc (entryFile, subpath) ->
            match program.getSourceFile entryFile with
            | None -> acc
            | Some sourceFile ->
                match checker.getSymbolAtLocation (unbox<Ts.Node> sourceFile) with
                | None -> acc
                | Some moduleSymbol ->
                    (acc, checker.getExportsOfModule moduleSymbol)
                    ||> Seq.fold (fun acc exportedSymbol ->
                        // `export { x as y }` publishes the declaration under the name of the alias
                        let exportName = exportedSymbol.name

                        // `export { x } from "./x.js"` exports an alias, the declaration is behind it
                        let exportedSymbol =
                            match exportedSymbol.flags with
                            | HasSymbolFlags Ts.SymbolFlags.Alias ->
                                checker.getAliasedSymbol exportedSymbol
                            | _ -> exportedSymbol

                        match exportedSymbol.declarations with
                        | None -> acc
                        | Some declarations ->
                            (acc, declarations)
                            ||> Seq.fold (fun acc declaration ->
                                let declaringFile =
                                    (unbox<Ts.Node> declaration).getSourceFile().fileName

                                let key = String.normalizePath declaringFile, exportName

                                if
                                    (fst key).StartsWith package.Dir
                                    && not (entryFiles.Contains(fst key))
                                    && not (acc.ContainsKey key)
                                then
                                    acc.Add(key, subpath)
                                else
                                    acc
                            )
                    )
        )

/// The names a file declares itself, the ones it re-exports belong to another file
let private declaredNames (program: Ts.Program) (checker: Ts.TypeChecker) (fileName: string) =
    match program.getSourceFile fileName with
    | None -> Set.empty
    | Some sourceFile ->
        match checker.getSymbolAtLocation (unbox<Ts.Node> sourceFile) with
        | None -> Set.empty
        | Some moduleSymbol ->
            let normalized = String.normalizePath fileName

            checker.getExportsOfModule moduleSymbol
            // A namespace stays in the module of its file, the reader keeps it there
            |> Seq.filter (fun exportedSymbol ->
                match exportedSymbol.flags with
                | HasSymbolFlags Ts.SymbolFlags.ValueModule
                | HasSymbolFlags Ts.SymbolFlags.NamespaceModule -> false
                | _ -> true
            )
            |> Seq.filter (fun exportedSymbol ->
                match exportedSymbol.declarations with
                | Some declarations ->
                    declarations
                    |> Seq.exists (fun declaration ->
                        // `export { alpha } from "./shared.js"` is a specifier, not a declaration
                        declaration.kind <> Ts.SyntaxKind.ExportSpecifier
                        && declaration.kind <> Ts.SyntaxKind.ExportAssignment
                        && String.normalizePath (
                            (unbox<Ts.Node> declaration).getSourceFile().fileName
                        )
                            =
                            normalized
                    )
                | None -> false
            )
            // `export default class Element` is exported as `default`, the class carries the name
            |> Seq.map (fun exportedSymbol ->
                if exportedSymbol.name = "default" then
                    exportedSymbol.declarations
                    |> Option.bind (
                        Seq.tryPick (fun declaration ->
                            match declaration?name with
                            | null -> None
                            | name -> Some(name?getText (): string)
                        )
                    )
                    |> Option.defaultValue exportedSymbol.name
                else
                    exportedSymbol.name
            )
            |> Set.ofSeq

/// A declaration the root entry exports is read into the package module, unless its name is
/// taken by the entry or by a file read before it
let private hoistableDeclarations
    (program: Ts.Program)
    (checker: Ts.TypeChecker)
    (package: Reader.Types.PackageInfo)
    (reExportedFiles: Map<string, string>)
    =
    let entryNames = declaredNames program checker package.EntryFile

    let candidates =
        reExportedFiles
        |> Map.toList
        |> List.filter (fun (file, subpath) ->
            subpath = ""
            && file <> package.EntryFile
            && not (package.SubpathEntries |> List.exists (fun (entry, _) -> entry = file))
        )
        |> List.map fst
        |> List.sort

    ((entryNames, Set.empty), candidates)
    ||> List.fold (fun (taken, hoistable) file ->
        let free = Set.difference (declaredNames program checker file) taken

        Set.union taken free, Set.union hoistable (free |> Set.map (fun name -> file, name))
    )
    |> snd

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

    let exportsMapPackages =
        (targets @ dependencies)
        |> List.filter (fun description -> description.hasExportsMap)
        |> List.map (fun description -> String.normalizePath description.dir + "/")
        |> set

    let withReExportedFiles (package: Reader.Types.PackageInfo) =
        let hasExportsMap = exportsMapPackages.Contains package.Dir
        let symbols = reExportedSymbols program checker hasExportsMap package

        let reExportedFiles =
            symbols
            |> Map.toList
            |> List.fold
                (fun acc ((file, _), subpath) ->
                    if Map.containsKey file acc then
                        acc
                    else
                        Map.add file subpath acc
                )
                Map.empty

        { package with
            HasExportsMap = hasExportsMap
            HoistableDeclarations =
                if hasExportsMap then
                    hoistableDeclarations program checker package reExportedFiles
                else
                    Set.empty
            ReExportedSymbols = symbols
            ReExportedNames = reExportedNames program checker hasExportsMap package
            ReExportedFiles =
                symbols
                |> Map.toList
                |> List.fold
                    (fun acc ((file, _), subpath) ->
                        if Map.containsKey file acc then
                            acc
                        else
                            Map.add file subpath acc
                    )
                    Map.empty
        }

    let packageContext: Reader.Types.PackageContext =
        {
            Packages =
                targetPackages @ (dependencies |> List.map toPackageInfo)
                |> List.map withReExportedFiles
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
