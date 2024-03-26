module Glutinum.Converter.Reader.Utils

open TypeScript
open Glutinum.Converter.GlueAST
open Fable.Core.JS
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Types

let (|HasTypeFlags|_|) (flag: Ts.TypeFlags) (flags: Ts.TypeFlags) =
    if int flags &&& int flag <> 0 then
        Some()
    else
        None

let (|HasSymbolFlags|_|) (flag: Ts.SymbolFlags) (flags: Ts.SymbolFlags) =
    if int flags &&& int flag <> 0 then
        Some()
    else
        None

let (|HasObjectFlags|_|) (flag: Ts.ObjectFlags) (flags: Ts.ObjectFlags) =
    if int flags &&& int flag <> 0 then
        Some()
    else
        None

let private isNumericString (text: string) =
    jsTypeof text = "string" && unbox text |> Constructors.Number.isNaN |> not

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

let tryReadLiteral (expression: Ts.Node) =
    match expression.kind with
    | Ts.SyntaxKind.StringLiteral ->
        let literal = (expression :?> Ts.StringLiteral)

        GlueLiteral.String literal.text |> Some
    | Ts.SyntaxKind.TrueKeyword -> GlueLiteral.Bool true |> Some
    | Ts.SyntaxKind.FalseKeyword -> GlueLiteral.Bool false |> Some
    | Ts.SyntaxKind.NullKeyword -> GlueLiteral.Null |> Some
    | _ ->
        let text = expression.getText ()

        tryReadNumericLiteral text

let generateReaderError
    (errorContext: string)
    (reason: string)
    (node: Ts.Node)
    =
    let sourceFile = node.getSourceFile ()
    let lineAndChar = sourceFile.getLineAndCharacterOfPosition node.pos
    let line = int lineAndChar.line + 1
    let column = int lineAndChar.character + 1

    $"""Error while reading %s{errorContext} in:
%s{sourceFile.fileName}(%d{line},%d{column})

%s{reason}

--- Text ---
%s{node.getFullText ()}
---

--- Parent text ---
%s{node.parent.getFullText ()}
---"""

let tryGetFullName (checker: Ts.TypeChecker) (node: Ts.Node) =
    match checker.getSymbolAtLocation node with
    | None -> None
    | Some symbol -> checker.getFullyQualifiedName symbol |> Some

let getFullNameOrEmpty (checker: Ts.TypeChecker) (node: Ts.Node) =
    tryGetFullName checker node |> Option.defaultValue ""
