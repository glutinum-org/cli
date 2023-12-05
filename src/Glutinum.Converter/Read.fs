module rec Glutinum.Converter.Read

open Fable.Core
open System
open Node
open TypeScript
open Fable.Core.JsInterop
open Node.Api
open Fable.Core.JS
open Glutinum.Converter.GlueAST

// TODO: Port to Fable.Core.JS
type JS.NumberConstructor with

    [<Emit("$0.isSafeInteger($1)")>]
    member __.isSafeInteger(_: obj) : bool = jsNative

let private isNumericString (text: string) =
    jsTypeof text = "string" && unbox text |> Constructors.Number.isNaN |> not

let (|HasTypeFlags|_|) (flag: Ts.TypeFlags) (flags: Ts.TypeFlags) =
    if int flags &&& int flag <> 0 then
        Some()
    else
        None

let private tryReadNumericLiteral (text: string) =
    if isNumericString text then
        // First, try to parse as an integer
        match System.Int32.TryParse text with
        | (true, i) -> GlueLiteral.Int i |> Some
        | _ ->
            // If it fails, try to parse as a float
            match System.Double.TryParse text with
            | (true, f) -> GlueLiteral.Float f |> Some
            | _ -> None
    else
        None

let private tryReadLiteral (expression: Ts.Node) =
    match expression.kind with
    | Ts.SyntaxKind.StringLiteral ->
        let literal = (expression :?> Ts.StringLiteral)

        GlueLiteral.String literal.text |> Some
    | Ts.SyntaxKind.TrueKeyword -> GlueLiteral.Bool true |> Some
    | Ts.SyntaxKind.FalseKeyword -> GlueLiteral.Bool false |> Some

    | _ ->
        let text = expression.getText ()

        tryReadNumericLiteral text


let private readEnumMembers
    (checker: Ts.TypeChecker)
    (state:
        {|
            NextCaseIndex: int
            Members: GlueEnumMember list
        |})
    (enumMember: Ts.EnumMember)
    =

    let caseValue =
        match enumMember.initializer with
        | None ->
            match checker.getConstantValue (!^enumMember) with
            | Some(U2.Case1 str) -> GlueLiteral.String str
            | Some(U2.Case2 num) ->
                if Constructors.Number.isSafeInteger num then
                    GlueLiteral.Int(int num)
                else
                    GlueLiteral.Float num
            | None -> GlueLiteral.Int(state.NextCaseIndex)
        | Some initializer ->
            match tryReadLiteral initializer with
            | Some glueLiteral ->
                match glueLiteral with
                | GlueLiteral.String _
                | GlueLiteral.Int _
                | GlueLiteral.Float _ as value -> value
                | GlueLiteral.Bool _ ->
                    failwith "Boolean literals are not supported in enums"

            | None -> failwith "readEnumCases: Unsupported enum initializer"

    let name = unbox<Ts.Identifier> enumMember.name

    let newCase =
        {
            Name = name.getText ()
            Value = caseValue
        }
        : GlueEnumMember

    {|
        NextCaseIndex =
            match caseValue with
            // Use the current case index as a reference for the next case
            // In TypeScript, you can have the following enum:
            // enum E { A, B = 4, C }
            // Meaning that C is 5
            | GlueLiteral.Int i -> i + 1
            // TODO: Mixed enums is not supported in F#, should we fail, ignore
            // or generate a comment in the generated code?
            | _ -> state.NextCaseIndex + 1
        Members = state.Members @ [ newCase ]
    |}

let private readTypeParameters
    (checker: Ts.TypeChecker)
    (typeParameters: ResizeArray<Ts.TypeParameterDeclaration> option) : GlueTypeParameter list =
    match typeParameters with
    | None -> []
    | Some typeParameters ->
        typeParameters
        |> Seq.toList
        |> List.map (fun typeParameter ->
            {
                Name = typeParameter.name.getText ()
                Constraint = None
                Default = None
            }
        )


