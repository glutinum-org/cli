module Glutinum.Converter.Reader.Parameters

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readParameters
    (reader: ITypeScriptReader)
    (parameters: ResizeArray<Ts.ParameterDeclaration>)
    : GlueParameter list
    =
    parameters
    |> Seq.toList
    |> List.map (fun parameter ->
        let name = unbox<Ts.Identifier> parameter.name

        {
            Name = name.getText ()
            IsOptional = parameter.questionToken.IsSome
            IsSpread = parameter.dotDotDotToken.IsSome
            Type = reader.ReadTypeNode parameter.``type``
        }
    )
