module Glutinum.Converter.Reader.FunctionDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readFunctionDeclaration
    (reader: ITypeScriptReader)
    (declaration: Ts.FunctionDeclaration)
    : GlueType
    =

    let isDeclared =
        match declaration.modifiers with
        | Some modifiers ->
            modifiers
            |> Seq.exists (fun modifier -> modifier?kind = Ts.SyntaxKind.DeclareKeyword)
        | None -> false

    let isDefaultExport =
        Utils.ModifierUtil.HasModifier(declaration.modifiers, Ts.SyntaxKind.DefaultKeyword)

    let name =
        match declaration.name with
        | Some name -> name.getText ()
        | None when isDefaultExport -> "defaultExport"
        | None ->
            Report.readerError ("function declaration", "Missing name", declaration)
            |> failwith

    let functionDeclaration =
        {
            Documentation = reader.ReadDocumentationFromSignature declaration
            IsDeclared = isDeclared
            Name = name
            Type = reader.ReadTypeNode declaration.``type``
            Parameters = reader.ReadParameters declaration.parameters
            TypeParameters =
                reader.ReadTypeParameters declaration.typeParameters
                |> List.mapi (fun index typeParameter ->
                    match
                        Utils.tryReadKeyOfConstraint
                            reader
                            declaration.typeParameters.Value.[index]
                    with
                    | Some keyOf ->
                        { typeParameter with
                            Constraint = Some keyOf
                        }
                    | None -> typeParameter
                )
        }

    if isDefaultExport then
        GlueType.ExportDefault(GlueType.FunctionDeclaration functionDeclaration)
    else
        GlueType.FunctionDeclaration functionDeclaration
