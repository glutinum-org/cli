module Glutinum.Converter.Reader.Parameters

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.Utils
open TypeScript

let readParameters
    (reader: ITypeScriptReader)
    (parameters: ResizeArray<Ts.ParameterDeclaration>)
    : GlueParameter list
    =
    parameters
    |> Seq.toList
    |> List.mapi (fun index parameter ->
        let nameNode = unbox<Ts.Node> parameter.name

        let name =
            match nameNode.kind with
            | Ts.SyntaxKind.Identifier -> identifierText nameNode
            | Ts.SyntaxKind.ObjectBindingPattern -> $"arg%i{index}"
            | _ ->
                Report.readerError ("name", $"Unsupported kind %s{nameNode.kind.Name}", nameNode)
                |> reader.Warnings.Add

                $"arg%i{index}"

        {
            Name = name
            IsOptional = parameter.questionToken.IsSome
            IsSpread = parameter.dotDotDotToken.IsSome
            Type = reader.ReadTypeNode parameter.``type``
        }
    )
