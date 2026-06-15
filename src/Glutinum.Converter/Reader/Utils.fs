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

/// <summary>
/// Determine if a numeric literal source text denotes a float (i.e. has a
/// decimal point or a decimal exponent), as opposed to an integer.
///
/// Radix literals (<c>0x</c>, <c>0b</c>, <c>0o</c>) are always integers, so we
/// don't mistake the <c>e</c> in <c>0xE</c> for an exponent.
/// </summary>
let private isWrittenAsFloat (text: string) =
    let lower = text.ToLower()

    if lower.StartsWith "0x" || lower.StartsWith "0b" || lower.StartsWith "0o" then
        false
    else
        text.Contains "." || lower.Contains "e"

let tryReadLiteral (checker: Ts.TypeChecker) (expression: Ts.Node) =
    match expression.kind with
    | Ts.SyntaxKind.StringLiteral ->
        let literal = (expression :?> Ts.StringLiteral)

        GlueLiteral.String literal.text |> Some
    | Ts.SyntaxKind.TrueKeyword -> GlueLiteral.Bool true |> Some
    | Ts.SyntaxKind.FalseKeyword -> GlueLiteral.Bool false |> Some
    | Ts.SyntaxKind.NullKeyword -> GlueLiteral.Null |> Some
    | _ ->
        let text = expression.getText ()

        // Source the numeric value from the type checker (robust against
        // numeric separators, hex/binary/octal literals, large values, ...)
        // but decide int vs float from the source text so that `10.0` and
        // `1e3` are kept as floats even though TypeScript normalises them to
        // the integer-valued literal type `10` / `1000`.
        let resolvedType = checker.getTypeAtLocation expression

        match resolvedType.flags with
        | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
            let value = unbox<float> (resolvedType :?> Ts.LiteralType).value

            if Constructors.Number.isSafeInteger value && not (isWrittenAsFloat text) then
                GlueLiteral.Int(unbox<int> value) |> Some
            else
                GlueLiteral.Float value |> Some
        | _ ->
            // Fallback to parsing the source text directly
            tryReadNumericLiteral text

let tryGetFullName (checker: Ts.TypeChecker) (node: Ts.Node) =
    // Naive way to check if the node has a symbol
    // The others solutions is to redo a pattern matching on the node.type
    // but it's more complex (can be changed if this solution is too permissive)
    if isNull node?symbol then
        match checker.getSymbolAtLocation node with
        | None -> None
        | Some symbol -> checker.getFullyQualifiedName symbol |> Some
    else
        checker.getFullyQualifiedName node?symbol |> Some

let getFullNameOrEmpty (checker: Ts.TypeChecker) (node: Ts.Node) =
    tryGetFullName checker node |> Option.defaultValue ""

type ModifierUtil =

    static member GetAccessor(modifiers: ResizeArray<Ts.Modifier> option) =
        match modifiers with
        | Some modifiers ->
            modifiers
            |> Seq.exists (fun modifier -> modifier.kind = Ts.SyntaxKind.ReadonlyKeyword)
            |> function
                | true -> GlueAccessor.ReadOnly
                | false -> GlueAccessor.ReadWrite
        | None -> GlueAccessor.ReadWrite

    static member GetAccessor(modifiers: ResizeArray<Ts.ModifierLike> option) =
        ModifierUtil.GetAccessor(unbox<ResizeArray<Ts.Modifier> option> modifiers)

    static member HasModifier(modifiers: ResizeArray<Ts.Modifier> option, modifier: Ts.SyntaxKind) =
        match modifiers with
        | Some modifiers ->
            modifiers |> Seq.exists (fun currentModifier -> currentModifier.kind = modifier)
        | None -> false

    static member HasModifier
        (modifiers: option<ResizeArray<Ts.ModifierLike>>, modifier: Ts.SyntaxKind)
        =
        ModifierUtil.HasModifier(unbox<ResizeArray<Ts.Modifier> option> modifiers, modifier)

let readTypeArguments (reader: ITypeScriptReader) (node: Ts.NodeWithTypeArguments) =
    match node.typeArguments with
    | None -> []
    | Some typeArguments -> typeArguments |> Seq.toList |> List.map (Some >> reader.ReadTypeNode)

let readHeritageClauses
    (reader: ITypeScriptReader)
    (heritageClauses: ResizeArray<Ts.HeritageClause> option)
    =
    match heritageClauses with
    | Some heritageClauses ->
        heritageClauses
        |> Seq.toList
        |> List.collect (fun clause -> clause.types |> Seq.toList |> List.map reader.ReadTypeNode)
    | None -> []

/// <summary>
/// Determine if the type is from the ES5 library
///
/// This is to detect native utility types usage
/// </summary>
/// <param name="symbolOpt"></param>
/// <returns>
/// <c>True</c> if the type is from the ES5 library otherwise <c>False</c>
/// </returns>
let isFromEs5Lib (symbolOpt: Ts.Symbol option) =
    match symbolOpt with
    | None -> false
    | Some symbol ->
        match symbol.declarations with
        | None ->
            // For some reason, I can't seem to resolve the actual symbol for some Es5 types
            // So, we make a naive fallback checking the name of the symbol
            [ "Iterable"; "IterableIterator" ] |> List.contains symbol.name
        | Some declarations ->
            match declarations[0].parent.kind with
            | Ts.SyntaxKind.SourceFile ->
                let sourceFile = declarations[0].parent :?> Ts.SourceFile

                sourceFile.fileName.EndsWith("lib/lib.es5.d.ts")
            | _ -> false

module Type =

    module StringLiteral =

        let (|String|Other|) (literalType: Ts.Type) =
            match literalType.flags with
            | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                let literalType = literalType :?> Ts.LiteralType

                String(unbox<string> literalType.value)
            | _ -> Other

    module NumberLiteral =

        let (|Int|Float|Other|) (literalType: Ts.Type) =
            match literalType.flags with
            | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                let literalType = literalType :?> Ts.LiteralType

                if Constructors.Number.isSafeInteger literalType.value then
                    Int(unbox<int> literalType.value)
                else
                    Float(unbox<float> literalType.value)
            | _ -> Other
