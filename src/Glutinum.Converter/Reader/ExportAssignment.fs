module Glutinum.Converter.Reader.ExportAssignment

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

let readExportAssignment
    (reader: ITypeScriptReader)
    (exportNode: Ts.ExportAssignment)
    : GlueType
    =

    // Not sure why we don't have symbol property in the type definition
    let symbolOpt: Ts.Symbol option = exportNode?symbol

    match symbolOpt with
    | Some symbol ->
        if symbol.name = "default" then
            match exportNode.expression.kind with
            | Ts.SyntaxKind.Identifier ->
                // Get the identifier node, so we know what name to use
                // for naming the default export variable
                let identiferNode: Ts.Identifier = !!exportNode.expression
                // Determine the type of the default export
                let typ =
                    let tsTyp =
                        reader.checker.getTypeAtLocation (exportNode.expression)

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
                    | HasTypeFlags Ts.TypeFlags.Boolean ->
                        GlueType.Primitive GluePrimitive.Bool
                    | HasTypeFlags Ts.TypeFlags.Any ->
                        GlueType.Primitive GluePrimitive.Any
                    | HasTypeFlags Ts.TypeFlags.Void ->
                        GlueType.Primitive GluePrimitive.Unit
                    | _ -> GlueType.Primitive GluePrimitive.Any

                {
                    Name = identiferNode.getText ()
                    Type = typ
                }
                |> GlueType.Variable
                |> GlueType.ExportDefault

            | _ -> GlueType.Discard
        else
            GlueType.Discard

    | None -> GlueType.Discard
