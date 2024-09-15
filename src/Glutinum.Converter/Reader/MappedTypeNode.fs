module Glutinum.Converter.Reader.MappedTypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readMappedTypeNode
    (reader: ITypeScriptReader)
    (mappedTypeNode: Ts.MappedTypeNode)
    : GlueType
    =

    let typParam =
        // TODO: Make a single reader.ReadTypeParameter method
        let typeParameters =
            reader.ReadTypeParameters(
                Some(ResizeArray([ mappedTypeNode.typeParameter ]))
            )

        match typeParameters with
        | [ tp ] -> tp
        | _ ->
            Utils.generateReaderError
                "readMappedTypeNode"
                $"Expected exactly one type parameter but was {List.length typeParameters}"
                mappedTypeNode
            |> failwith

    {
        TypeParameter = typParam
        Type = mappedTypeNode.``type`` |> Option.map reader.ReadNode
    }
    |> GlueType.MappedType
