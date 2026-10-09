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
        DefaultExportNames = Map.empty
        HasExportsMap = false
        HoistableDeclarations = Set.empty
        AmbientModuleFiles = Map.empty
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

/// The declaration `export default` of the file points to
let private defaultExportOf (program: Ts.Program) (checker: Ts.TypeChecker) (fileName: string) =
    program.getSourceFile fileName
    |> Option.bind (fun sourceFile -> checker.getSymbolAtLocation (unbox<Ts.Node> sourceFile))
    |> Option.bind (fun moduleSymbol ->
        checker.getExportsOfModule moduleSymbol
        |> Seq.tryFind (fun exported -> exported.name = "default")
    )
    |> Option.map (fun symbol -> resolveAlias checker symbol |> Option.defaultValue symbol)

/// The declarations a public entry of the package exports, keyed by the declaring file and the
/// exported name, with the subpath to import each from, and the name each entry exports the
/// default export of a file under
let private reExportedSymbols
    (program: Ts.Program)
    (checker: Ts.TypeChecker)
    (hasExportsMap: bool)
    (package: Reader.Types.PackageInfo)
    : Map<string * string, string> * Map<string, string>
    =
    if not hasExportsMap then
        Map.empty, Map.empty
    else

        let entries = (package.EntryFile, "") :: package.SubpathEntries
        let entryFiles = entries |> List.map fst |> set

        ((Map.empty, Map.empty), entries)
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
                            resolveAlias checker exportedSymbol
                            |> Option.defaultValue exportedSymbol

                        match exportedSymbol.declarations with
                        | None -> acc
                        | Some declarations ->
                            (acc, declarations)
                            ||> Seq.fold (fun (symbols, defaults) declaration ->
                                let declaringFile =
                                    (unbox<Ts.Node> declaration).getSourceFile().fileName
                                    |> String.normalizePath

                                let key = declaringFile, exportName

                                if
                                    declaringFile.StartsWith package.Dir
                                    && not (entryFiles.Contains declaringFile)
                                    && not (symbols.ContainsKey key)
                                then
                                    let isDefault =
                                        not (defaults.ContainsKey declaringFile)
                                        && (
                                            match
                                                defaultExportOf program checker declaringFile
                                            with
                                            | Some target ->
                                                obj.ReferenceEquals(target, exportedSymbol)
                                            | None -> false
                                        )

                                    symbols.Add(key, subpath),
                                    (if isDefault then
                                         defaults.Add(declaringFile, exportName)
                                     else
                                         defaults)
                                else
                                    symbols, defaults
                            )
                    )
        )