let private readEnum
    (checker: Ts.TypeChecker)
    (enumDeclaration: Ts.EnumDeclaration)
    : GlueEnum
    =
    let initialState =
        {|
            NextCaseIndex = 0
            Members = []
        |}

    let readEnumResults =
        enumDeclaration.members
        |> List.ofSeq
        |> List.fold (readEnumMembers checker) initialState

    {
        Name = enumDeclaration.name.getText ()
        Members = readEnumResults.Members
    }

let private readTypeNode
    (checker: Ts.TypeChecker)
    (typeNode: option<Ts.TypeNode>)
    : GlueType
    =
    match typeNode with
    | Some typeNode ->
        match typeNode.kind with
        | Ts.SyntaxKind.NumberKeyword -> GlueType.Primitive GluePrimitive.Number
        | Ts.SyntaxKind.StringKeyword -> GlueType.Primitive GluePrimitive.String
        | Ts.SyntaxKind.VoidKeyword -> GlueType.Primitive GluePrimitive.Unit
        | Ts.SyntaxKind.BooleanKeyword -> GlueType.Primitive GluePrimitive.Bool
        | Ts.SyntaxKind.AnyKeyword -> GlueType.Primitive GluePrimitive.Any
        | Ts.SyntaxKind.NullKeyword -> GlueType.Primitive GluePrimitive.Null
        | Ts.SyntaxKind.UndefinedKeyword ->
            GlueType.Primitive GluePrimitive.Undefined
        | Ts.SyntaxKind.UnionType ->
            readUnionType checker (typeNode :?> Ts.UnionTypeNode)

        | Ts.SyntaxKind.TypeReference ->
            let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode

            let symbolOpt =
                checker.getSymbolAtLocation !!typeReferenceNode.typeName

            let fullName =
                match symbolOpt with
                | None -> failwith "readTypeNode: Missing symbol"
                | Some symbol -> checker.getFullyQualifiedName symbol

            // Could this detect false positive, if the library defined
            // its own Exclude type?
            if fullName = "Exclude" then
                let typ =
                    checker.getTypeFromTypeNode typeReferenceNode
                    :?> Ts.UnionOrIntersectionType

                let cases =
                    typ.types
                    |> Seq.toList
                    |> List.choose (fun typ ->
                        match typ.flags with
                        | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                            let literalType = typ :?> Ts.LiteralType

                            let value = unbox<string> literalType.value

                            GlueLiteral.String value
                            |> GlueType.Literal
                            |> Some
                        | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                            let literalType = typ :?> Ts.LiteralType

                            let value =
                                if
                                    Constructors.Number.isSafeInteger
                                        literalType.value
                                then
                                    GlueLiteral.Int(
                                        unbox<int> literalType.value
                                    )
                                else
                                    GlueLiteral.Float(
                                        unbox<float> literalType.value
                                    )

                            value |> GlueType.Literal |> Some
                        | _ -> None
                    )

                cases |> GlueTypeUnion |> GlueType.Union

            else if fullName = "Partial" then
                let typ = checker.getTypeFromTypeNode typeReferenceNode

                // Try find the original type
                // For now, I am navigating inside of the symbol information
                // to find a reference to the interface declaration via one of
                // the members of the type
                // Is there a better way of doing it?
                match typ.aliasTypeArguments with
                | None -> GlueType.Discard
                | Some aliasTypeArguments ->
                    if aliasTypeArguments.Count <> 1 then
                        GlueType.Discard
                    else
                        let symbol = aliasTypeArguments.[0].symbol

                        if symbol.members.IsNone then
                            GlueType.Discard
                        else

                            // Take any of the members
                            let (_, refMember) =
                                symbol.members.Value.entries () |> Seq.head

                            let originalType =
                                refMember.declarations.Value[0].parent

                            match originalType.kind with
                            | Ts.SyntaxKind.InterfaceDeclaration ->
                                let interfaceDeclaration =
                                    originalType :?> Ts.InterfaceDeclaration

                                let members =
                                    interfaceDeclaration.members
                                    |> Seq.toList
                                    |> List.map (
                                        tryReadNamedDeclaration checker
                                    )

                                ({
                                    Name = interfaceDeclaration.name.getText ()
                                    Members = members
                                    TypeParameters = []
                                }
                                : GlueInterface)
                                |> GlueType.Partial

                            | _ -> GlueType.Discard

            else
                ({
                    Name = typeReferenceNode.getText ()
                    FullName = fullName
                })
                |> GlueType.TypeReference

        | Ts.SyntaxKind.ArrayType ->
            let arrayTypeNode = typeNode :?> Ts.ArrayTypeNode

            let elementType =
                readTypeNode checker (Some arrayTypeNode.elementType)

            GlueType.Array elementType

        | Ts.SyntaxKind.TypePredicate -> GlueType.Primitive GluePrimitive.Bool

        | Ts.SyntaxKind.FunctionType ->
            let functionTypeNode = typeNode :?> Ts.FunctionTypeNode

            {
                Type = readTypeNode checker (Some functionTypeNode.``type``)
                Parameters = readParameters checker functionTypeNode.parameters
            }
            |> GlueType.FunctionType

        | Ts.SyntaxKind.TypeQuery ->
            let typeNodeQuery = typeNode :?> Ts.TypeQueryNode

            let typ = checker.getTypeAtLocation typeNodeQuery

            match typ.flags with
            | HasTypeFlags Ts.TypeFlags.Object ->
                {
                    Name = typ.symbol.name
                    Constructors = []
                    Members = []
                    TypeParameters = []
                }
                |> GlueType.ClassDeclaration
            | _ -> GlueType.Primitive GluePrimitive.Any

        | _ -> failwith $"readTypeNode: Unsupported kind {typeNode.kind}"
    | None -> GlueType.Primitive GluePrimitive.Unit

