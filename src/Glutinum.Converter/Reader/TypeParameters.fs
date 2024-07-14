module Glutinum.Converter.Reader.TypeParameters

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readTypeParameters
    (reader: ITypeScriptReader)
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
                Constraint =
                    typeParameter.``constraint``
                    |> Option.map reader.ReadTypeNode
                Default = None
            }
        )
