module Glutinum.Converter.Reader.TypeQueryNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils
open FsToolkit.ErrorHandling

let private declarationsInProgress = ResizeArray<Ts.Node>()

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
            // `typeof ns.X` resolves to the export alias of `X`, not to its declaration
            let symbol = resolveAlias checker symbol |> Option.defaultValue symbol

            // Try to find the declaration of the type, to get more information about it
            match symbol.declarations with
            | Some declarations when declarations.Count > 0 ->
                let declaration = declarations.[0]

                match declaration.kind with
                | Ts.SyntaxKind.ClassDeclaration ->
                    // `typeof Action` is the constructor, the instance type arguments are unknown
                    let typeArguments =
                        match (declaration :?> Ts.ClassDeclaration).typeParameters with
                        | Some typeParameters ->
                            typeParameters
                            |> Seq.toList
                            |> List.map (fun _ -> GlueType.Primitive GluePrimitive.Any)
                        | None -> []

                    ({
                        Name = declaredName symbol |> Option.defaultValue symbol.name
                        FullName = checker.getFullyQualifiedName symbol
                        ModulePath =
                            modulePathForSymbol checker reader.PackageContext false (Some symbol)
                        TypeArguments = typeArguments
                        IsStandardLibrary = false
                    }
                    : GlueTypeReference)
                    |> GlueType.TypeReference

                // We don't support TypeQuery for ModuleDeclaration yet
                // See https://github.com/glutinum-org/cli/issues/70 for a possible solution
                | Ts.SyntaxKind.ModuleDeclaration -> GlueType.Discard
                // `typeof fn` is the function type, a generic one has no F# delegate at the use site
                | Ts.SyntaxKind.FunctionDeclaration ->
                    match reader.ReadNode declaration with
                    | GlueType.FunctionDeclaration info when not info.TypeParameters.IsEmpty ->
                        GlueType.Primitive GluePrimitive.Any
                    | GlueType.FunctionDeclaration info ->
                        ({
                            Documentation = info.Documentation
                            Type = info.Type
                            TypeParameters = info.TypeParameters
                            OwnTypeParameterNames = []
                            Parameters = info.Parameters
                        }
                        : GlueFunctionType)
                        |> GlueType.FunctionType
                    | glueType -> glueType
                | Ts.SyntaxKind.MethodDeclaration
                | Ts.SyntaxKind.MethodSignature ->
                    // `typeof YAMLMap.prototype.add` leaves the type parameters of `YAMLMap<K, V>` free
                    let inScope =
                        let rec collect (node: Ts.Node) (acc: string list) =
                            if isNull node then
                                acc
                            else
                                let typeParameters: ResizeArray<Ts.TypeParameterDeclaration> option =
                                    node?typeParameters

                                let acc =
                                    match typeParameters with
                                    | Some typeParameters ->
                                        acc
                                        @ (typeParameters
                                           |> Seq.toList
                                           |> List.map (fun typeParameter ->
                                               identifierText typeParameter.name
                                           ))
                                    | None -> acc

                                collect node.parent acc

                        collect (typeQueryNode :> Ts.Node) [] |> set

                    let toFunctionDeclaration
                        (name: string)
                        (documentation: GlueComment list)
                        (parameters: GlueParameter list)
                        (returnType: GlueType)
                        =
                        let free =
                            [
                                yield! GlueSubstitution.mentionedTypeParameters returnType
                                for parameter in parameters do
                                    yield! GlueSubstitution.mentionedTypeParameters parameter.Type
                            ]
                            |> List.filter (fun name -> not (inScope.Contains name))
                            |> List.map (fun name -> name, GlueType.Primitive GluePrimitive.Any)
                            |> Map.ofList

                        ({
                            Documentation = documentation
                            IsDeclared = true
                            Name = name
                            Type = GlueSubstitution.substitute free returnType
                            Parameters =
                                parameters |> List.map (GlueSubstitution.substituteParameter free)
                            TypeParameters = []
                        }
                        : GlueFunctionDeclaration)
                        |> GlueType.FunctionDeclaration

                    match reader.ReadDeclaration declaration with
                    | GlueMember.Method info ->
                        toFunctionDeclaration info.Name info.Documentation info.Parameters info.Type
                    | GlueMember.MethodSignature info ->
                        toFunctionDeclaration info.Name info.Documentation info.Parameters info.Type
                    | _ -> GlueType.Primitive GluePrimitive.Any
                // `typeof Promise` is the `PromiseConstructor` interface of the library, it has no binding
                | _ when isFromEs5Lib (Some symbol) || isFromEsLib (Some symbol) ->
                    GlueType.Primitive GluePrimitive.Any
                | _ ->
                    if declarationsInProgress.Contains declaration then
                        GlueType.Primitive GluePrimitive.Any
                    else
                        declarationsInProgress.Add declaration

                        try
                            reader.ReadNode declaration
                        finally
                            declarationsInProgress.RemoveAt(declarationsInProgress.Count - 1)

            | _ -> GlueType.Primitive GluePrimitive.Any

    | HasTypeFlags Ts.TypeFlags.String -> GlueType.Primitive GluePrimitive.String

    | HasTypeFlags Ts.TypeFlags.Number -> GlueType.Primitive GluePrimitive.Number

    | HasTypeFlags Ts.TypeFlags.Boolean -> GlueType.Primitive GluePrimitive.Bool

    | HasTypeFlags Ts.TypeFlags.Any -> GlueType.Primitive GluePrimitive.Any

    | HasTypeFlags Ts.TypeFlags.Void -> GlueType.Primitive GluePrimitive.Unit

    | _ -> GlueType.Primitive GluePrimitive.Any
