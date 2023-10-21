module rec Glutinum.Converter.Program

open Fable.Core
open System
open Node
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.FSharpAST
open Node.Api
open Fable.Core.JS

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

        FSharpLiteral.String literal.text |> Some
    | Ts.SyntaxKind.TrueKeyword -> FSharpLiteral.Bool true |> Some
    | Ts.SyntaxKind.FalseKeyword -> FSharpLiteral.Bool false |> Some

    | _ ->
        let text = expression.getText ()

        if isNumericString text then
            // First, try to parse as an integer
            match System.Int32.TryParse text with
            | (true, i) -> FSharpLiteral.Int i |> Some
            | _ ->
                // If it fails, try to parse as a float
                match System.Double.TryParse text with
                | (true, f) -> FSharpLiteral.Float f |> Some
                | _ -> None
        else
            None


let private readEnumCases
    (checker: Ts.TypeChecker)
    (state:
        {|
            NextCaseIndex: int
            Cases: FSharpEnumCase list
        |})
    (enumMember: Ts.EnumMember)
    =

    let caseValue =
        match enumMember.initializer with
        | None ->
            match checker.getConstantValue (!^enumMember) with
            | Some(U2.Case1 str) -> FSharpLiteral.String str
            | Some(U2.Case2 num) ->
                if Constructors.Number.isSafeInteger num then
                    FSharpLiteral.Int(int num)
                else
                    FSharpLiteral.Float num
            | None -> FSharpLiteral.Int(state.NextCaseIndex)
        | Some initializer ->
            match tryReadLiteral initializer with
            | Some fsharpLiteral ->
                match fsharpLiteral with
                | FSharpLiteral.String _
                | FSharpLiteral.Int _
                | FSharpLiteral.Float _ as value -> value
                | FSharpLiteral.Bool _ ->
                    failwith "Boolean literals are not supported in enums"

            | None -> failwith "readEnumCases: Unsupported enum initializer"

    let name = unbox<Ts.Identifier> enumMember.name

    let newCase =
        {
            Name = name.getText ()
            Value = caseValue
        }
        : FSharpEnumCase

    {|
        NextCaseIndex =
            match caseValue with
            // Use the current case index as a reference for the next case
            // In TypeScript, you can have the following enum:
            // enum E { A, B = 4, C }
            // Meaning that C is 5
            | FSharpLiteral.Int i -> i + 1
            // TODO: Mixed enums is not supported in F#, should we fail, ignore
            // or generate a comment in the generated code?
            | _ -> state.NextCaseIndex + 1
        Cases = state.Cases @ [ newCase ]
    |}

let private readEnum
    (checker: Ts.TypeChecker)
    (enumDeclaration: Ts.EnumDeclaration)
    : FSharpEnum =
    let initialState = {| NextCaseIndex = 0; Cases = [] |}

    let readEnumResults =
        enumDeclaration.members
        |> List.ofSeq
        |> List.fold (readEnumCases checker) initialState

    {
        Name = enumDeclaration.name.getText ()
        Cases = readEnumResults.Cases
    }

let private createSimpleType (name: string) : FSharpType =
    { Name = name; Declarations = [] } |> FSharpType.Mapped

let private readTypeNode
    (checker: Ts.TypeChecker)
    (typeNode: option<Ts.TypeNode>)
    : FSharpType =
    match typeNode with
    | Some typeNode ->
        match typeNode.kind with
        | Ts.SyntaxKind.NumberKeyword -> createSimpleType "float"
        | Ts.SyntaxKind.StringKeyword -> createSimpleType "string"
        | Ts.SyntaxKind.VoidKeyword -> createSimpleType "unit"
        | Ts.SyntaxKind.UnionType ->
            readUnionType checker "fake" (typeNode :?> Ts.UnionTypeNode)
            |> FSharpType.Enum
        | _ -> failwith $"readTypeNode: Unsupported kind {typeNode.kind}"
    | None -> createSimpleType "unit"