/// The names a file declares itself, the ones it re-exports belong to another file
let private declaredNames
    (program: Ts.Program)
    (checker: Ts.TypeChecker)
    (defaultExportNames: Map<string, string>)
    (fileName: string)
    =
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
            // `export { alpha } from "./shared.js"` and `export default _default` are aliases,
            // the declaration behind each tells the file
            |> Seq.filter (fun exportedSymbol ->
                match
                    (resolveAlias checker exportedSymbol |> Option.defaultValue exportedSymbol)
                        .declarations
                with
                | Some declarations ->
                    declarations
                    |> Seq.exists (fun declaration ->
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
                    match defaultExportNames.TryFind normalized with
                    | Some name -> name
                    | None ->
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

/// A declaration the root entry exports, or one of a file no entry publishes, is read into the
/// package module, unless its name is taken by the entry or by a file read before it
let private hoistableDeclarations
    (program: Ts.Program)
    (checker: Ts.TypeChecker)
    (package: Reader.Types.PackageInfo)
    (reExportedFiles: Map<string, string>)
    (defaultExportNames: Map<string, string>)
    =
    let declaredNames = declaredNames program checker defaultExportNames
    let entryNames = declaredNames package.EntryFile

    let isEntry (file: string) =
        file = package.EntryFile
        || package.SubpathEntries |> List.exists (fun (entry, _) -> entry = file)

    let exported =
        reExportedFiles
        |> Map.toList
        |> List.filter (fun (file, subpath) -> subpath = "" && not (isEntry file))
        |> List.map fst
        |> List.sort

    let unpublished =
        program.getSourceFiles ()
        |> Seq.map (fun sourceFile -> String.normalizePath sourceFile.fileName)
        |> Seq.filter (fun file ->
            file.StartsWith package.Dir
            && not (file.Substring(package.Dir.Length).Contains "node_modules/")
            && not (isEntry file)
            && not (reExportedFiles.ContainsKey file)
        )
        |> Seq.sort
        |> Seq.toList

    let candidates = exported @ unpublished

    ((entryNames, Set.empty), candidates)
    ||> List.fold (fun (taken, hoistable) file ->
        let free = Set.difference (declaredNames file) taken

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
        /// A single `.d.ts` declares the globals of a script, nothing is imported
        GlobalScript: bool
        /// The overloads a signature gets at most from its union parameters
        MaxOverloads: int
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
        GlobalScript = false
        MaxOverloads = Transformer.UnionOverloads.defaultMaxOverloads
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
        | [] -> Resolve.listInstalledPackages host |> Array.toList
        | inputs ->
            inputs
            |> List.map (fun input ->
                let resolved = Resolve.resolveInput host input

                match resolved.kind with
                | "package" -> resolved.packageDir
                | _ ->
                    Resolve.findPackageDir host resolved.file
                    |> Option.defaultWith (fun () ->
                        failwith $"Could not find the package of {resolved.file}"
                    )
            )
            // `date-fns date-fns/locale` is one package
            |> List.distinct

    let targets =
        targetDirs
        |> List.map (fun dir ->
            Resolve.describePackage host dir
            |> Option.defaultWith (fun () ->
                failwith $"Could not find a declaration file for the package in {dir}"
            )
        )

    let entryFiles =
        targets
        |> List.collect (fun target ->
            target.entryFile :: (target.subpathEntries |> Array.toList |> List.map _.file)
        )
        |> List.toArray

    let withoutDomLib =
        targets |> List.exists (fun target -> domLibReplacements.Contains target.name)

    // TypeScript loads an installed `@types/node` as a global type package, `Buffer` of playwright is one
    let globalTypePackages =
        // A package standing for the runtime declares the globals itself
        let isRuntime =
            targets
            |> List.exists (fun target ->
                builtInExternalPackageNames
                |> List.exists (fun (name, _, _) -> name = target.name)
            )

        if options.ExternalPackages && not isRuntime then
            Resolve.findNodeModules host host.cwd
            |> Option.map (fun nodeModules -> host.path.join [| nodeModules; "@types/node" |])
            |> Option.filter host.fs.directoryExists
            // The program names the files of a linked package by their real path
            |> Option.map host.fs.realPath
            |> Option.bind (Resolve.describePackage host)
            |> Option.toList
        else
            []

    let globalTypes = globalTypePackages |> List.map _.runtimeName

    let program =
        Bootstrap.createProgramFromFiles host entryFiles globalTypes withoutDomLib options.NoLib

    let checker = program.getTypeChecker ()

    let targetDirSet =
        targets |> List.map (fun target -> String.normalizePath target.dir) |> set

    let describeReachable (excludedRuntimeNames: string list) =
        Bootstrap.reachableFiles host program entryFiles (List.toArray excludedRuntimeNames)
        |> Array.toList
        |> List.filter (fun fileName -> not (isTypeScriptLibFile fileName))
        |> List.choose (Resolve.findPackageDir host >> Option.map String.normalizePath)
        |> List.distinct
        |> List.filter (fun dir -> not (targetDirSet.Contains dir))
        |> List.choose (Resolve.describePackage host)

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
            let reachable = describeReachable [] @ globalTypePackages |> List.distinctBy _.name

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

    let ambientNames = ambientModuleNames checker

    let withReExportedFiles (package: Reader.Types.PackageInfo) =
        let hasExportsMap = exportsMapPackages.Contains package.Dir

        let ambientModuleFiles =
            program.getSourceFiles ()
            |> Seq.choose (fun sourceFile ->
                let fileName = String.normalizePath sourceFile.fileName

                if fileName.StartsWith package.Dir then
                    promotedAmbientModule sourceFile
                    |> Option.map (fun moduleDeclaration ->
                        fileName, ambientModuleSpecifier ambientNames moduleDeclaration
                    )
                else
                    None
            )
            |> Map.ofSeq

        let symbols, defaultExportNames =
            reExportedSymbols program checker hasExportsMap package

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
                    hoistableDeclarations program checker package reExportedFiles defaultExportNames
                else
                    Set.empty
            ReExportedSymbols = symbols
            ReExportedNames = reExportedNames program checker hasExportsMap package
            DefaultExportNames = defaultExportNames
            AmbientModuleFiles = ambientModuleFiles
            ReExportedFiles = reExportedFiles
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
            AmbientModuleNames = ambientNames
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
        Transform.applyWithOptions
            {
                Source =
                    Transformer.Context.ImportSource.Module(Naming.MODULE_PLACEHOLDER, Map.empty)
                MaxOverloads = options.MaxOverloads
            }
            readerResult.TypeMemory
            glueAst

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
