module Glutinum.Converter.Reader.ModuleDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readModuleDeclaration
    (reader: ITypeScriptReader)
    (declaration: Ts.ModuleDeclaration)
    : GlueModuleDeclaration
    =

    let name = unbox<Ts.Identifier> declaration.name
    let children = declaration.getChildren ()

    // For `namespace A.B {}`, TypeScript creates `B` as the body of `A` and the keyword belongs to `A`
    let isBodyOfDottedName =
        declaration.parent?kind = Ts.SyntaxKind.ModuleDeclaration
        && obj.ReferenceEquals(declaration.parent?body, declaration)

    let isNamespace =
        isBodyOfDottedName
        || children |> Seq.exists (fun node -> node.kind = Ts.SyntaxKind.NamespaceKeyword)

    let types =
        children
        |> Seq.choose (fun child ->
            match child.kind with
            | Ts.SyntaxKind.ModuleBlock ->
                let moduleBlock = child :?> Ts.ModuleBlock

                moduleBlock.statements |> List.ofSeq |> List.map reader.ReadNode |> Some

            | Ts.SyntaxKind.ModuleDeclaration -> reader.ReadNode child |> List.singleton |> Some

            | Ts.SyntaxKind.NamespaceKeyword
            | _ -> None
        )
        |> Seq.concat
        |> Seq.toList

    // A namespace of the ambient module a file is made of is at the top level of the file
    let isTopLevel =
        match declaration.parent?kind with
        | Ts.SyntaxKind.SourceFile -> true
        | Ts.SyntaxKind.ModuleBlock ->
            let moduleBlock: Ts.Node = !!declaration.parent

            moduleBlock.parent.kind = Ts.SyntaxKind.ModuleDeclaration
            && Utils.isPromotedAmbientModule (moduleBlock.parent :?> Ts.ModuleDeclaration)
        | _ -> false

    {
        Documentation = reader.ReadDocumentationFromNode declaration
        Name = name.getText ()
        IsTopLevel = isTopLevel
        IsNamespace = isNamespace
        IsRecursive = false
        Types = types
    }