let rec private readUnionTypeCases
    (checker: Ts.TypeChecker)
    (unionTypeNode: Ts.UnionTypeNode)
    : GlueTypeUnion
    =
    // If all the types are literal, generate a Fable enum
    // If the types are TypeReference, of the same literal type, inline the case in a Fable enum
    // If the type are TypeReference, of different literal types, generate an erased Fable union type
    // If otherwise, not supported?

    let rec removeParenthesizedType (node: Ts.Node) =
        if ts.isParenthesizedTypeNode node then
            let parenthesizedTypeNode = node :?> Ts.ParenthesizedTypeNode

            let i = 0
            removeParenthesizedType parenthesizedTypeNode.``type``
        else
            node

    unionTypeNode.types
    |> Seq.toList
    // Remove the ParenthesizedType
    |> List.map removeParenthesizedType
    |> List.choose (fun node ->
        if ts.isLiteralTypeNode node then
            let literalTypeNode = node :?> Ts.LiteralTypeNode

            let literalExpression =
                unbox<Ts.LiteralExpression> literalTypeNode.literal

            if
                ts.isStringLiteral literalExpression
                || ts.isNumericLiteral literalExpression
            then
                tryReadLiteral literalExpression
                |> Option.defaultWith (fun () ->
                    failwith "Expected a NumericLiteral"
                )
                |> GlueType.Literal
                |> fun case -> [ case ]
                |> Some
            else
                match literalExpression.kind with
                | Ts.SyntaxKind.NullKeyword
                | Ts.SyntaxKind.UndefinedKeyword ->
                    GlueType.Primitive GluePrimitive.Null
                    |> List.singleton
                    |> Some
                | _ -> None
        else if ts.isTypeReferenceNode node then
            let typeReferenceNode = node :?> Ts.TypeReferenceNode

            // let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode

            let symbolOpt =
                checker.getSymbolAtLocation !!typeReferenceNode.typeName

            let symbol =
                Option.defaultWith
                    (fun () ->
                        failwith
                            "readUnionTypeCases: Unsupported type reference, missing symbol"
                    )
                    symbolOpt

            // TODO: How to differentiate TypeReference to Enum/Union vs others
            // Check below is really hacky / not robust
            match symbol.declarations with
            | Some declarations ->
                if declarations.Count = 0 then
                    None // Should it be obj ?
                else if declarations.Count > 1 then
                    let fullName = checker.getFullyQualifiedName symbol

                    ({
                        Name = typeReferenceNode.getText ()
                        FullName = fullName
                    })
                    |> GlueType.TypeReference
                    |> List.singleton
                    |> Some
                else
                    let declaration = declarations.[0]

                    readNode checker declaration |> List.singleton |> Some

            | None ->
                let typ = checker.getTypeOfSymbol symbol

                match typ.flags with
                | HasTypeFlags Ts.TypeFlags.Any ->
                    GlueType.Primitive GluePrimitive.Any
                    |> List.singleton
                    |> Some
                | _ ->
                    failwith "readUnionTypeCases: Unsupported type reference"

        // else
        //     symbol.declarations
        //     |> Seq.toList
        //     |> List.collect (fun declaration ->
        //         // We use the readUnionType to handle nested unions
        //         let enum = readUnionType checker declaration?``type``

        //         [ enum ]
        //     )
        //     |> Some
        else
            match node.kind with
            | Ts.SyntaxKind.UnionType ->
                let unionTypeNode = node :?> Ts.UnionTypeNode
                // Unwrap union
                let (GlueTypeUnion cases) =
                    readUnionTypeCases checker unionTypeNode

                Some cases
            | _ ->
                // Capture simple types like string, number, real type, etc.
                readTypeNode checker (Some(node :?> Ts.TypeNode))
                |> List.singleton
                |> Some
    )
    |> List.concat
    |> GlueTypeUnion