let rec private readUnionTypeCases
    (checker: Ts.TypeChecker)
    (unionTypeNode: Ts.UnionTypeNode)
    : FSharpEnumCase list =
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
                ({
                    Name = literalTypeNode.getText ()
                    Value =
                        tryReadLiteral literalExpression
                        |> Option.defaultWith (fun () ->
                            failwith "Expected a NumericLiteral"
                        )
                }
                : FSharpEnumCase)
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
                        readUnionType checker "fake" declaration?``type``

                    enum.Cases
                )
                |> Some
        else
            match node.kind with
            | Ts.SyntaxKind.UnionType ->
                let unionTypeNode = node :?> Ts.UnionTypeNode
                // Unwrap union
                readUnionTypeCases checker unionTypeNode |> Some
            | _ ->
                let typ = readTypeNode checker (Some (node :?> Ts.TypeNode))
                printfn $"readUnionTypeCases: Unsupported type {typ}"
                // Capture simple types like string, number, real type, etc.
                None
    )
    |> List.concat

let private readUnionType
    (checker: Ts.TypeChecker)
    (name: string)
    (unionTypeNode: Ts.UnionTypeNode)
    : FSharpEnum =

    let cases = readUnionTypeCases checker unionTypeNode

    { Name = name; Cases = cases }: FSharpEnum

let readTypeOperator
    (checker: Ts.TypeChecker)
    (name: string)
    (node: Ts.TypeOperatorNode)
    : FSharpType =

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

                let fsharpInterface =
                    readInterfaceDeclaration checker interfaceDeclaration

                // Transform the F# interface into a F# enum
                let cases =
                    fsharpInterface.Members
                    |> List.map (fun memb ->
                        {
                            Name = memb.Name
                            Value = FSharpUnionCaseType.Named memb.Name
                        }
                        : FSharpUnionCase
                    )

                { Name = name; Cases = cases } |> FSharpType.Union

        else
            failwith "readTypeOperator: Unsupported type reference"

    | _ -> failwith $"readTypeOperator: Unsupported operator {node.operator}"

let private readTypeAliasDeclaration
    (checker: Ts.TypeChecker)
    (declaration: Ts.TypeAliasDeclaration)
    =

    match declaration.``type``.kind with
    | Ts.SyntaxKind.UnionType ->
        let unionTypeNode = declaration.``type`` :?> Ts.UnionTypeNode
        // Should it be moved inside of readUnionType?
        let unionName = declaration.name.getText ()

        readUnionType checker unionName unionTypeNode |> FSharpType.Enum

    | Ts.SyntaxKind.TypeOperator ->
        let typeOperatorNode = declaration.``type`` :?> Ts.TypeOperatorNode
        let aliasName = declaration.name.getText ()
        readTypeOperator checker aliasName typeOperatorNode

    | Ts.SyntaxKind.IndexedAccessType -> FSharpType.Discard

    | _ ->
        failwith
            $"ReadTypeAliasDeclaration: Unsupported kind {declaration.``type``.kind}"

let readInterfaceDeclaration
    (checker: Ts.TypeChecker)
    (declaration: Ts.InterfaceDeclaration)
    : FSharpInterface =

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
                        | true -> FSharpAccessor.ReadOnly
                        | false -> FSharpAccessor.ReadWrite
                | None -> FSharpAccessor.ReadWrite

            {
                Attributes = []
                Name = name.getText ()
                Parameters = []
                Type = readTypeNode checker propertySignature.``type``
                IsOptional = false
                IsStatic = false
                Accessor = Some accessor
                Accessibility = FSharpAccessiblity.Protected
            }
            : FSharpMember

        | Ts.SyntaxKind.CallSignature ->
            let callSignature = declaration :?> Ts.CallSignatureDeclaration

            // let readParameters (parameters : Ts.ParameterDeclaration list) =

            let parameters =
                callSignature.parameters
                |> Seq.toList
                |> List.map (fun parameter ->
                    let name = unbox<Ts.Identifier> parameter.name

                    {
                        Name = name.getText ()
                        IsOptional = false
                        Type = readTypeNode checker parameter.``type``
                    }
                )

            {
                Attributes = [ FSharpAttribute.EmitSelfInvoke ]
                Name = "Invoke"
                Parameters = parameters
                Type = readTypeNode checker callSignature.``type``
                IsOptional = false
                IsStatic = false
                Accessor = None
                Accessibility = FSharpAccessiblity.Protected
            }
            : FSharpMember

        | _ -> failwith "tryReadNamedDeclaration: Unsupported kind"


    let members =
        declaration.members
        |> Seq.toList
        |> List.map (tryReadNamedDeclaration checker)

    {
        Attributes = [ FSharpAttribute.AllowNullLiteral ]
        Name = declaration.name.getText ()
        Members = members
    }

