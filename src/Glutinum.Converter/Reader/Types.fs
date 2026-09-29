module Glutinum.Converter.Reader.Types

open Fable.Core
open TypeScript
open Glutinum.Converter.GlueAST
open System.Collections.Generic

type PackageInfo =
    {
        /// Name used for the F# module (e.g. `VscodeLanguageserver`)
        ModuleName: string
        /// Name used to import the package at runtime (e.g. `vscode` for `@types/vscode`)
        RuntimeName: string
        /// The package ships JavaScript, `undici-types` is declaration files and nothing else
        HasRuntime: bool
        /// Normalized absolute directory, with a trailing `/`
        Dir: string
        /// Directory the file modules are named from: the `typesVersions` folder of the entry, else `Dir`
        TypesRoot: string
        /// Normalized absolute path of the main declaration file
        EntryFile: string
        /// Other declaration entry points, with the subpath to import them from
        SubpathEntries: (string * string) list
        /// A declaration a public entry re-exports, keyed by the file declaring it and the name
        /// it is exported under, with the subpath to import it from
        ReExportedSymbols: Map<string * string, string>
        /// A file a public entry re-exports a declaration of, with the subpath to import it from
        ReExportedFiles: Map<string, string>
        /// A name a public entry exports, with the subpath to import it from
        ReExportedNames: Map<string, string>
        /// The name a public entry exports the default export of a file under, keyed by the file
        DefaultExportNames: Map<string, string>
        /// The `exports` map makes every file it does not list unreachable
        HasExportsMap: bool
        /// A declaration the root entry exports under a name nothing else declares, keyed by the
        /// file declaring it: it is read into the package module
        HoistableDeclarations: Set<string * string>
    }

/// A package published as its own binding: its types are referenced, never generated
type ExternalPackage =
    {
        /// Module of the binding under `Glutinum` (`Web` for `Glutinum.Web`)
        ModuleName: string
        /// The package on disk, when installed
        Package: PackageInfo option
        /// TypeScript lib files standing for the package (`lib.dom` for `@types/web`)
        LibFilePrefixes: string list
    }

