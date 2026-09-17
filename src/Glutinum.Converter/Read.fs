module rec Glutinum.Converter.Read

open TypeScript
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.TypeScriptReader
open Glutinum.Converter.Reader.Utils
open Fable.Core.JsInterop

let readSourceFile (checker: Ts.TypeChecker) (sourceFile: option<Ts.SourceFile>) =
    let reader: ITypeScriptReader = TypeScriptReader(checker)

    {|
        GlueAST = readStatements reader sourceFile.Value
        // The module the file is made of, when it is one
        ImportSpecifier =
            promotedAmbientModule sourceFile.Value
            |> Option.map (fun moduleDeclaration ->
                Naming.removeSurroundingQuotes (moduleDeclaration.name?text)
            )
        Warnings = reader.Warnings
        TypeMemory = reader.TypeMemory |> List.ofSeq
    |}

/// A file without import or export, and not made of an ambient module: its declarations are globals
let private isScript (sourceFile: Ts.SourceFile) =
    not (ts.isExternalModule sourceFile)
    && (promotedAmbientModule sourceFile).IsNone

let private declarationKey (glueType: GlueType) =
    match glueType with
    | GlueType.Interface info -> Some("type", info.Name)
    | GlueType.TypeAliasDeclaration info -> Some("type", info.Name)
    | GlueType.Enum info -> Some("type", info.Name)
    | GlueType.ClassDeclaration info -> Some("class", info.Name)
    | GlueType.FunctionDeclaration info -> Some("value", info.Name)
    | GlueType.Variable info -> Some("value", info.Name)
    | _ -> None

/// A name declared in the file wins over a re-export, and the first re-export wins over the others
let private dropShadowedReExports (types: GlueType list) =
    let localKeys =
        types
        |> List.choose (fun glueType ->
            match glueType with
            | GlueType.ReExport _ -> None
            | _ -> declarationKey glueType
        )
        |> set

    (types, (Map.empty, []))
    ||> List.foldBack (fun glueType (seen, acc) ->
        match glueType with
        | GlueType.ReExport reExport ->
            match declarationKey reExport.Declaration with
            | Some(category, _) ->
                let key = category, reExport.Name

                if localKeys.Contains key then
                    seen, acc
                else
                    match Map.tryFind key seen with
                    // Overloads of a function are re-exported one by one
                    | Some modulePath when category = "value" && modulePath = reExport.ModulePath ->
                        seen, glueType :: acc
                    | Some _ -> seen, acc
                    | None -> Map.add key reExport.ModulePath seen, glueType :: acc
            | None -> seen, glueType :: acc
        | _ -> seen, glueType :: acc
    )
    |> snd

/// The declarations of the `global { }` blocks, taken out of their modules
let rec private extractGlobals (types: GlueType list) : GlueType list * GlueType list =
    (([], []), types)
    ||> List.fold (fun (remaining, globals) glueType ->
        match glueType with
        | GlueType.ModuleDeclaration info when info.IsGlobal -> remaining, globals @ info.Types
        | GlueType.ModuleDeclaration info ->
            let innerRemaining, innerGlobals = extractGlobals info.Types

            remaining @ [ GlueType.ModuleDeclaration { info with Types = innerRemaining } ],
            globals @ innerGlobals
        | _ -> remaining @ [ glueType ], globals
    )

let private readStatements (reader: ITypeScriptReader) (sourceFile: Ts.SourceFile) =
    let promoted = promotedAmbientModule sourceFile

    sourceFile.statements
    |> List.ofSeq
    |> List.collect (fun statement ->
        match statement.kind with
        | Ts.SyntaxKind.ExportDeclaration ->
            reader.ReadExportDeclaration(statement :?> Ts.ExportDeclaration)
        | Ts.SyntaxKind.InterfaceDeclaration when
            isMergedInterfaceDeclaration reader.checker reader.PackageContext statement
            ->
            []
        // `declare module "os" { ... }` is the file
        | Ts.SyntaxKind.ModuleDeclaration when
            (match promoted with
             | Some promoted -> obj.ReferenceEquals(promoted, statement)
             | None -> false)
            ->
            match reader.ReadNode statement with
            | GlueType.ModuleDeclaration moduleDeclaration -> moduleDeclaration.Types
            | glueType -> [ glueType ]
        | _ -> [ reader.ReadNode statement ]
    )
    |> dropShadowedReExports

