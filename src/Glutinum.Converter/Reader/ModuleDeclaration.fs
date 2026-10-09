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

    let isAmbientModule =
        (unbox<Ts.Node> declaration.name).kind = Ts.SyntaxKind.StringLiteral

    let isExportAssigned =
        declaration.getSourceFile().statements
        |> Seq.exists (fun statement ->
            statement.kind = Ts.SyntaxKind.ExportAssignment
            && (let expression: Ts.Node = (statement :?> Ts.ExportAssignment).expression

                expression.kind = Ts.SyntaxKind.Identifier
                && Utils.identifierText expression = name.getText ())
        )

    let types =
        children
        |> Seq.choose (fun child ->
            match child.kind with
            | Ts.SyntaxKind.ModuleBlock ->
                let moduleBlock = child :?> Ts.ModuleBlock

                moduleBlock.statements
                |> List.ofSeq
                |> List.filter (fun statement ->
                    not (
                        Utils.isMergedInterfaceDeclaration
                            reader.checker
                            reader.PackageContext
                            statement
                    )
                )
                |> List.collect (fun statement ->
                    match statement.kind with
                    | Ts.SyntaxKind.ExportDeclaration ->
                        let exportDeclaration = statement :?> Ts.ExportDeclaration

                        match exportDeclaration.exportClause with
                        | Some exportClause when
                            exportDeclaration.moduleSpecifier.IsNone
                            && exportClause?kind = Ts.SyntaxKind.NamedExports
                            ->
                            let namedExports: Ts.NamedExports = !!exportClause

                            namedExports.elements |> Seq.toList |> List.map reader.ReadNode
                        | _ -> []
                    | _ -> [ reader.ReadNode statement ]
                )
                |> Some

            | Ts.SyntaxKind.ModuleDeclaration -> reader.ReadNode child |> List.singleton |> Some

            | Ts.SyntaxKind.NamespaceKeyword
            | _ -> None
        )
        |> Seq.concat
        |> Seq.toList

    {
        Documentation = reader.ReadDocumentationFromNode declaration
        Name = name.getText ()
        IsTopLevel = Utils.isTopLevelModuleDeclaration reader.PackageContext declaration
        IsNamespace = isNamespace
        IsGlobal = reader.PackageContext.IsSome && Utils.isGlobalAugmentation declaration
        IsExported =
            isAmbientModule
            || isBodyOfDottedName
            || isExportAssigned
            || Utils.isExportedDeclaration declaration (Set.singleton (name.getText ()))
        IsRecursive = false
        Types = types
    }