let readVariableStatement
    (checker: Ts.TypeChecker)
    (statement: Ts.VariableStatement)
    =

    let i = ()

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
        statement.declarationList.declarations
        |> Seq.toList
        |> List.map (fun declaration ->
            let i = 0

            let name =
                match declaration.name?kind with
                | Ts.SyntaxKind.Identifier ->
                    let id: Ts.Identifier = !!declaration.name
                    id.getText ()
                | _ -> failwith "readVariableStatement: Unsupported kind"

            {
                Attributes = [ FSharpAttribute.Import(name, "module") ]
                Name = name
                Parameters = []
                Type = readTypeNode checker declaration.``type``
                IsOptional = false
                IsStatic = true
                Accessor = None
                Accessibility = FSharpAccessiblity.Public
            }
        )
    else
        []

let private readNode (checker: Ts.TypeChecker) (typeNode: Ts.Node) =
    match typeNode.kind with
    | Ts.SyntaxKind.EnumDeclaration ->
        let declaration = typeNode :?> Ts.EnumDeclaration

        let enum = readEnum checker declaration

        FSharpType.Enum enum

    | Ts.SyntaxKind.TypeAliasDeclaration ->
        let declaration = typeNode :?> Ts.TypeAliasDeclaration

        readTypeAliasDeclaration checker declaration

    | Ts.SyntaxKind.InterfaceDeclaration ->
        let declaration = typeNode :?> Ts.InterfaceDeclaration

        readInterfaceDeclaration checker declaration |> FSharpType.Interface

    | Ts.SyntaxKind.VariableStatement ->
        {
            Attributes = [ FSharpAttribute.Erase ]
            Name = "Exports"
            Members =
                readVariableStatement
                    checker
                    (typeNode :?> Ts.VariableStatement)
        }
        |> FSharpType.Interface

    | unsupported -> FSharpType.Unsupported unsupported

let private convert
    (checker: Ts.TypeChecker)
    (sourceFile: option<Ts.SourceFile>)
    =
    sourceFile.Value.statements |> List.ofSeq |> List.map (readNode checker)


let transform (filePath: string) =
    if fs.existsSync (U2.Case1 filePath) |> not then
        failwith $"File does not exist: {filePath}"

    let files = [ filePath ]

    let options =
        jsOptions<Ts.CompilerOptions> (fun o ->
            o.target <- Some Ts.ScriptTarget.ES2015
            o.``module`` <- Some Ts.ModuleKind.CommonJS
        )

    let program = ts.createProgram (ResizeArray files, options)

    let checker = program.getTypeChecker ()

    let sourceFile = program.getSourceFile filePath

    let printer = new Printer.Printer()

    let res = convert checker sourceFile

    let outFile = {
        Name = None
        Opens = [ "Fable.Core"; "System" ]
    }

    Printer.printOutFile printer outFile

    Printer.print printer res

    printer.ToString()

// print res

// log(printer.ToString())

// let res = transform "./tests/specs/enums/literalStringEnumWithInheritance.d.ts"
let res = transform "tests/specs/mappedType/indexedAccessType.d.ts"
// let res = transform "./tests/specs/enums/literalNumericEnum.d.ts"
// let res = transform "./tests/specs/enums/literalStringEnum.d.ts"

printfn "'%A'" res
