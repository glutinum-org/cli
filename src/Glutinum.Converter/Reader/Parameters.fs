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

        let isSpread = parameter.dotDotDotToken.IsSome

        // `...args: [a: string, b?: number, ...rest: boolean[]]` is one parameter per element,
        // `...args: Parameters<F>` too once the checker resolves the tuple
        let tupleElements =
            match parameter.``type`` with
            | Some typeNode when isSpread && typeNode.kind = Ts.SyntaxKind.TupleType ->
                (typeNode :?> Ts.TupleTypeNode).elements |> Seq.toList |> Some
            | Some typeNode when
                isSpread && typeNode.kind = Ts.SyntaxKind.TypeReference && typeNode.pos >= 0
                ->
                let typ = reader.checker.getTypeFromTypeNode typeNode

                if isTupleType typ then
                    let flags =
                        Ts.NodeBuilderFlags.NoTruncation
                        ||| Ts.NodeBuilderFlags.UseAliasDefinedOutsideCurrentScope

                    match reader.checker.typeToTypeNode (typ, None, Some flags) with
                    | Some tupleNode when tupleNode.kind = Ts.SyntaxKind.TupleType ->
                        (tupleNode :?> Ts.TupleTypeNode).elements |> Seq.toList |> Some
                    | _ -> None
                else
                    None
            | _ -> None

        match tupleElements with
        | Some elements ->
            elements
            |> List.mapi (fun elementIndex element ->
                let element = unbox<Ts.Node> element

                match element.kind with
                | Ts.SyntaxKind.NamedTupleMember ->
                    let namedMember = element :?> Ts.NamedTupleMember

                    {
                        Name = identifierText namedMember.name
                        IsOptional = namedMember.questionToken.IsSome
                        IsSpread = namedMember.dotDotDotToken.IsSome
                        Type = reader.ReadTypeNode namedMember.``type``
                    }
                | Ts.SyntaxKind.OptionalType ->
                    {
                        Name = $"%s{name}%i{elementIndex}"
                        IsOptional = true
                        IsSpread = false
                        Type = reader.ReadTypeNode (element :?> Ts.OptionalTypeNode).``type``
                    }
                | Ts.SyntaxKind.RestType ->
                    {
                        Name = $"%s{name}%i{elementIndex}"
                        IsOptional = false
                        IsSpread = true
                        Type = reader.ReadTypeNode (element :?> Ts.RestTypeNode).``type``
                    }
                | _ ->
                    {
                        Name = $"%s{name}%i{elementIndex}"
                        IsOptional = false
                        IsSpread = false
                        Type = reader.ReadTypeNode(element :?> Ts.TypeNode)
                    }
            )
        | None ->
            [
                {
                    Name = name
                    IsOptional = parameter.questionToken.IsSome
                    IsSpread = isSpread
                    Type = reader.ReadTypeNode parameter.``type``
                }
            ]
    )
    |> List.concat