let private readUnionType
    (checker: Ts.TypeChecker)
    (unionTypeNode: Ts.UnionTypeNode)
    : GlueType
    =
    readUnionTypeCases checker unionTypeNode |> GlueType.Union

let readTypeOperator
    (checker: Ts.TypeChecker)
    (node: Ts.TypeOperatorNode)
    : GlueType
    =

    match node.operator with
    | Ts.SyntaxKind.KeyOfKeyword ->
        if ts.isTypeReferenceNode node.``type`` then
            let typeReferenceNode = node.``type`` :?> Ts.TypeReferenceNode

            // TODO: Remove unboxing
            let symbolOpt =
                checker.getSymbolAtLocation !!typeReferenceNode.typeName

            match symbolOpt with
            | None -> failwith "readTypeOperator: Missing symbol"

            | Some symbol ->
                match symbol.declarations with
                | Some declarations ->
                    let interfaceDeclaration =
                        declarations[0] :?> Ts.InterfaceDeclaration

                    readInterfaceDeclaration checker interfaceDeclaration
                    |> GlueType.Interface
                    |> GlueType.KeyOf

                | None -> failwith "readTypeOperator: Missing declaration"

        else
            failwith "readTypeOperator: Unsupported type reference"

    | _ -> failwith $"readTypeOperator: Unsupported operator {node.operator}"

let private readIndexedAccessType
    (checker: Ts.TypeChecker)
    (declaration: Ts.IndexedAccessType)
    =

    let nodeType = declaration.indexType :?> Ts.TypeNode

    let typ =
        match nodeType.kind with
        | Ts.SyntaxKind.TypeOperator ->
            let typeOperatorNode = declaration.indexType :?> Ts.TypeOperatorNode
            readTypeOperator checker typeOperatorNode

        | _ ->
            // readTypeNode checker declaration.symbol
            // |>
            GlueType.Discard

    GlueType.IndexedAccessType typ