type PackageContext =
    {
        Packages: PackageInfo list
        Externals: ExternalPackage list
        /// A declaration of the generated package standing for the standard library, when the
        /// package is the library itself generated without it
        IsLibraryName: string -> bool
    }

    member this.TryFindPackage(fileName: string) =
        let fileName = String.normalizePath fileName

        this.Packages
        |> List.filter (fun package ->
            fileName.StartsWith package.Dir
            // A package nested in `node_modules` of the package is another package
            && not (fileName.Substring(package.Dir.Length).Contains "node_modules/")
        )
        |> List.tryHead

    /// The F# modules of the external binding declaring `fileName`
    member this.TryFindExternalModulePath(fileName: string) : string list option =
        this.TryFindExternalModulePath(fileName, true)

    member this.TryFindExternalModulePath
        (fileName: string, includeFile: bool)
        : string list option
        =
        let fileName = String.normalizePath fileName

        this.Externals
        |> List.tryPick (fun external ->
            let isLibFile =
                external.LibFilePrefixes |> List.exists (fun prefix -> fileName.Contains prefix)

            if isLibFile then
                Some [ "Glutinum"; external.ModuleName ]
            else
                match external.Package with
                | Some package when fileName.StartsWith package.Dir ->
                    Some
                        [
                            yield "Glutinum"
                            yield external.ModuleName

                            if includeFile && fileName <> package.EntryFile then
                                yield this.FileModuleName(package, fileName)
                        ]
                | _ -> None
        )

    member this.IsExternal(fileName: string) =
        (this.TryFindPackage fileName).IsNone
        && (this.TryFindExternalModulePath fileName).IsNone

    /// Whether the root entry exports the declaration: it belongs to the package module, the
    /// file declaring it is an implementation detail
    member this.IsHoisted(package: PackageInfo, fileName: string, name: string) =
        package.HoistableDeclarations.Contains(String.normalizePath fileName, name)

    /// The subpath a public entry exports the declarations of the file from, when there is one
    member this.SubpathOf(package: PackageInfo, fileName: string) =
        let fileName = String.normalizePath fileName

        package.SubpathEntries
        |> List.tryPick (fun (file, subpath) ->
            if file = fileName then
                Some subpath
            else
                None
        )
        |> Option.orElseWith (fun () -> package.ReExportedFiles.TryFind fileName)

    member this.FileModuleName(package: PackageInfo, fileName: string) =
        let fileName = String.normalizePath fileName

        // `animejs/svg` is `Animejs.svg`, spelled as the import specifier is, the file declaring
        // it is an implementation detail
        let fromSubpath =
            match this.SubpathOf(package, fileName) with
            | Some subpath when subpath <> "" ->
                subpath.Split('/')
                |> Array.map (fun segment ->
                    segment.Split(
                        [| '-'; '.'; ' ' |],
                        System.StringSplitOptions.RemoveEmptyEntries
                    )
                    |> String.concat "_"
                    |> Naming.sanitizeTypeName
                )
                |> String.concat "."
                |> Some
            | _ -> None

        match fromSubpath with
        | Some name -> name
        | None ->

            let relativePath =
                if fileName.StartsWith package.TypesRoot then
                    fileName.Substring(package.TypesRoot.Length)
                else
                    fileName.Substring(package.Dir.Length)

            let segments =
                relativePath.Split('/')
                |> Array.toList
                |> List.filter (fun segment -> segment <> "")

            let segments =
                match List.rev segments with
                | last :: rest ->
                    let withoutExtension =
                        System.Text.RegularExpressions.Regex.Replace(last, "\\.d\\.[cm]?ts$", "")

                    let isSubpathEntry =
                        package.SubpathEntries |> List.exists (fun (file, _) -> file = fileName)

                    if withoutExtension = "index" && not rest.IsEmpty then
                        List.rev rest
                    // `chart.js/auto` is `auto/auto.d.ts`
                    elif List.tryHead rest = Some withoutExtension && isSubpathEntry then
                        List.rev rest
                    else
                        List.rev (withoutExtension :: rest)
                | [] -> []

            // `fs/promises.d.ts` is the nested module `fs.promises`, spelled as the package
            // spells it
            segments
            |> List.map (fun segment ->
                segment.Split([| '-'; '.'; ' ' |], System.StringSplitOptions.RemoveEmptyEntries)
                |> String.concat "_"
                |> Naming.sanitizeTypeName
            )
            |> String.concat "."

    /// F# modules qualifying a type declared in `fileName`, empty for the target entry file
    member this.ModulePath(fileName: string) : string list = this.ModulePath(fileName, true)

    /// `includeFile = false` gives the package module only, where its globals are
    member this.ModulePath(fileName: string, includeFile: bool) : string list =
        this.ModulePath(fileName, includeFile, None)

    /// `name` is the declaration at the top of the file the reference lands in
    member this.ModulePath(fileName: string, includeFile: bool, name: string option) : string list =
        match this.TryFindPackage fileName with
        | None -> this.TryFindExternalModulePath(fileName, includeFile) |> Option.defaultValue []
        | Some package ->
            let fileName = String.normalizePath fileName

            let isHoisted =
                match name with
                | Some name -> this.IsHoisted(package, fileName, name)
                | None -> false

            [
                package.ModuleName

                if includeFile && fileName <> package.EntryFile && not isHoisted then
                    this.FileModuleName(package, fileName)
            ]

    member this.IsEntryFile(fileName: string) =
        match this.TryFindPackage fileName with
        | Some package -> String.normalizePath fileName = package.EntryFile
        | None -> false

    member this.ImportSpecifier(fileName: string) =
        match this.TryFindPackage fileName with
        | None -> Naming.MODULE_PLACEHOLDER
        | Some package ->
            let fileName = String.normalizePath fileName

            if fileName = package.EntryFile then
                package.RuntimeName
            else
                match package.SubpathEntries |> List.tryFind (fun (file, _) -> file = fileName) with
                | Some(_, subpath) -> package.RuntimeName + "/" + subpath
                | None ->

                    match package.ReExportedFiles.TryFind fileName with
                    | Some "" -> package.RuntimeName
                    | Some subpath -> package.RuntimeName + "/" + subpath
                    | None ->
                        let relativePath = fileName.Substring(package.Dir.Length)

                        let withoutExtension =
                            System.Text.RegularExpressions.Regex.Replace(
                                relativePath,
                                "\\.d\\.[cm]?ts$",
                                ""
                            )

                        package.RuntimeName + "/" + withoutExtension + ".js"

    /// Whether a public entry of the package re-exports a declaration of the file
    member this.IsReExported(fileName: string) =
        match this.TryFindPackage fileName with
        | None -> true
        | Some package ->
            let fileName = String.normalizePath fileName

            fileName = package.EntryFile
            || package.SubpathEntries |> List.exists (fun (file, _) -> file = fileName)
            || package.ReExportedFiles.ContainsKey fileName

    /// Whether a specifier can import the declaration at runtime: the `exports` map of a package
    /// blocks the file declaring it unless a public entry exports the name
    member this.IsImportable(fileName: string, name: string) =
        match this.TryFindPackage fileName with
        | None -> true
        | Some package when not package.HasExportsMap -> true
        | Some package ->
            let fileName = String.normalizePath fileName

            fileName = package.EntryFile
            || package.SubpathEntries |> List.exists (fun (file, _) -> file = fileName)
            || (if name = "default" then
                    package.DefaultExportNames.ContainsKey fileName
                else
                    package.ReExportedSymbols.ContainsKey(fileName, name)
                    || package.ReExportedNames.ContainsKey name)

    /// The name a public entry exports the default export of the file under
    member this.DefaultExportName(fileName: string) : string option =
        this.TryFindPackage fileName
        |> Option.bind (fun package ->
            package.DefaultExportNames.TryFind(String.normalizePath fileName)
        )

    /// The declarations of the file a public entry re-exports, with the specifier to import each
    /// of them from
    member this.SymbolSpecifiers(fileName: string) : Map<string, string> =
        match this.TryFindPackage fileName with
        | None -> Map.empty
        | Some package ->
            let fileName = String.normalizePath fileName

            let specifierOf (subpath: string) =
                if subpath = "" then
                    package.RuntimeName
                else
                    package.RuntimeName + "/" + subpath

            let byName =
                if this.IsReExported fileName then
                    Map.empty
                else
                    package.ReExportedNames |> Map.map (fun _ subpath -> specifierOf subpath)

            let bySymbol =
                package.ReExportedSymbols
                |> Map.toList
                |> List.choose (fun ((file, name), subpath) ->
                    if file = fileName then
                        Some(name, specifierOf subpath)
                    else
                        None
                )
                |> Map.ofList

            (byName, bySymbol)
            ||> Map.fold (fun acc name specifier -> Map.add name specifier acc)

