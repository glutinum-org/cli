module Glutinum.Converter.Reader.Declaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Utils
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core
open Fable.Core.JsInterop

/// The constraints of a method stay implicit in F#, only the defaults make an overload
let private readMethodTypeParameters
    (reader: ITypeScriptReader)
    (typeParameters: ResizeArray<Ts.TypeParameterDeclaration> option)
    : GlueTypeParameter list
    =
    match typeParameters with
    | None -> []
    | Some typeParameters ->
        typeParameters
        |> Seq.toList
        |> List.map (fun typeParameter ->
            {
                Name = identifierText typeParameter.name
                Constraint = None
                Default = typeParameter.``default`` |> Option.map reader.ReadTypeNode
            }
        )

let readDeclaration (reader: ITypeScriptReader) (declaration: Ts.Declaration) : GlueMember =
    match declaration.kind with
    | Ts.SyntaxKind.PropertySignature ->

        let propertySignature = declaration :?> Ts.PropertySignature
        let name = unbox<Ts.Node> propertySignature.name

        let getName (name: Ts.PropertyName) : string =
            if not (isNull name?text) then
                name?text
            // A node synthesized by `typeToTypeNode` has no source text
            elif (unbox<Ts.Node> name).pos < 0 then
                match name?escapedText with
                | null -> "computed"
                | escapedText -> escapedText
            else
                name?getText ()

        ({
            Name = getName propertySignature.name
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
            TypeParameters = readMethodTypeParameters reader callSignature.typeParameters
            Parameters = reader.ReadParameters callSignature.parameters
            Type = reader.ReadTypeNode callSignature.``type``
        }
        : GlueCallSignature)
        |> GlueMember.CallSignature

    | Ts.SyntaxKind.MethodDeclaration ->
        let methodDeclaration = declaration :?> Ts.MethodDeclaration
        let name = unbox<Ts.Identifier> methodDeclaration.name

        {
            Name = identifierText name
            Documentation = reader.ReadDocumentationFromNode name
            TypeParameters = readMethodTypeParameters reader methodDeclaration.typeParameters
            Parameters = reader.ReadParameters methodDeclaration.parameters
            Type = reader.ReadTypeNode methodDeclaration.``type``
            IsOptional = methodDeclaration.questionToken.IsSome
            IsStatic =
                methodDeclaration.modifiers
                |> Option.map (fun modifiers ->
                    modifiers
                    |> Seq.exists (fun modifier -> modifier?kind = Ts.SyntaxKind.StaticKeyword)
                )
                |> Option.defaultValue false
        }
        |> GlueMember.Method

    | Ts.SyntaxKind.IndexSignature ->
        let indexSignature = declaration :?> Ts.IndexSignatureDeclaration

        ({
            Parameters = reader.ReadParameters indexSignature.parameters
            Type = reader.ReadTypeNode(Some indexSignature.``type``)
            IsReadOnly =
                match indexSignature.modifiers with
                | Some modifiers ->
                    modifiers
                    |> Seq.exists (fun modifier -> modifier?kind = Ts.SyntaxKind.ReadonlyKeyword)
                | None -> false
        }
        : GlueIndexSignature)
        |> GlueMember.IndexSignature

    | Ts.SyntaxKind.MethodSignature ->
        let methodSignature = declaration :?> Ts.MethodSignature
        let name = unbox<Ts.Identifier> methodSignature.name

        ({
            Documentation = reader.ReadDocumentationFromNode name
            Name = identifierText name
            TypeParameters = readMethodTypeParameters reader methodSignature.typeParameters
            Parameters = reader.ReadParameters methodSignature.parameters
            Type = reader.ReadTypeNode methodSignature.``type``
        }
        : GlueMethodSignature)
        |> GlueMember.MethodSignature

    | Ts.SyntaxKind.ConstructSignature ->
        let constructSignature = declaration :?> Ts.ConstructSignatureDeclaration

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
            ModifierUtil.HasModifier(propertyDeclaration.modifiers, Ts.SyntaxKind.PrivateKeyword)

        let isPrivateIdentifier = name.kind = Ts.SyntaxKind.PrivateIdentifier

        ({
            Name = identifierText name
            Documentation = reader.ReadDocumentationFromNode name
            Type = reader.ReadTypeNode propertyDeclaration.``type``
            IsOptional = propertyDeclaration.questionToken.IsSome
            IsStatic =
                ModifierUtil.HasModifier(propertyDeclaration.modifiers, Ts.SyntaxKind.StaticKeyword)
            Accessor = ModifierUtil.GetAccessor propertyDeclaration.modifiers
            IsPrivate = hasPrivateModifier || isPrivateIdentifier
        }
        : GlueProperty)
        |> GlueMember.Property

    | Ts.SyntaxKind.GetAccessor ->
        let getAccessorDeclaration = declaration :?> Ts.GetAccessorDeclaration
        let name = unbox<Ts.Identifier> getAccessorDeclaration.name

        let hasPrivateModifier =
            ModifierUtil.HasModifier(getAccessorDeclaration.modifiers, Ts.SyntaxKind.PrivateKeyword)

        let isPrivateIdentifier = name.kind = Ts.SyntaxKind.PrivateIdentifier

        ({
            Name = identifierText name
            Documentation = reader.ReadDocumentationFromNode name
            Type = reader.ReadTypeNode getAccessorDeclaration.``type``
            IsStatic =
                ModifierUtil.HasModifier(
                    getAccessorDeclaration.modifiers,
                    Ts.SyntaxKind.StaticKeyword
                )
            IsPrivate = hasPrivateModifier || isPrivateIdentifier
        }
        : GlueGetAccessor)
        |> GlueMember.GetAccessor

    | Ts.SyntaxKind.SetAccessor ->
        let setAccessorDeclaration = declaration :?> Ts.SetAccessorDeclaration
        let name = unbox<Ts.Identifier> setAccessorDeclaration.name

        let hasPrivateModifier =
            ModifierUtil.HasModifier(setAccessorDeclaration.modifiers, Ts.SyntaxKind.PrivateKeyword)

        let isPrivateIdentifier = name.kind = Ts.SyntaxKind.PrivateIdentifier

        ({
            Name = identifierText name
            Documentation = reader.ReadDocumentationFromNode name
            ArgumentType = reader.ReadTypeNode setAccessorDeclaration.parameters.[0].``type``
            IsStatic =
                ModifierUtil.HasModifier(
                    setAccessorDeclaration.modifiers,
                    Ts.SyntaxKind.StaticKeyword
                )
            IsPrivate = hasPrivateModifier || isPrivateIdentifier
        }
        : GlueSetAccessor)
        |> GlueMember.SetAccessor

    // `export { AtRule }` inside a namespace read as a member (`typeof ns`)
    | Ts.SyntaxKind.ExportSpecifier ->
        let exportSpecifier = declaration :?> Ts.ExportSpecifier

        let target =
            reader.checker.getExportSpecifierLocalTargetSymbol (U2.Case1 exportSpecifier)
            |> Option.bind (resolveAlias reader.checker)
            |> Option.bind (fun target ->
                match target.declarations with
                | Some declarations when declarations.Count > 0 -> Some declarations.[0]
                | _ -> None
            )

        match target with
        | Some target when
            (match target.kind with
             | Ts.SyntaxKind.FunctionDeclaration
             | Ts.SyntaxKind.VariableDeclaration
             | Ts.SyntaxKind.PropertySignature
             | Ts.SyntaxKind.PropertyDeclaration
             | Ts.SyntaxKind.MethodSignature
             | Ts.SyntaxKind.MethodDeclaration -> true
             | _ -> false)
            ->
            reader.ReadDeclaration target
        // A re-exported type is a value only when it is a class, `obj` is enough
        | _ ->
            ({
                Name = identifierText (unbox<Ts.Node> exportSpecifier.name)
                Documentation = []
                Type = GlueType.Primitive GluePrimitive.Any
                IsOptional = false
                IsStatic = false
                Accessor = GlueAccessor.ReadOnly
                IsPrivate = false
            }
            : GlueProperty)
            |> GlueMember.Property

    // `function f(): void` inside a namespace read as a member (`typeof ns`)
    | Ts.SyntaxKind.FunctionDeclaration ->
        let functionDeclaration =
            reader.ReadFunctionDeclaration(declaration :?> Ts.FunctionDeclaration)

        match functionDeclaration with
        | GlueType.FunctionDeclaration info ->
            ({
                Name = info.Name
                Documentation = info.Documentation
                TypeParameters = info.TypeParameters
                Parameters = info.Parameters
                Type = info.Type
                IsOptional = false
                IsStatic = false
            }
            : GlueMethod)
            |> GlueMember.Method
        | _ -> failwith "Expected a function declaration"

    // `const X: number` inside a namespace read as a member (`typeof ns`)
    | Ts.SyntaxKind.VariableDeclaration ->
        let variableDeclaration = declaration :?> Ts.VariableDeclaration
        let name = unbox<Ts.Node> variableDeclaration.name

        let isConst =
            let parent: Ts.Node = !!variableDeclaration.parent
            not (isNull parent) && (int parent.flags &&& int Ts.NodeFlags.Const) <> 0

        ({
            Name = identifierText name
            Documentation = reader.ReadDocumentationFromNode name
            Type =
                match variableDeclaration.``type`` with
                | Some typeNode -> reader.ReadTypeNode typeNode
                | None -> GlueType.Primitive GluePrimitive.Any
            IsOptional = false
            IsStatic = false
            Accessor =
                if isConst then
                    GlueAccessor.ReadOnly
                else
                    GlueAccessor.ReadWrite
            IsPrivate = false
        }
        : GlueProperty)
        |> GlueMember.Property

    | _ ->
        Report.readerError (
            "declaration",
            $"Unsupported kind %s{declaration.kind.Name}",
            declaration
        )
        |> failwith
