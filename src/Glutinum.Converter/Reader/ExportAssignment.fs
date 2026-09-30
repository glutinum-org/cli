module Glutinum.Converter.Reader.ExportAssignment

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

let readExportAssignment (reader: ITypeScriptReader) (exportNode: Ts.ExportAssignment) : GlueType =

    // Not sure why we don't have symbol property in the type definition
    let symbolOpt: Ts.Symbol option = exportNode?symbol

    let targetDeclarationKind =
        if exportNode.expression.kind = Ts.SyntaxKind.Identifier then
            reader.checker.getSymbolAtLocation exportNode.expression
            |> Option.bind (fun symbol -> symbol.valueDeclaration)
            |> Option.map (fun declaration -> declaration.kind)
        else
            None

    let exportEqualsDeclarationKind =
        if exportNode.isExportEquals = Some true then
            targetDeclarationKind
        else
            None

    let isDefaultOfVariable =
        exportNode.isExportEquals <> Some true
        && targetDeclarationKind = Some Ts.SyntaxKind.VariableDeclaration

    let sourceFile = exportNode.getSourceFile ()

    // `import yargs = require("./index.js")` is an alias, nothing is generated for it
    let fileDeclares (name: string) =
        let declares (table: obj) =
            if isNull table then
                false
            else
                let symbol: Ts.Symbol = table?get (name)

                not (isNull symbol)
                && (
                    match symbol.flags with
                    | HasSymbolFlags Ts.SymbolFlags.Alias -> false
                    | _ -> true
                )

        let exports: obj =
            reader.checker.getSymbolAtLocation (unbox<Ts.Node> sourceFile)
            |> Option.map (fun moduleSymbol -> moduleSymbol?exports)
            |> Option.defaultValue null

        declares sourceFile?locals || declares exports

    // `export default _instanceFactory` of `yargs` is imported as `yargs`
    let packageName =
        reader.PackageContext
        |> Option.bind (fun packageContext -> packageContext.TryFindPackage sourceFile.fileName)
        |> Option.filter (fun package ->
            String.normalizePath sourceFile.fileName = package.EntryFile
        )
        |> Option.map _.RuntimeName
        |> Option.filter (fun name ->
            name |> Seq.forall (fun c -> System.Char.IsLetterOrDigit c || c = '_')
            && not (fileDeclares name)
        )

    // `export { default as animator } from "./animator.js"`: the entry names the default export
    let publishedName =
        reader.PackageContext
        |> Option.bind (fun packageContext -> packageContext.DefaultExportName sourceFile.fileName)

    // `export = path` of a variable: the module is the variable
    let isExportEqualsOfVariable =
        exportEqualsDeclarationKind = Some Ts.SyntaxKind.VariableDeclaration

    // `export = dayjs` of a function or a class: `module.exports` is the default import
    let isExportEqualsOfDeclaration =
        exportEqualsDeclarationKind = Some Ts.SyntaxKind.FunctionDeclaration
        || exportEqualsDeclarationKind = Some Ts.SyntaxKind.ClassDeclaration

    match symbolOpt with
    | Some symbol ->
        if
            symbol.name = "default"
            || isExportEqualsOfVariable
            || isExportEqualsOfDeclaration
        then
            match exportNode.expression.kind with
            | Ts.SyntaxKind.Identifier ->
                let identiferNode: Ts.Identifier = !!exportNode.expression
                // The declared type of the variable keeps the module path of a reference
                let declaredTypeNode =
                    reader.checker.getSymbolAtLocation exportNode.expression
                    |> Option.bind _.valueDeclaration
                    |> Option.filter (fun declaration ->
                        declaration.kind = Ts.SyntaxKind.VariableDeclaration
                    )
                    |> Option.bind (fun declaration ->
                        (declaration :?> Ts.VariableDeclaration).``type``
                    )

                let typ =
                    let tsTyp = reader.checker.getTypeAtLocation (exportNode.expression)

                    match declaredTypeNode with
                    | Some typeNode -> reader.ReadTypeNode typeNode
                    | None ->
                        match tsTyp.flags with
                        | HasTypeFlags Ts.TypeFlags.Object ->
                            match tsTyp.symbol.declarations with
                            | Some declarations when declarations.Count = 1 ->
                                reader.ReadNode declarations[0]
                            | _ -> GlueType.Primitive GluePrimitive.Any
                        | _ -> primitiveOfFlags tsTyp

                let documentation = reader.ReadDocumentationFromNode exportNode

                let isPublished = isDefaultOfVariable && publishedName.IsSome

                // `export=` marks the export of the module itself, followed by the variable name
                let name =
                    if isExportEqualsOfVariable then
                        "export=" + identiferNode.getText ()
                    elif isPublished then
                        publishedName.Value
                    elif isDefaultOfVariable then
                        packageName |> Option.defaultValue (identiferNode.getText ())
                    else
                        identiferNode.getText ()

                let declaration =
                    match typ with
                    // `declare const yargs: (args?: string[]) => Argv; export default yargs` is
                    // the function
                    | GlueType.FunctionType functionType when isDefaultOfVariable ->
                        ({
                            Documentation = documentation
                            IsDeclared = true
                            Name = name
                            Type = functionType.Type
                            Parameters = functionType.Parameters
                            TypeParameters = functionType.TypeParameters
                        }
                        : GlueFunctionDeclaration)
                        |> GlueType.FunctionDeclaration
                    | _ ->
                        ({
                            Documentation = documentation
                            Name = name
                            Type = typ
                        }
                        : GlueVariable)
                        |> GlueType.Variable

                if isPublished then
                    declaration
                else
                    GlueType.ExportDefault declaration

            | _ -> GlueType.Discard
        else
            GlueType.Discard

    | None -> GlueType.Discard