let private readTypeAliasDeclaration
    (checker: Ts.TypeChecker)
    (declaration: Ts.TypeAliasDeclaration)
    : GlueType
    =

    let typ =
        match declaration.``type``.kind with
        | Ts.SyntaxKind.UnionType ->
            let unionTypeNode = declaration.``type`` :?> Ts.UnionTypeNode

            readUnionType checker unionTypeNode

        | Ts.SyntaxKind.TypeOperator ->
            let typeOperatorNode = declaration.``type`` :?> Ts.TypeOperatorNode
            readTypeOperator checker typeOperatorNode

        | Ts.SyntaxKind.IndexedAccessType ->
            let declaration = declaration.``type`` :?> Ts.IndexedAccessType
            readIndexedAccessType checker declaration

        | _ -> readTypeNode checker (Some declaration.``type``)

    {
        Name = declaration.name.getText ()
        Type = typ
        TypeParameters = readTypeParameters checker declaration.typeParameters
    }
    |> GlueType.TypeAliasDeclaration

let private readParameters
    (checker: Ts.TypeChecker)
    (parameters: ResizeArray<Ts.ParameterDeclaration>)
    =
    parameters
    |> Seq.toList
    |> List.map (fun parameter ->
        let name = unbox<Ts.Identifier> parameter.name

        {
            Name = name.getText ()
            IsOptional = parameter.questionToken.IsSome
            Type = readTypeNode checker parameter.``type``
        }
    )

let private tryReadNamedDeclaration
    (checker: Ts.TypeChecker)
    (declaration: Ts.NamedDeclaration)
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

        {
            Name = name.getText ()
            Type = readTypeNode checker propertySignature.``type``
            IsStatic = false
            Accessor = accessor
        }
        |> GlueMember.Property

    | Ts.SyntaxKind.CallSignature ->
        let callSignature = declaration :?> Ts.CallSignatureDeclaration

        ({
            Parameters = readParameters checker callSignature.parameters
            Type = readTypeNode checker callSignature.``type``
        }
        : GlueCallSignature)
        |> GlueMember.CallSignature

    | Ts.SyntaxKind.MethodDeclaration ->
        let methodDeclaration = declaration :?> Ts.MethodDeclaration
        let name = unbox<Ts.Identifier> methodDeclaration.name

        {
            Name = name.getText ()
            Parameters = readParameters checker methodDeclaration.parameters
            Type = readTypeNode checker methodDeclaration.``type``
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
            Parameters = readParameters checker indexSignature.parameters
            Type = readTypeNode checker (Some indexSignature.``type``)
        }
        : GlueIndexSignature)
        |> GlueMember.IndexSignature

    | _ ->
        failwith $"tryReadNamedDeclaration: Unsupported kind {declaration.kind}"

let private readInterfaceDeclaration
    (checker: Ts.TypeChecker)
    (declaration: Ts.InterfaceDeclaration)
    : GlueInterface
    =

    let members =
        declaration.members
        |> Seq.toList
        |> List.map (tryReadNamedDeclaration checker)

    {
        Name = declaration.name.getText ()
        Members = members
        TypeParameters = []
    }

let private tryReadVariableStatement
    (checker: Ts.TypeChecker)
    (statement: Ts.VariableStatement)
    : GlueVariable option
    =

    let isExported =
        statement.modifiers
        |> Option.map (fun modifiers ->
            modifiers
            |> Seq.exists (fun modifier ->
                modifier?kind = Ts.SyntaxKind.ExportKeyword
            )
        )
        |> Option.defaultValue false

    if isExported then
        let declaration =
            statement.declarationList.declarations |> Seq.toList |> List.head

        let name =
            match declaration.name?kind with
            | Ts.SyntaxKind.Identifier ->
                let id: Ts.Identifier = !!declaration.name
                id.getText ()
            | _ -> failwith "readVariableStatement: Unsupported kind"

        ({
            Name = name
            Type = readTypeNode checker declaration.``type``
        }
        : GlueVariable)
        |> Some
    else
        None

let private readFunctionDeclaration
    (checker: Ts.TypeChecker)
    (declaration: Ts.FunctionDeclaration)
    : GlueFunctionDeclaration
    =

    let isDeclared =
        match declaration.modifiers with
        | Some modifiers ->
            modifiers
            |> Seq.exists (fun modifier ->
                modifier?kind = Ts.SyntaxKind.DeclareKeyword
            )
        | None -> false

    let name =
        match declaration.name with
        | Some name -> name.getText ()
        | None -> failwith "readFunctionDeclaration: Missing name"

    {
        IsDeclared = isDeclared
        Name = name
        Type = readTypeNode checker declaration.``type``
        Parameters = readParameters checker declaration.parameters
    }


