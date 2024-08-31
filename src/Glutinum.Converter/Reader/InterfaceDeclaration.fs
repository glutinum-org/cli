module Glutinum.Converter.Reader.InterfaceDeclaration

open TypeScript
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types

let readInterfaceDeclaration
    (reader: ITypeScriptReader)
    (declaration: Ts.InterfaceDeclaration)
    : GlueInterface
    =

    let members =
        declaration.members |> Seq.toList |> List.map reader.ReadDeclaration

    {
        FullName = Utils.getFullNameOrEmpty reader.checker declaration
        Name = declaration.name.getText ()
        Members = members
        TypeParameters = reader.ReadTypeParameters declaration.typeParameters
        HeritageClauses =
            Utils.readHeritageClauses reader declaration.heritageClauses
    }
