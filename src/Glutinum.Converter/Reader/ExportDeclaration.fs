module Glutinum.Converter.Reader.ExportDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

let private tryResolveAlias (checker: Ts.TypeChecker) (symbol: Ts.Symbol) =
    match symbol.flags with
    | HasSymbolFlags Ts.SymbolFlags.Alias ->
        try
            let aliased = checker.getAliasedSymbol symbol

            if isNull (box aliased) then
                None
            else
                Some aliased
        with _ ->
            None
    | _ -> Some symbol

let private readReExport
    (reader: ITypeScriptReader)
    (packageContext: PackageContext)
    (currentFile: string)
    (exportedName: string)
    (symbol: Ts.Symbol)
    : GlueType list
    =
    match tryResolveAlias reader.checker symbol with
    | None -> []
    | Some target ->
        match target.declarations with
        | Some declarations when declarations.Count > 0 ->
            let declarationFile =
                declarations.[0].getSourceFile().fileName |> String.normalizePath

            if declarationFile = currentFile || packageContext.IsExternal declarationFile then
                []
            else
                declarations
                |> Seq.toList
                |> List.choose (fun declaration ->
                    let declarationToRead: Ts.Node option =
                        match declaration.kind with
                        | Ts.SyntaxKind.InterfaceDeclaration
                        | Ts.SyntaxKind.TypeAliasDeclaration
                        | Ts.SyntaxKind.EnumDeclaration
                        | Ts.SyntaxKind.ClassDeclaration
                        | Ts.SyntaxKind.FunctionDeclaration -> Some(declaration :> Ts.Node)
                        | Ts.SyntaxKind.VariableDeclaration ->
                            // VariableDeclaration -> VariableDeclarationList -> VariableStatement
                            Some(declaration.parent.parent)
                        | _ -> None

                    declarationToRead
                    |> Option.map (fun declaration ->
                        ({
                            Name = exportedName
                            Declaration = reader.ReadNode declaration
                            ModulePath = modulePathForDeclaration packageContext declaration
                        }
                        : GlueReExport)
                        |> GlueType.ReExport
                    )
                )
                // Merged interfaces are declared several times, one alias is enough
                |> List.distinctBy (fun glueType ->
                    match glueType with
                    | GlueType.ReExport { Declaration = GlueType.Interface _ } -> "interface"
                    | _ -> System.Guid.NewGuid().ToString()
                )
        | _ -> []

let readExportDeclaration
    (reader: ITypeScriptReader)
    (exportDeclaration: Ts.ExportDeclaration)
    : GlueType list
    =
    match reader.PackageContext with
    | None ->
        // Only package mode knows the module the declaration of another file lands in
        match exportDeclaration.moduleSpecifier with
        | Some _ ->
            let warning =
                "A re-export from another file is only resolved when generating a package, run `glue <package>` instead of a single declaration file"

            if not (reader.Warnings.Contains warning) then
                reader.Warnings.Add warning
        | None -> ()

        []
    | Some packageContext ->
        let checker = reader.checker

        let currentFile = exportDeclaration.getSourceFile().fileName |> String.normalizePath

        match exportDeclaration.exportClause with
        // export * from "./file"
        | None ->
            match exportDeclaration.moduleSpecifier with
            | None -> []
            | Some moduleSpecifier ->
                match checker.getSymbolAtLocation moduleSpecifier with
                | None -> []
                | Some moduleSymbol ->
                    checker.getExportsOfModule moduleSymbol
                    |> Seq.toList
                    |> List.filter (fun symbol -> symbol.name <> "default")
                    |> List.collect (fun symbol ->
                        readReExport reader packageContext currentFile symbol.name symbol
                    )

        | Some exportClause ->
            match exportClause?kind with
            // export { a, b as c } [from "./file"]
            | Ts.SyntaxKind.NamedExports ->
                let namedExports: Ts.NamedExports = !!exportClause

                namedExports.elements
                |> Seq.toList
                |> List.collect (fun specifier ->
                    let exportedName: string = specifier.name?text

                    match checker.getExportSpecifierLocalTargetSymbol (U2.Case1 specifier) with
                    | None -> []
                    | Some symbol ->
                        readReExport reader packageContext currentFile exportedName symbol
                )

            // export * as ns from "./file" is not supported yet
            | _ -> []
