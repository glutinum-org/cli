module Glutinum.Converter.Reader.TypeParameters

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.Utils
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
                Name = identifierText typeParameter.name
                Constraint = typeParameter.``constraint`` |> Option.map reader.ReadTypeNode
                Default = typeParameter.``default`` |> Option.map reader.ReadTypeNode
            }
        )
