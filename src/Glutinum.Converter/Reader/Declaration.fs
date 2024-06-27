module Glutinum.Converter.Reader.Declaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Utils
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readDeclaration
    (reader: ITypeScriptReader)
    (declaration: Ts.Declaration)
    : GlueMember
    =
    match declaration.kind with
    | Ts.SyntaxKind.PropertySignature ->

        let propertySignature = declaration :?> Ts.PropertySignature
        let name = unbox<Ts.Node> propertySignature.name

        ({
            Name = name.getText ()
            Documentation = reader.ReadDocumentationFromNode name
            Type = reader.ReadTypeNode propertySignature.``type``
            IsOptional = propertySignature.questionToken.IsSome
            IsStatic = false
            Accessor = ModifierUtil.GetAccessor propertySignature.modifiers
            IsPrivate = false
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
            Documentation = reader.ReadDocumentationFromNode name
            Name = name.getText ()
            Parameters = reader.ReadParameters methodSignature.parameters
            Type = reader.ReadTypeNode methodSignature.``type``
        }
        : GlueMethodSignature)
        |> GlueMember.MethodSignature

    | Ts.SyntaxKind.ConstructSignature ->
        let constructSignature =
            declaration :?> Ts.ConstructSignatureDeclaration

        ({
            Parameters = reader.ReadParameters constructSignature.parameters
            Type = reader.ReadTypeNode constructSignature.``type``
        }
        : GlueConstructSignature)
        |> GlueMember.ConstructSignature

    | Ts.SyntaxKind.PropertyDeclaration ->
        let propertyDeclaration = declaration :?> Ts.PropertyDeclaration
        let name = unbox<Ts.Identifier> propertyDeclaration.name

        let hasPrivateModifier =
            ModifierUtil.HasModifier(
                propertyDeclaration.modifiers,
                Ts.SyntaxKind.PrivateKeyword
            )

        let isPrivateIdentifier = name.kind = Ts.SyntaxKind.PrivateIdentifier

        ({
            Name = name.getText ()
            Documentation = reader.ReadDocumentationFromNode name
            Type = reader.ReadTypeNode propertyDeclaration.``type``
            IsOptional = propertyDeclaration.questionToken.IsSome
            IsStatic =
                ModifierUtil.HasModifier(
                    propertyDeclaration.modifiers,
                    Ts.SyntaxKind.StaticKeyword
                )
            Accessor = ModifierUtil.GetAccessor propertyDeclaration.modifiers
            IsPrivate = hasPrivateModifier || isPrivateIdentifier
        }
        : GlueProperty)
        |> GlueMember.Property

    | _ ->
        generateReaderError
            "declaration"
            $"Unsupported kind %A{declaration.kind}"
            declaration
        |> failwith
