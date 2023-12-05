module Glutinum.Converter.Reader.TypeParameters

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readTypeParameters
    (reader: TypeScriptReader)
    (typeParameters: ResizeArray<Ts.TypeParameterDeclaration> option)
    : GlueTypeParameter list
    =
    match typeParameters with
    | None -> []
    | Some typeParameters ->
        typeParameters
        |> Seq.toList
        |> List.map (fun typeParameter ->
            {
                Name = typeParameter.name.getText ()
                Constraint = None
                Default = None
            }
        )
