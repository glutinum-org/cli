module Glutinum.Converter.Reader.Parameters

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open TypeScriptHelpers

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
            | Ts.SyntaxKind.Identifier ->
                let name = nameNode :?> Ts.Identifier
                name.getText ()
            | Ts.SyntaxKind.ObjectBindingPattern -> $"arg%i{index}"
            | _ ->
                Utils.generateReaderError
                    "name"
                    $"Unsupported kind {SyntaxKind.name nameNode.kind} in {__SOURCE_FILE__}"
                    nameNode
                |> reader.Warnings.Add

                $"arg%i{index}"

        {
            Name = name
            IsOptional = parameter.questionToken.IsSome
            IsSpread = parameter.dotDotDotToken.IsSome
            Type = reader.ReadTypeNode parameter.``type``
        }
    )