[<Mangle>]
type ITypeScriptReader =
    abstract checker: Ts.TypeChecker with get

    /// Set in package mode only
    abstract PackageContext: PackageContext option with get

    abstract Warnings: ResizeArray<string> with get

    abstract TypeMemory: ResizeArray<GlueType> with get

    /// The source node a type synthesized by `typeToTypeNode` is read for, it has no parent itself
    abstract SyntheticContext: Ts.Node option with get, set

    abstract ReadNode: node: Ts.Node -> GlueType

    abstract ReadTypeNode: typNode: Ts.TypeNode -> GlueType

    abstract ReadTypeNode: typNode: Ts.TypeNode option -> GlueType

    abstract ReadEnumDeclaration: enumDeclaration: Ts.EnumDeclaration -> GlueType

    abstract ReadTypeAliasDeclaration: typeAliasDeclaration: Ts.TypeAliasDeclaration -> GlueType

    abstract ReadInterfaceDeclaration: interfaceDeclaration: Ts.InterfaceDeclaration -> GlueType

    abstract ReadVariableStatement: variableStatement: Ts.VariableStatement -> GlueType

    abstract ReadFunctionDeclaration: functionDeclaration: Ts.FunctionDeclaration -> GlueType

    abstract ReadModuleDeclaration: moduleDeclaration: Ts.ModuleDeclaration -> GlueType

    abstract ReadClassDeclaration: classDeclaration: Ts.ClassDeclaration -> GlueType

    abstract ReadExportAssignment: exportAssignment: Ts.ExportAssignment -> GlueType

    abstract ReadExportDeclaration: exportDeclaration: Ts.ExportDeclaration -> GlueType list

    abstract ReadParameters: parameters: Ts.NodeArray<Ts.ParameterDeclaration> -> GlueParameter list

    abstract ReadDeclaration: declaration: Ts.Declaration -> GlueMember

    abstract ReadUnionTypeNode: unionType: Ts.UnionTypeNode -> GlueType

    abstract ReadTypeOperatorNode: node: Ts.TypeOperatorNode -> GlueType

    abstract ReadIndexedAccessType: declaration: Ts.IndexedAccessType -> GlueType

    abstract ReadTypeParameters:
        typeParametersOpt: Ts.NodeArray<Ts.TypeParameterDeclaration> option ->
            GlueTypeParameter list

    abstract ReadDocumentationFromSignature: declaration: Ts.Declaration -> GlueComment list

    abstract ReadDocumentationFromNode: node: Ts.Node -> GlueComment list

    abstract ReadNamedTupleMember: namedTupleMember: Ts.NamedTupleMember -> GlueType

    abstract ReadMappedTypeNode: declaration: Ts.MappedTypeNode -> GlueType
