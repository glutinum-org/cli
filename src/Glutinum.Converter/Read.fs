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

    // A re-export whose declaration did not resolve has no key, the name is the only match
    let localNames =
        types
        |> List.choose (fun glueType ->
            match glueType with
            | GlueType.ReExport _ -> None
            | _ -> declarationKey glueType |> Option.map snd
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
            | None ->
                if localNames.Contains reExport.Name then
                    seen, acc
                else
                    seen, glueType :: acc
        | _ -> seen, glueType :: acc
    )
    |> snd

/// `animejs/easings/spring` and `animejs/easings/steps` share the `Easings` module: a name of
/// several segments is nested, and F# declares each module once
let rec private nestByName (fileModules: GlueFileModule list) : GlueType list =
    fileModules
    |> List.groupBy (fun fileModule -> fileModule.Name.Split('.').[0])
    |> List.map (fun (head, group) ->
        // Several files can share a subpath, the module holds the declarations of all of them
        let ownModules = group |> List.filter (fun fileModule -> fileModule.Name = head)

        let ownModule =
            match ownModules with
            | [] -> None
            | first :: rest ->
                Some
                    { first with
                        // One of them can re-export what another declares
                        Types =
                            first.Types @ (rest |> List.collect _.Types) |> dropShadowedReExports
                        SymbolSpecifiers =
                            (first.SymbolSpecifiers, rest)
                            ||> List.fold (fun acc other ->
                                (acc, other.SymbolSpecifiers)
                                ||> Map.fold (fun acc name specifier -> acc.Add(name, specifier))
                            )
                    }

        let nested =
            group
            |> List.filter (fun fileModule -> fileModule.Name <> head)
            |> List.map (fun fileModule ->
                { fileModule with
                    Name = fileModule.Name.Substring(head.Length + 1)
                }
            )
            |> nestByName

        match ownModule, nested with
        | Some fileModule, [] -> GlueType.FileModule fileModule
        | Some fileModule, nested ->
            GlueType.FileModule
                { fileModule with
                    Types = fileModule.Types @ nested
                }
        | None, nested ->
            GlueType.FileModule
                {
                    Name = head
                    ImportSpecifier = ""
                    SymbolSpecifiers = Map.empty
                    IsGlobal = false
                    HasRuntime = false
                    Types = nested
                }
    )

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
        // The entry is read first, a file is only read into the package module when the names
        // it declares are still free
        |> List.sortBy (fun sourceFile ->
            if String.normalizePath sourceFile.fileName = package.EntryFile then
                0
            else
                1
        )

    let readPackage (package: PackageInfo) =
        let globals = ResizeArray<GlueType>()
        let unexported = ResizeArray<string>()
        // A file the root entry exports is read into the package module, its declarations keep
        // the specifier of the subpath exporting each of them
        let hoistedSpecifiers = ResizeArray<string * string>()
        let hoistedTypes = ResizeArray<GlueType>()

        let rec withoutValues (types: GlueType list) =
            types
            |> List.choose (
                function
                | GlueType.ClassDeclaration info ->
                    Some(GlueType.ClassDeclaration { info with IsExported = false })
                | GlueType.FunctionDeclaration _
                | GlueType.Variable _
                | GlueType.ExportDefault(GlueType.FunctionDeclaration _)
                | GlueType.ExportDefault(GlueType.Variable _) -> None
                | GlueType.ModuleDeclaration info ->
                    Some(
                        GlueType.ModuleDeclaration
                            { info with
                                Types = withoutValues info.Types
                            }
                    )
                | glueType -> Some glueType
            )

        let withoutUnexported (fileName: string) (types: GlueType list) =
            let isUnexported (name: string) =
                if packageContext.IsImportable(fileName, name) then
                    false
                else
                    unexported.Add name
                    true

            types
            |> List.choose (
                function
                | GlueType.ClassDeclaration info when isUnexported info.Name ->
                    Some(GlueType.ClassDeclaration { info with IsExported = false })
                | GlueType.FunctionDeclaration info when isUnexported info.Name -> None
                | GlueType.Variable info when isUnexported info.Name -> None
                | GlueType.ExportDefault(GlueType.FunctionDeclaration _)
                | GlueType.ExportDefault(GlueType.Variable _) when isUnexported "default" -> None
                | GlueType.ModuleDeclaration info when
                    not info.IsGlobal
                    && not (info.Name.StartsWith "\"" || info.Name.StartsWith "'")
                    && isUnexported info.Name
                    ->
                    Some(
                        GlueType.ModuleDeclaration
                            { info with
                                Types = withoutValues info.Types
                            }
                    )
                | glueType -> Some glueType
            )

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
                        SymbolSpecifiers = packageContext.SymbolSpecifiers fileName
                        IsGlobal = false
                        HasRuntime = package.HasRuntime
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
                    let types = withoutUnexported fileName types

                    // A declaration the root entry exports goes to the package module, the file
                    // keeps a module for the ones whose name is taken elsewhere
                    let hoisted, kept =
                        types
                        |> List.partition (fun glueType ->
                            // `export default class Element` is hoisted under the name of the class
                            let declaration =
                                match glueType with
                                | GlueType.ExportDefault inner -> inner
                                | _ -> glueType

                            match declarationKey declaration with
                            | Some(_, name) -> packageContext.IsHoisted(package, fileName, name)
                            | None -> false
                        )

                    if not hoisted.IsEmpty then
                        hoistedSpecifiers.AddRange(
                            packageContext.SymbolSpecifiers fileName |> Map.toSeq
                        )

                        hoistedTypes.AddRange hoisted

                    if kept.IsEmpty then
                        None
                    else
                        fileModule kept
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
            |> fun types -> types @ List.ofSeq hoistedTypes
            // The entry re-exports what a file it is read with declares
            |> dropShadowedReExports

        let fileModules =
            fileModules
            |> List.choose (
                function
                | Choice2Of2(GlueType.FileModule fileModule) -> Some fileModule
                | _ -> None
            )
            |> nestByName

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

        if unexported.Count > 0 then
            reader.Warnings.Add
                $"%s{package.RuntimeName}: %i{unexported.Count} declarations are not exported by any entry, they are generated as types only"

        entryTypes @ globalsModule @ fileModules, entryIsGlobal, Map.ofSeq hoistedSpecifiers

    let glueAst =
        packageContext.Packages
        |> List.collect (fun package ->
            let types, isGlobal, hoistedSpecifiers = readPackage package

            [
                ({
                    Name = package.ModuleName
                    ImportSpecifier = package.RuntimeName
                    SymbolSpecifiers = hoistedSpecifiers
                    IsGlobal = isGlobal
                    HasRuntime = package.HasRuntime
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
