module Glutinum.Converter.Reader.NamedTupleMember

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.Utils
open TypeScript

let readNamedTupleMember (reader: ITypeScriptReader) (declaration: Ts.NamedTupleMember) : GlueType =

    ({
        Name = identifierText declaration.name
        Type = reader.ReadTypeNode declaration.``type``
    }
    : NamedTupleType)
    |> GlueType.NamedTupleType
