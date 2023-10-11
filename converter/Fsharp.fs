module Glutinum.Converter.FSharpAST

open TypeScript

[<RequireQualifiedAccess>]
type FSharpLiteral =
    | String of string
    | Int of int
    | Float of float
    | Bool of bool

    member this.ToText() =
        match this with
        | String value ->
            value
        | Int value ->
            string value
        | Float value ->
            string value
        | Bool value ->
            string value

// [<RequireQualifiedAccess>]
// type FSharpEnumCaseValue =
//     | Int of int
//     | Float of float

type FSharpEnumCase =
    {
        Name : string
        Value : FSharpLiteral
    }

[<RequireQualifiedAccess>]
type FSharpEnumType =
    | String
    | Numeric
    | Unknown // or Mixed?

type FSharpEnum =
    {
        Name : string
        Cases : FSharpEnumCase list
    }

    member this.Type =
        let isString =
            this.Cases
            |> List.forall (fun c ->
                match c.Value with
                | FSharpLiteral.String _ ->
                    true
                | FSharpLiteral.Float _
                | FSharpLiteral.Int _
                | FSharpLiteral.Bool _ ->
                    false
            )

        let isNumeric =
            this.Cases
            |> List.forall (fun c ->
                match c.Value with
                | FSharpLiteral.String _
                | FSharpLiteral.Bool _ ->
                    false
                | FSharpLiteral.Float _
                | FSharpLiteral.Int _ ->
                    true
            )

        if isNumeric then
            FSharpEnumType.Numeric
        elif isString then
            FSharpEnumType.String
        else
            FSharpEnumType.Unknown

[<RequireQualifiedAccess>]
type FSharpUnionCaseType =
    | Named of string
    | Literal of string
    // | Float of float
    // | Int of int

type FSharpUnionCase =
    {
        Name : string
        Value : FSharpUnionCaseType
    }

[<RequireQualifiedAccess>]
type FSharpUnionType =
    | String
    | Numeric
    | Unknown

type FSharpUnion =
    {
        Name : string
        Cases : FSharpUnionCase list
    }

    // member this.Type =
    //     let isString =
    //         this.Cases
    //         |> List.forall (fun c ->
    //             match c.Value with
    //             | FSharpUnionCaseType.Literal _ ->
    //                 true
    //             | FSharpUnionCaseType.Float _
    //             | FSharpUnionCaseType.Int _ ->
    //                 false
    //         )

    //     let isNumeric =
    //         this.Cases
    //         |> List.forall (fun c ->
    //             match c.Value with
    //             | FSharpUnionCaseType.String _ ->
    //                 false
    //             | FSharpUnionCaseType.Float _
    //             | FSharpUnionCaseType.Int _ ->
    //                 true
    //         )

    //     if isNumeric then
    //         FSharpUnionType.Numeric
    //     elif isString then
    //         FSharpUnionType.String
    //     else
    //         FSharpUnionType.Unknown

type FSharpModule =
    {
        Name : string
    }

[<RequireQualifiedAccess>]
type FSharpAccessor =
    | ReadOnly
    | WriteOnly
    | ReadWrite

[<RequireQualifiedAccess>]
type FSharpAccessiblity =
    | Public
    | Private
    | Protected

[<RequireQualifiedAccess>]
type FSharpAttribute =
    | Text of string
    /// <summary>
    /// Generates <c>[&lt;Emit("$0($1...)")&gt;]</c> attribute.
    /// </summary>
    | EmitSelfInvoke
    | Import of string * string
    | Erase
    | AllowNullLiteral

type FSharpParameter =
    {
        Name: string
        IsOptional: bool
        Type: string
    }

type FSharpMember =
    {
        Attributes : FSharpAttribute list
        Name : string
        Parameters : FSharpParameter list
        Type : string
        IsOptional : bool
        IsStatic : bool
        Accessor : FSharpAccessor option
        Accessibility : FSharpAccessiblity
    }

type FSharpInterface =
    {
        Attributes : FSharpAttribute list
        Name : string
        Members : FSharpMember list
    }

[<RequireQualifiedAccess>]
type FSharpType =
    | Enum of FSharpEnum
    | Union of FSharpUnion
    | Module of FSharpModule
    | Interface of FSharpInterface
    | Unsupported of Ts.SyntaxKind
    | Discard

type FSharpOutFile =
    {
        Name : string option
        Opens : string list
    }
