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

    let heritageClauses =
        match declaration.heritageClauses with
        | Some heritageClauses ->
            heritageClauses
            |> Seq.toList
            |> List.collect (fun clause ->
                clause.types |> Seq.toList |> List.map reader.ReadTypeNode
            )
        | None -> []

    {
        Name = declaration.name.getText ()
        Members = members
        TypeParameters = reader.ReadTypeParameters declaration.typeParameters
        HeritageClauses = heritageClauses
    }
