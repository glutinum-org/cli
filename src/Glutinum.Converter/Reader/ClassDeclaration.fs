module Glutinum.Converter.Reader.ClassDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.Utils
open TypeScript

let readClassDeclaration
    (reader: ITypeScriptReader)
    (classDeclaration: Ts.ClassDeclaration)
    : GlueType
    =

    let name = unbox<Ts.Identifier> classDeclaration.name

    let members = classDeclaration.members |> Seq.toList

    let constructors, members =
        members
        |> List.partition (fun m ->
            match m.kind with
            | Ts.SyntaxKind.Constructor -> true
            | _ -> false
        )

    let constructors =
        constructors
        |> List.choose (fun m ->
            match m.kind with
            | Ts.SyntaxKind.Constructor ->
                let constructor = m :?> Ts.ConstructorDeclaration

                ({
                    Documentation =
                        reader.ReadDocumentationFromSignature constructor
                    Parameters = reader.ReadParameters constructor.parameters
                }
                : GlueConstructor)
                |> Some

            | _ -> None
        )

    let members = members |> Seq.toList |> List.map reader.ReadDeclaration

    let isDefaultExport =
        ModifierUtil.HasModifier(
            classDeclaration.modifiers,
            Ts.SyntaxKind.DefaultKeyword
        )

    let classDeclaration =
        {
            Name = name.getText ()
            Constructors = constructors
            Members = members
            TypeParameters = []
        }
        |> GlueType.ClassDeclaration

    if isDefaultExport then
        classDeclaration |> GlueType.ExportDefault
    else
        classDeclaration