/// <summary>
/// Read every file of the packages, the target package entry file stays at the top level,
/// its other files and the dependency packages become modules.
/// </summary>
let readPackages
    (checker: Ts.TypeChecker)
    (packageContext: PackageContext)
    (sourceFiles: Ts.SourceFile list)
    =
    let reader: ITypeScriptReader = TypeScriptReader(checker, packageContext)

    let filesOfPackage (package: PackageInfo) =
        sourceFiles
        |> List.filter (fun sourceFile ->
            match packageContext.TryFindPackage sourceFile.fileName with
            | Some candidate -> candidate.Dir = package.Dir
            | None -> false
        )
        |> List.sortBy (fun sourceFile -> String.normalizePath sourceFile.fileName)

    let readPackage (package: PackageInfo) =
        let globals = ResizeArray<GlueType>()

        let entryTypes, fileModules =
            filesOfPackage package
            |> List.choose (fun sourceFile ->
                let fileName = String.normalizePath sourceFile.fileName
                let types, fileGlobals = readStatements reader sourceFile |> extractGlobals
                globals.AddRange fileGlobals

                let importSpecifier =
                    match promotedAmbientModule sourceFile with
                    | Some moduleDeclaration ->
                        Naming.removeSurroundingQuotes (moduleDeclaration.name?text)
                    | None -> packageContext.ImportSpecifier fileName

                let fileModule (types: GlueType list) =
                    ({
                        Name = packageContext.FileModuleName(package, fileName)
                        ImportSpecifier = importSpecifier
                        IsGlobal = false
                        Types = types
                    }
                    : GlueFileModule)
                    |> GlueType.FileModule
                    |> Choice2Of2
                    |> Some

                if fileName = package.EntryFile then
                    Some(Choice1Of2(types, isScript sourceFile))
                elif isScript sourceFile then
                    let ambientModules, scriptGlobals =
                        types
                        |> List.partition (
                            function
                            | GlueType.ModuleDeclaration info ->
                                info.Name.StartsWith "\"" || info.Name.StartsWith "'"
                            | _ -> false
                        )

                    globals.AddRange scriptGlobals

                    if ambientModules.IsEmpty then
                        None
                    else
                        fileModule ambientModules
                else
                    fileModule types
            )
            |> List.partition (
                function
                | Choice1Of2 _ -> true
                | Choice2Of2 _ -> false
            )

        let entryIsGlobal =
            entryTypes
            |> List.exists (
                function
                | Choice1Of2(_, isGlobal) -> isGlobal
                | Choice2Of2 _ -> false
            )

        let entryTypes =
            entryTypes
            |> List.collect (
                function
                | Choice1Of2(types, _) -> types
                | Choice2Of2 _ -> []
            )

        let fileModules =
            fileModules
            |> List.choose (
                function
                | Choice2Of2 fileModule -> Some fileModule
                | Choice1Of2 _ -> None
            )

        let globalsModule =
            if globals.Count = 0 then
                []
            else
                [
                    ({
                        Documentation = []
                        Name = "global"
                        IsTopLevel = false
                        IsNamespace = false
                        IsGlobal = true
                        IsRecursive = false
                        Types = List.ofSeq globals
                    }
                    : GlueModuleDeclaration)
                    |> GlueType.ModuleDeclaration
                ]

        entryTypes @ globalsModule @ fileModules, entryIsGlobal

    let glueAst =
        packageContext.Packages
        |> List.collect (fun package ->
            let types, isGlobal = readPackage package

            [
                ({
                    Name = package.ModuleName
                    ImportSpecifier = package.RuntimeName
                    IsGlobal = isGlobal
                    Types = types
                }
                : GlueFileModule)
                |> GlueType.FileModule
            ]
        )

    {|
        GlueAST = glueAst
        Warnings = reader.Warnings
        TypeMemory = reader.TypeMemory |> List.ofSeq
    |}
