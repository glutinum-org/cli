module Glutinum.Converter.Reader.NamedDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readNamedDeclaration
    (reader: ITypeScriptReader)
    (declaration: Ts.NamedDeclaration)
    : GlueMember
    =
    match declaration.kind with
    | Ts.SyntaxKind.PropertySignature ->

        let propertySignature = declaration :?> Ts.PropertySignature
        let name = unbox<Ts.Node> propertySignature.name

        let accessor =
            match propertySignature.modifiers with
            | Some modifiers ->
                modifiers
                |> Seq.exists (fun modifier ->
                    modifier?kind = Ts.SyntaxKind.ReadonlyKeyword
                )
                |> function
                    | true -> GlueAccessor.ReadOnly
                    | false -> GlueAccessor.ReadWrite
            | None -> GlueAccessor.ReadWrite

        ({
            Name = name.getText ()
            Type = reader.ReadTypeNode propertySignature.``type``
            IsOptional = propertySignature.questionToken.IsSome
            IsStatic = false
            Accessor = accessor
        }
        : GlueProperty)
        |> GlueMember.Property

    | Ts.SyntaxKind.CallSignature ->
        let callSignature = declaration :?> Ts.CallSignatureDeclaration

        ({
            Parameters = reader.ReadParameters callSignature.parameters
            Type = reader.ReadTypeNode callSignature.``type``
        }
        : GlueCallSignature)
        |> GlueMember.CallSignature

    | Ts.SyntaxKind.MethodDeclaration ->
        let methodDeclaration = declaration :?> Ts.MethodDeclaration
        let name = unbox<Ts.Identifier> methodDeclaration.name

        {
            Name = name.getText ()
            Parameters = reader.ReadParameters methodDeclaration.parameters
            Type = reader.ReadTypeNode methodDeclaration.``type``
            IsOptional = methodDeclaration.questionToken.IsSome
            IsStatic =
                methodDeclaration.modifiers
                |> Option.map (fun modifiers ->
                    modifiers
                    |> Seq.exists (fun modifier ->
                        modifier?kind = Ts.SyntaxKind.StaticKeyword
                    )
                )
                |> Option.defaultValue false
        }
        |> GlueMember.Method

    | Ts.SyntaxKind.IndexSignature ->
        let indexSignature = declaration :?> Ts.IndexSignatureDeclaration

        ({
            Parameters = reader.ReadParameters indexSignature.parameters
            Type = reader.ReadTypeNode(Some indexSignature.``type``)
        }
        : GlueIndexSignature)
        |> GlueMember.IndexSignature

    | Ts.SyntaxKind.MethodSignature ->
        let methodSignature = declaration :?> Ts.MethodSignature
        let name = unbox<Ts.Identifier> methodSignature.name

        ({
            Name = name.getText ()
            Parameters = reader.ReadParameters methodSignature.parameters
            Type = reader.ReadTypeNode methodSignature.``type``
        }
        : GlueMethodSignature)
        |> GlueMember.MethodSignature

    | _ ->
        Utils.generateReaderError
            "named declaration"
            $"Unsupported kind %A{declaration.kind}"
            declaration
        |> TypeScriptReaderException
        |> raise
