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
        reader.ReadTypeParameters(
            Some(ResizeArray([ mappedTypeNode.typeParameter ]))
        )
        |> List.head

    {
        TypeParameter = typParam
        Type = mappedTypeNode.``type`` |> Option.map reader.ReadNode
    }
    |> GlueType.MappedType
