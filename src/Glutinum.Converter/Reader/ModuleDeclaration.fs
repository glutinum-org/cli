module Glutinum.Converter.Reader.ModuleDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readModuleDeclaration
    (reader: TypeScriptReader)
    (declaration: Ts.ModuleDeclaration)
    : GlueModuleDeclaration
    =

    let name = unbox<Ts.Identifier> declaration.name
    let children = declaration.getChildren ()

    let isNamespace =
        children
        |> Seq.exists (fun node -> node.kind = Ts.SyntaxKind.NamespaceKeyword)

    let types =
        children
        |> Seq.choose (fun child ->
            match child.kind with
            | Ts.SyntaxKind.ModuleBlock ->
                let moduleBlock = child :?> Ts.ModuleBlock

                moduleBlock.statements
                |> List.ofSeq
                |> List.map reader.ReadNode
                |> Some

            | Ts.SyntaxKind.NamespaceKeyword
            | _ -> None
        )
        |> Seq.concat
        |> Seq.toList

    {
        Name = name.getText ()
        IsNamespace = isNamespace
        IsRecursive = false
        Types = types
    }
