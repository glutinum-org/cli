module Glutinum.Converter.Program

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
                | FSharpLiteral.Float _ as value ->
                    value
                | FSharpLiteral.Bool _ ->
                    failwith "Boolean literals are not supported in enums"

            | None ->
                failwith "readEnumCases: Unsupported enum initializer"

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
            // In TypeScript, you can have the following enum: enum E { A, B = 4, C }
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

let private readUnionType (name : string) (unionTypeNode : Ts.UnionTypeNode) : FSharpEnum =
    let cases =
        unionTypeNode.types
        |> List.ofSeq
        |> List.filter ts.isLiteralTypeNode
        |> List.map (fun node ->
            let literalTypeNode = node :?> Ts.LiteralTypeNode
            let literalExpression = unbox<Ts.LiteralExpression> literalTypeNode.literal

            match literalExpression.kind with
            | Ts.SyntaxKind.StringLiteral ->
                let x = tryReadLiteral literalExpression
                {
                    Name = literalTypeNode.getText ()
                    Value =
                        tryReadLiteral literalExpression
                        |> Option.defaultWith (fun () ->
                            failwith "Expected a StringLiteral"
                        )
                } : FSharpEnumCase
            | Ts.SyntaxKind.NumericLiteral ->
                {
                    Name = literalTypeNode.getText ()
                    Value =
                        tryReadLiteral literalExpression
                        |> Option.defaultWith (fun () ->
                            failwith "Expected a NumericLiteral"
                        )
                } : FSharpEnumCase
            | _ ->
                failwith "Expected a StringLiteral or NumericLiteral"
        )

    {
        Name = name
        Cases = cases
    }

let private readTypeAliasDeclaration (declaration : Ts.TypeAliasDeclaration)  =
    match declaration.``type``.kind with
    | Ts.SyntaxKind.UnionType ->
        let unionTypeNode = declaration.``type`` :?> Ts.UnionTypeNode
        let name = declaration.name.getText ()

        readUnionType name unionTypeNode
    | _ ->
        failwith "ReadTypeAliasDeclaration: Unsupported kind"

let private convert
    (checker: Ts.TypeChecker)
    (sourceFile: option<Ts.SourceFile>)
    =
    sourceFile.Value.statements
    |> List.ofSeq
    |> List.collect (fun statement ->
        match statement.kind with
        | Ts.SyntaxKind.EnumDeclaration ->
            let declaration = (statement :?> Ts.EnumDeclaration)

            let enum = readEnum checker declaration

            [ FSharpType.Enum enum ]

        | Ts.SyntaxKind.TypeAliasDeclaration ->
            let declaration = (statement :?> Ts.TypeAliasDeclaration)

            [ FSharpType.Enum (readTypeAliasDeclaration declaration) ]

        | _ ->
            printfn "Unsupported Statement kind: %A" statement.kind
            []

    )


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

let res = transform "./tests/specs/enums/literalStringEnum.d.ts"

printfn "%A" res
