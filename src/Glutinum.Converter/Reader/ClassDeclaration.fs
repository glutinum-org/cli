module Glutinum.Converter.Reader.ClassDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readClassDeclaration
    (reader: ITypeScriptReader)
    (classDeclaration: Ts.ClassDeclaration)
    : GlueClassDeclaration
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

                reader.ReadParameters constructor.parameters
                |> GlueConstructor
                |> Some

            | _ -> None
        )

    let members = members |> Seq.toList |> List.map reader.ReadDeclaration

    {
        Name = name.getText ()
        Constructors = constructors
        Members = members
        TypeParameters = []
    }
