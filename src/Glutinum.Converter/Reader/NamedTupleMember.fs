module Glutinum.Converter.Reader.NamedTupleMember

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readNamedTupleMember
    (reader: ITypeScriptReader)
    (declaration: Ts.NamedTupleMember)
    : GlueType
    =

    ({
        Name = declaration.name.getText ()
        Type = reader.ReadTypeNode declaration.``type``
    }
    : NamedTupleType)
    |> GlueType.NamedTupleType
