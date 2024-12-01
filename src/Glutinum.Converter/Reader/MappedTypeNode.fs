module Glutinum.Converter.Reader.MappedTypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open FsToolkit.ErrorHandling

let readMappedTypeNode (reader: ITypeScriptReader) (mappedTypeNode: Ts.MappedTypeNode) : GlueType =
    result {
        let! typParam =
            // TODO: Make a single reader.ReadTypeParameter method
            let typeParameters =
                reader.ReadTypeParameters(Some(ResizeArray([ mappedTypeNode.typeParameter ])))

            match typeParameters with
            | [ tp ] -> Ok tp
            | _ ->
                Report.readerError (
                    "readMappedTypeNode",
                    $"Expected exactly one type parameter but was {List.length typeParameters}",
                    mappedTypeNode
                )
                |> Error

        return
            {
                TypeParameter = typParam
                Type = mappedTypeNode.``type`` |> Option.map reader.ReadNode
            }
            |> GlueType.MappedType
    }
    |> function
        | Ok glueType -> glueType
        | Error warning ->
            reader.Warnings.Add warning
            GlueType.Discard
