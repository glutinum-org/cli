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

let private tryReadLiteral (expression: Ts.Node) =
    match expression.kind with
    | Ts.SyntaxKind.StringLiteral ->
        let literal = (expression :?> Ts.StringLiteral)

        GlueLiteral.String literal.text |> Some
    | Ts.SyntaxKind.TrueKeyword -> GlueLiteral.Bool true |> Some
    | Ts.SyntaxKind.FalseKeyword -> GlueLiteral.Bool false |> Some

    | _ ->
        let text = expression.getText ()

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

let private readEnum
    (checker: Ts.TypeChecker)
    (enumDeclaration: Ts.EnumDeclaration)
    : GlueEnum =
    let initialState = {| NextCaseIndex = 0; Members = [] |}

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
    : GlueType =
    match typeNode with
    | Some typeNode ->
        match typeNode.kind with
        | Ts.SyntaxKind.NumberKeyword -> GlueType.Primitive GluePrimitive.Number
        | Ts.SyntaxKind.StringKeyword -> GlueType.Primitive GluePrimitive.String
        | Ts.SyntaxKind.VoidKeyword -> GlueType.Primitive GluePrimitive.Unit
        | Ts.SyntaxKind.UnionType ->
            readUnionType checker (typeNode :?> Ts.UnionTypeNode)

        | _ -> failwith $"readTypeNode: Unsupported kind {typeNode.kind}"
    | None -> GlueType.Primitive GluePrimitive.Unit

let rec private readUnionTypeCases
    (checker: Ts.TypeChecker)
    (unionTypeNode: Ts.UnionTypeNode)
    : GlueType list =
    // If all the types are literal, generate a Fable enum
    // If the types are TypeReference, of the same literal type, inline the case in a Fable enum
    // If the type are TypeReference, of different literal types, generate an erased Fable union type
    // If otherwise, not supported?

    let rec removeParenthesizedType (node: Ts.Node) =
        if ts.isParenthesizedTypeNode node then
            let parenthesizedTypeNode = node :?> Ts.ParenthesizedTypeNode

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
                None
        else if ts.isTypeReferenceNode node then
            let typeReferenceNode = node :?> Ts.TypeReferenceNode

            // TODO: Remove unboxing
            let symbolOpt =
                checker.getSymbolAtLocation !!typeReferenceNode.typeName

            match symbolOpt with
            | None ->
                failwith "readUnionTypeCases: Unsupported type reference"
                None

            | Some symbol ->
                symbol.declarations
                |> Seq.toList
                |> List.collect (fun declaration ->
                    // We use the readUnionType to handle nested unions
                    let enum =
                        readUnionType checker declaration?``type``

                    [ enum ]
                )
                |> Some
        else
            match node.kind with
            | Ts.SyntaxKind.UnionType ->
                let unionTypeNode = node :?> Ts.UnionTypeNode
                // Unwrap union
                readUnionTypeCases checker unionTypeNode |> Some
            | _ ->
                // Capture simple types like string, number, real type, etc.
                readTypeNode checker (Some (node :?> Ts.TypeNode))
                |> List.singleton
                |> Some
    )
    |> List.concat

let private readUnionType
    (checker: Ts.TypeChecker)
    (unionTypeNode: Ts.UnionTypeNode)
    : GlueType =

    readUnionTypeCases checker unionTypeNode
    |> GlueType.Union

let readTypeOperator
    (checker: Ts.TypeChecker)
    (node: Ts.TypeOperatorNode)
    : GlueType =

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
                let interfaceDeclaration =
                    symbol.declarations[0] :?> Ts.InterfaceDeclaration

                readInterfaceDeclaration checker interfaceDeclaration
                |> GlueType.Interface
                |> GlueType.KeyOf

        else
            failwith "readTypeOperator: Unsupported type reference"

    | _ -> failwith $"readTypeOperator: Unsupported operator {node.operator}"

let private readIndexedAccessType
    (checker : Ts.TypeChecker)
    (declaration : Ts.IndexedAccessType) =

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

    let types =
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

        | _ ->
            failwith
                $"ReadTypeAliasDeclaration: Unsupported kind {declaration.``type``.kind}"

    {
        Name = declaration.name.getText ()
        Types = [ types ]
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
            IsOptional = false
            Type = readTypeNode checker parameter.``type``
        }
    )

let private readInterfaceDeclaration
    (checker: Ts.TypeChecker)
    (declaration: Ts.InterfaceDeclaration)
    : GlueInterface =

    let tryReadNamedDeclaration
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

            {
                Parameters = readParameters checker callSignature.parameters
                Type = readTypeNode checker callSignature.``type``
            }
            |> GlueMember.CallSignature

        | _ -> failwith "tryReadNamedDeclaration: Unsupported kind"

    let members =
        declaration.members
        |> Seq.toList
        |> List.map (tryReadNamedDeclaration checker)

    {
        Name = declaration.name.getText ()
        Members = members
    }

let private tryReadVariableStatement
    (checker: Ts.TypeChecker)
    (statement: Ts.VariableStatement)
    : GlueVariable option =

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

        {
            Name = name
            Type = readTypeNode checker declaration.``type``
        }
        |> Some
    else
        None

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

    | unsupported -> GlueType.Discard

let readSourceFile
    (checker: Ts.TypeChecker)
    (sourceFile: option<Ts.SourceFile>)
    =
    sourceFile.Value.statements |> List.ofSeq |> List.map (readNode checker)
