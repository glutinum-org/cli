module Glutinum.Converter.Reader.TypeOperatorNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let rec private removeParenthesizedType (node: Ts.TypeNode) =
    match node.kind with
    | Ts.SyntaxKind.ParenthesizedType ->
        removeParenthesizedType (node :?> Ts.ParenthesizedTypeNode).``type``
    | _ -> node

// `class ChartView { call(name: keyof ChartView) }`: the keys of a declaration being read
let private declarationsInProgress = ResizeArray<Ts.Node>()

let readTypeOperatorNode (reader: ITypeScriptReader) (node: Ts.TypeOperatorNode) =

    match node.operator with
    | Ts.SyntaxKind.KeyOfKeyword ->
        let operandNode = removeParenthesizedType node.``type``

        match operandNode.kind with
        | Ts.SyntaxKind.TypeReference ->
            let typeReferenceNode = operandNode :?> Ts.TypeReferenceNode

            // TODO: Remove unboxing
            let symbolOpt = reader.checker.getSymbolAtLocation !!typeReferenceNode.typeName

            match symbolOpt with
            // A node synthesized by `typeToTypeNode` doesn't always carry its symbol
            | None when (unbox<Ts.Node> node).pos < 0 -> GlueType.KeyOf GlueType.Discard

            | None ->
                Report.readerError ("type operator (keyof)", "Missing symbol", node) |> failwith

            | Some symbol ->
                match symbol.declarations with
                | Some declarations when declarations.Count > 0 ->
                    // `interface ChartView` merged with `class ChartView`: the class has the members
                    let declaration =
                        declarations
                        |> Seq.tryFind (fun declaration ->
                            declaration.kind = Ts.SyntaxKind.ClassDeclaration
                        )
                        |> Option.defaultValue declarations[0]

                    // The keys of a type parameter are known once it is substituted
                    if declaration.kind = Ts.SyntaxKind.TypeParameter then
                        GlueType.KeyOf(GlueType.TypeParameter symbol.name)
                    elif declarationsInProgress.Contains declaration then
                        GlueType.KeyOf GlueType.Discard
                    else
                        declarationsInProgress.Add declaration

                        try
                            reader.ReadNode declaration |> GlueType.KeyOf
                        finally
                            declarationsInProgress.RemoveAt(declarationsInProgress.Count - 1)
                | Some _ ->
                    Report.readerError ("type operator (keyof)", "Missing declarations", node)
                    |> failwith

                | None ->
                    Report.readerError ("type operator (keyof)", "Missing declarations", node)
                    |> failwith

        | Ts.SyntaxKind.TypeQuery ->
            let typeQueryNode = operandNode :?> Ts.TypeQueryNode

            TypeQueryNode.readTypeQueryNode reader typeQueryNode |> GlueType.KeyOf

        // `keyof T["options"]`, the keys can't be known
        | Ts.SyntaxKind.IndexedAccessType
        | Ts.SyntaxKind.IntersectionType -> GlueType.KeyOf GlueType.Discard

        | Ts.SyntaxKind.AnyKeyword ->
            GlueType.Union(
                GlueTypeUnion
                    [
                        GlueType.Primitive GluePrimitive.String
                        GlueType.Primitive GluePrimitive.Number
                        GlueType.Primitive GluePrimitive.Symbol
                    ]
            )

        | _ ->
            Report.readerError (
                "type operator (keyof)",
                $"Was expecting a type reference or a type query instead got a Node of type %s{operandNode.kind.Name}",
                node
            )
            |> reader.Warnings.Add

            GlueType.Primitive GluePrimitive.Any

    | Ts.SyntaxKind.ReadonlyKeyword -> reader.ReadTypeNode node.``type`` |> GlueType.ReadOnly

    | Ts.SyntaxKind.UniqueKeyword -> reader.ReadTypeNode node.``type``

    | _ ->
        Report.readerError ("type operator", $"Unsupported operator %s{node.operator.Name}", node)
        |> reader.Warnings.Add

        GlueType.Primitive GluePrimitive.Any
