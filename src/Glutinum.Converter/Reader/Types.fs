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

    member this.FileModuleName(package: PackageInfo, fileName: string) =
        let fileName = String.normalizePath fileName

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

        segments
        |> List.map (fun segment ->
            segment.Split([| '-'; '.'; ' ' |], System.StringSplitOptions.RemoveEmptyEntries)
            |> String.concat "_"
        )
        |> String.concat "_"
        |> Naming.sanitizeTypeName

    /// F# modules qualifying a type declared in `fileName`, empty for the target entry file
    member this.ModulePath(fileName: string) : string list = this.ModulePath(fileName, true)

    /// `includeFile = false` gives the package module only, where its globals are
    member this.ModulePath(fileName: string, includeFile: bool) : string list =
        match this.TryFindPackage fileName with
        | None -> this.TryFindExternalModulePath(fileName, includeFile) |> Option.defaultValue []
        | Some package ->
            let fileName = String.normalizePath fileName

            [
                package.ModuleName

                if includeFile && fileName <> package.EntryFile then
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
                    let relativePath = fileName.Substring(package.Dir.Length)

                    let withoutExtension =
                        System.Text.RegularExpressions.Regex.Replace(
                            relativePath,
                            "\\.d\\.[cm]?ts$",
                            ""
                        )

                    package.RuntimeName + "/" + withoutExtension + ".js"

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

    abstract ReadParameters: parameters: ResizeArray<Ts.ParameterDeclaration> -> GlueParameter list

    abstract ReadDeclaration: declaration: Ts.Declaration -> GlueMember

    abstract ReadUnionTypeNode: unionType: Ts.UnionTypeNode -> GlueType

    abstract ReadTypeOperatorNode: node: Ts.TypeOperatorNode -> GlueType

    abstract ReadIndexedAccessType: declaration: Ts.IndexedAccessType -> GlueType

    abstract ReadTypeParameters:
        typeParametersOpt: ResizeArray<Ts.TypeParameterDeclaration> option -> GlueTypeParameter list

    abstract ReadDocumentationFromSignature: declaration: Ts.Declaration -> GlueComment list

    abstract ReadDocumentationFromNode: node: Ts.Node -> GlueComment list

    abstract ReadNamedTupleMember: namedTupleMember: Ts.NamedTupleMember -> GlueType

    abstract ReadMappedTypeNode: declaration: Ts.MappedTypeNode -> GlueType
