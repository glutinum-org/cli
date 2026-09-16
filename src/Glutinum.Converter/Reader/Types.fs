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
        /// Normalized absolute directory, with a trailing `/`
        Dir: string
        /// Normalized absolute path of the main declaration file
        EntryFile: string
        /// Other declaration entry points, with the subpath to import them from
        SubpathEntries: (string * string) list
        IsTarget: bool
    }

type PackageContext =
    {
        Packages: PackageInfo list
    }

    member this.TryFindPackage(fileName: string) =
        let fileName = String.normalizePath fileName

        this.Packages
        |> List.filter (fun package -> fileName.StartsWith package.Dir)
        // Nested `node_modules` must win over their parent
        |> List.sortByDescending _.Dir.Length
        |> List.tryHead

    member this.IsExternal(fileName: string) = (this.TryFindPackage fileName).IsNone

    member this.FileModuleName(package: PackageInfo, fileName: string) =
        let relativePath = (String.normalizePath fileName).Substring(package.Dir.Length)

        let segments =
            relativePath.Split('/')
            |> Array.toList
            |> List.filter (fun segment -> segment <> "")

        let segments =
            match List.rev segments with
            | last :: rest ->
                let withoutExtension =
                    System.Text.RegularExpressions.Regex.Replace(last, "\\.d\\.[cm]?ts$", "")

                if withoutExtension = "index" && not rest.IsEmpty then
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
    member this.ModulePath(fileName: string) : string list =
        match this.TryFindPackage fileName with
        | None -> []
        | Some package ->
            let fileName = String.normalizePath fileName

            [
                if not package.IsTarget then
                    package.ModuleName

                if fileName <> package.EntryFile then
                    this.FileModuleName(package, fileName)
            ]

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
