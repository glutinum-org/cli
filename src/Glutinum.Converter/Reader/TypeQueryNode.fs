module Glutinum.Converter.Reader.TypeQueryNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils
open FsToolkit.ErrorHandling

let readTypeQueryNode (reader: ITypeScriptReader) (typeQueryNode: Ts.TypeQueryNode) =

    let checker = reader.checker
    let typ = checker.getTypeAtLocation !!typeQueryNode.exprName

    match typ.flags with
    | HasTypeFlags Ts.TypeFlags.Object ->
        // This is safe as both cases have a `kind` field
        let exprNameKind: Ts.SyntaxKind = typeQueryNode.exprName?kind

        match typ.getSymbol (), exprNameKind with
        | None, Ts.SyntaxKind.Identifier ->

            let exprName: Ts.Identifier = !!typeQueryNode.exprName

            result {
                let! aliasSymbol =
                    checker.getSymbolAtLocation exprName
                    |> Result.requireSome (
                        Report.readerError (
                            "type node (TypeQuery)",
                            "Missing symbol",
                            typeQueryNode
                        )
                    )

                let! declarations =
                    aliasSymbol.declarations
                    |> Result.requireSome (
                        Report.readerError (
                            "type node (TypeQuery)",
                            "Missing declarations",
                            typeQueryNode
                        )
                    )

                let! declaration =
                    if declarations.Count <> 1 then
                        Report.readerError (
                            "type node (TypeQuery)",
                            "Expected exactly one declaration",
                            typeQueryNode
                        )
                        |> Error

                    else
                        Ok(declarations.[0])

                let! variableDeclaration =
                    match declaration.kind with
                    | Ts.SyntaxKind.VariableDeclaration ->
                        Ok(declaration :?> Ts.VariableDeclaration)

                    | unsupported ->
                        Report.readerError (
                            "type node (TypeQuery)",
                            $"Unsupported declaration kind {unsupported.Name}",
                            typeQueryNode
                        )
                        |> Error

                let! typeNode =
                    variableDeclaration.``type``
                    |> Result.requireSome (
                        Report.readerError ("type node (TypeQuery)", "Missing type", typeQueryNode)
                    )

                match typeNode.kind with
                | Ts.SyntaxKind.TypeOperator ->
                    let typeOperatorNode = typeNode :?> Ts.TypeOperatorNode
                    return reader.ReadTypeOperatorNode typeOperatorNode

                | unsupported ->
                    return!
                        Report.readerError (
                            "type node (TypeQuery)",
                            $"Unsupported declaration kind {unsupported.Name}",
                            typeQueryNode
                        )
                        |> Error

            }
            |> function
                | Ok glueType -> glueType
                | Error warning ->
                    reader.Warnings.Add warning
                    GlueType.Discard

        | None, _ ->
            let warning =
                Report.readerError (
                    "type node (TypeQuery)",
                    "Expected an Identifier",
                    typeQueryNode
                )

            reader.Warnings.Add warning
            GlueType.Primitive GluePrimitive.Any

        | Some symbol, _ ->
            // Try to find the declaration of the type, to get more information about it
            match symbol.declarations with
            | Some declarations ->
                let declaration = declarations.[0]

                match declaration.kind with
                | Ts.SyntaxKind.ClassDeclaration ->
                    {
                        Name = symbol.name
                        Constructors = []
                        Members = []
                        TypeParameters = []
                        HeritageClauses = []
                    }
                    |> GlueType.ClassDeclaration

                // We don't support TypeQuery for ModuleDeclaration yet
                // See https://github.com/glutinum-org/cli/issues/70 for a possible solution
                | Ts.SyntaxKind.ModuleDeclaration -> GlueType.Discard
                | _ -> reader.ReadNode declaration

            | None -> GlueType.Primitive GluePrimitive.Any

    | HasTypeFlags Ts.TypeFlags.String -> GlueType.Primitive GluePrimitive.String

    | HasTypeFlags Ts.TypeFlags.Number -> GlueType.Primitive GluePrimitive.Number

    | HasTypeFlags Ts.TypeFlags.Boolean -> GlueType.Primitive GluePrimitive.Bool

    | HasTypeFlags Ts.TypeFlags.Any -> GlueType.Primitive GluePrimitive.Any

    | HasTypeFlags Ts.TypeFlags.Void -> GlueType.Primitive GluePrimitive.Unit

    | _ -> GlueType.Primitive GluePrimitive.Any
