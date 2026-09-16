module Glutinum.Converter.Reader.ExportAssignment

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

let readExportAssignment (reader: ITypeScriptReader) (exportNode: Ts.ExportAssignment) : GlueType =

    // Not sure why we don't have symbol property in the type definition
    let symbolOpt: Ts.Symbol option = exportNode?symbol

    // `export = path` of a variable: the module is the variable
    let isExportEqualsOfVariable =
        exportNode.isExportEquals = Some true
        && exportNode.expression.kind = Ts.SyntaxKind.Identifier
        && (
            match reader.checker.getSymbolAtLocation exportNode.expression with
            | Some symbol ->
                match symbol.valueDeclaration with
                | Some declaration -> declaration.kind = Ts.SyntaxKind.VariableDeclaration
                | None -> false
            | None -> false
        )

    match symbolOpt with
    | Some symbol ->
        if symbol.name = "default" || isExportEqualsOfVariable then
            match exportNode.expression.kind with
            | Ts.SyntaxKind.Identifier ->
                // Get the identifier node, so we know what name to use
                // for naming the default export variable
                let identiferNode: Ts.Identifier = !!exportNode.expression
                // The declared type of the variable keeps the module path of a reference
                let declaredTypeNode =
                    if isExportEqualsOfVariable then
                        reader.checker.getSymbolAtLocation exportNode.expression
                        |> Option.bind _.valueDeclaration
                        |> Option.bind (fun declaration ->
                            (declaration :?> Ts.VariableDeclaration).``type``
                        )
                    else
                        None

                // Determine the type of the default export
                let typ =
                    let tsTyp = reader.checker.getTypeAtLocation (exportNode.expression)

                    match declaredTypeNode with
                    | Some typeNode -> reader.ReadTypeNode typeNode
                    | None ->

                        match tsTyp.flags with
                        | HasTypeFlags Ts.TypeFlags.Object ->
                            // Try to find the declaration of the type, to get more information about it
                            match tsTyp.symbol.declarations with
                            | Some declarations ->
                                if declarations.Count = 1 then
                                    reader.ReadNode declarations[0]
                                else
                                    GlueType.Primitive GluePrimitive.Any

                            | None -> GlueType.Primitive GluePrimitive.Any
                        | HasTypeFlags Ts.TypeFlags.String ->
                            GlueType.Primitive GluePrimitive.String
                        | HasTypeFlags Ts.TypeFlags.Number ->
                            GlueType.Primitive GluePrimitive.Number
                        | HasTypeFlags Ts.TypeFlags.Boolean -> GlueType.Primitive GluePrimitive.Bool
                        | HasTypeFlags Ts.TypeFlags.Any -> GlueType.Primitive GluePrimitive.Any
                        | HasTypeFlags Ts.TypeFlags.Void -> GlueType.Primitive GluePrimitive.Unit
                        | _ -> GlueType.Primitive GluePrimitive.Any

                ({
                    Documentation = reader.ReadDocumentationFromNode exportNode
                    // `export=` marks the export of the module itself, followed by the variable name
                    Name =
                        if isExportEqualsOfVariable then
                            "export=" + identiferNode.getText ()
                        else
                            identiferNode.getText ()
                    Type = typ
                }
                : GlueVariable)
                |> GlueType.Variable
                |> GlueType.ExportDefault

            | _ -> GlueType.Discard
        else
            GlueType.Discard

    | None -> GlueType.Discard
