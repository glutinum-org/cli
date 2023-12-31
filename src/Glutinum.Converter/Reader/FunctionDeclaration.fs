module Glutinum.Converter.Reader.FunctionDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readFunctionDeclaration
    (reader: TypeScriptReader)
    (declaration: Ts.FunctionDeclaration)
    : GlueFunctionDeclaration
    =

    let isDeclared =
        match declaration.modifiers with
        | Some modifiers ->
            modifiers
            |> Seq.exists (fun modifier ->
                modifier?kind = Ts.SyntaxKind.DeclareKeyword
            )
        | None -> false

    let name =
        match declaration.name with
        | Some name -> name.getText ()
        | None -> failwith "readFunctionDeclaration: Missing name"

    {
        IsDeclared = isDeclared
        Name = name
        Type = reader.ReadTypeNode declaration.``type``
        Parameters = reader.ReadParameters declaration.parameters
        TypeParameters = reader.ReadTypeParameters declaration.typeParameters
    }
