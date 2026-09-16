module rec Glutinum.Converter.Read

open TypeScript
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.TypeScriptReader

let readSourceFile (checker: Ts.TypeChecker) (sourceFile: option<Ts.SourceFile>) =
    let reader: ITypeScriptReader = TypeScriptReader(checker)

    {|
        GlueAST = sourceFile.Value.statements |> List.ofSeq |> List.map reader.ReadNode
        Warnings = reader.Warnings
        TypeMemory = reader.TypeMemory |> List.ofSeq
    |}

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

let private readStatements (reader: ITypeScriptReader) (sourceFile: Ts.SourceFile) =
    sourceFile.statements
    |> List.ofSeq
    |> List.collect (fun statement ->
        match statement.kind with
        | Ts.SyntaxKind.ExportDeclaration ->
            reader.ReadExportDeclaration(statement :?> Ts.ExportDeclaration)
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
        let entryTypes, fileModules =
            filesOfPackage package
            |> List.map (fun sourceFile ->
                let fileName = String.normalizePath sourceFile.fileName
                let types = readStatements reader sourceFile

                if fileName = package.EntryFile then
                    Choice1Of2 types
                else
                    ({
                        Name = packageContext.FileModuleName(package, fileName)
                        ImportSpecifier = packageContext.ImportSpecifier fileName
                        Types = types
                    }
                    : GlueFileModule)
                    |> GlueType.FileModule
                    |> Choice2Of2
            )
            |> List.partition (
                function
                | Choice1Of2 _ -> true
                | Choice2Of2 _ -> false
            )

        let entryTypes =
            entryTypes
            |> List.collect (
                function
                | Choice1Of2 types -> types
                | Choice2Of2 _ -> []
            )

        let fileModules =
            fileModules
            |> List.choose (
                function
                | Choice2Of2 fileModule -> Some fileModule
                | Choice1Of2 _ -> None
            )

        entryTypes @ fileModules

    let glueAst =
        packageContext.Packages
        |> List.collect (fun package ->
            let types = readPackage package

            if package.IsTarget then
                types
            else
                [
                    ({
                        Name = package.ModuleName
                        ImportSpecifier = package.RuntimeName
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
