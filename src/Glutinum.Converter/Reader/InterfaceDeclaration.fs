module Glutinum.Converter.Reader.InterfaceDeclaration

open TypeScript
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types

let readInterfaceDeclaration
    (reader: TypeScriptReader)
    (declaration: Ts.InterfaceDeclaration)
    : GlueInterface
    =

    let members =
        declaration.members
        |> Seq.toList
        |> List.map reader.ReadNamedDeclaration

    {
        Name = declaration.name.getText ()
        Members = members
        TypeParameters = []
    }