// let private readFunctionTye
//     (checker: Ts.TypeChecker)
//     (declaration: Ts.FunctionTypeNode)
//     : GlueFunctionType
//     =

//     let name =
//         match declaration.name with
//         | Some name -> name.getText ()
//         | None -> failwith "readFunctionDeclaration: Missing name"

//     {
//         IsDeclared = isDeclared
//         Name = name
//         Type = readTypeNode checker declaration.``type``
//         Parameters = readParameters checker declaration.parameters
//     }

let private readModuleDeclaration
    (checker: Ts.TypeChecker)
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
                |> List.map (readNode checker)
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

let private readClassDeclaration
    (checker: Ts.TypeChecker)
    (declaration: Ts.ClassDeclaration)
    : GlueClassDeclaration
    =

    let name = unbox<Ts.Identifier> declaration.name

    let members = declaration.members |> Seq.toList

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

                readParameters checker constructor.parameters
                |> GlueConstructor
                |> Some

            | _ -> None
        )

    let members =
        members |> Seq.toList |> List.map (tryReadNamedDeclaration checker)

    {
        Name = name.getText ()
        Constructors = constructors
        Members = members
        TypeParameters = []
    }

let private readNode (checker: Ts.TypeChecker) (typeNode: Ts.Node) : GlueType =
    match typeNode.kind with
    | Ts.SyntaxKind.EnumDeclaration ->
        let declaration = typeNode :?> Ts.EnumDeclaration

        let enum = readEnum checker declaration

        GlueType.Enum enum

    | Ts.SyntaxKind.TypeAliasDeclaration ->
        let declaration = typeNode :?> Ts.TypeAliasDeclaration

        readTypeAliasDeclaration checker declaration

    | Ts.SyntaxKind.InterfaceDeclaration ->
        let declaration = typeNode :?> Ts.InterfaceDeclaration

        readInterfaceDeclaration checker declaration |> GlueType.Interface

    | Ts.SyntaxKind.VariableStatement ->
        match
            tryReadVariableStatement checker (typeNode :?> Ts.VariableStatement)
        with
        | Some variable -> GlueType.Variable variable
        | None -> GlueType.Discard

    | Ts.SyntaxKind.FunctionDeclaration ->
        let declaration = typeNode :?> Ts.FunctionDeclaration

        readFunctionDeclaration checker declaration
        |> GlueType.FunctionDeclaration

    | Ts.SyntaxKind.ModuleDeclaration ->
        let declaration = typeNode :?> Ts.ModuleDeclaration

        readModuleDeclaration checker declaration |> GlueType.ModuleDeclaration

    | Ts.SyntaxKind.ClassDeclaration ->
        let declaration = typeNode :?> Ts.ClassDeclaration

        readClassDeclaration checker declaration |> GlueType.ClassDeclaration

    | Ts.SyntaxKind.FunctionType ->
        let functionType = typeNode :?> Ts.FunctionTypeNode

        // ({
        //     Parameters = readParameters checker functionType.parameters
        //     Type = readTypeNode checker functionType.``type``
        // } : GlueCallSignature)
        // |> GlueType.FuncTionType
        GlueType.Discard


    // | Ts.SyntaxKind.ExportAssignment ->
    //     let exportAssignment = typeNode :?> Ts.ExportAssignment

    //     let i = 0

    //     // let symbolOpt =
    //     //     checker.get

    //     GlueType.Discard

    | unsupported ->
        printfn $"readNode: Unsupported kind {unsupported}"
        GlueType.Discard

let readSourceFile
    (checker: Ts.TypeChecker)
    (sourceFile: option<Ts.SourceFile>)
    =
    sourceFile.Value.statements |> List.ofSeq |> List.map (readNode checker)
