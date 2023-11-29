module rec Glutinum.Converter.FSharpAST

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
        Attributes : FSharpAttribute list
        Name : string
    }

[<RequireQualifiedAccess>]
type FSharpUnionType =
    | String
    | Numeric
    | Unknown

type FSharpUnion =
    {
        Attributes : FSharpAttribute list
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
        IsRecursive : bool
        Types : FSharpType list
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
    | ImportAll of string
    | Erase
    | AllowNullLiteral
    | StringEnum
    | CompiledName of string
    | RequireQualifiedAccess
    | EmitConstructor
    | EmitMacroConstructor of className : string

type FSharpParameter =
    {
        Name: string
        IsOptional: bool
        Type: FSharpType
    }

type FSharpMemberInfo =
    {
        Attributes : FSharpAttribute list
        Name : string
        Parameters : FSharpParameter list
        Type : FSharpType
        IsOptional : bool
        IsStatic : bool
        Accessor : FSharpAccessor option
        Accessibility : FSharpAccessiblity
    }

[<RequireQualifiedAccess>]
type FSharpMember =
    | Method of FSharpMemberInfo
    | Property of FSharpMemberInfo

type FSharpInterface =
    {
        Attributes : FSharpAttribute list
        Name : string
        Members : FSharpMember list
    }

type FSharpMapped =
    {
        Name : string
        Declarations : FSharpType list
    }

[<RequireQualifiedAccess>]
type FSharpPrimitive =
    | String
    | Int
    | Float
    | Bool
    | Unit
    | Number
    | Null

type FSharpTypeAlias =
    {
        Name : string
        Type : FSharpType
    }

// type FSharpClass =
//     {
//         Name : string
//         Members : FSharpMember list
//     }

[<RequireQualifiedAccess>]
type FSharpType =
    | Enum of FSharpEnum
    | Union of FSharpUnion
    // Create ErasedUnion type to make a difference between standard F# union
    // and Fable U2, U3, etc. types.
    // It is also possible that a third type of union exist which are
    // specialized ErasedUnion types.
    // To avoid using U2, U3, etc. types, but insteand things like:
    // [<Erased>]
    // type ConfigType =
    //     | String of string
    //     | Numeric of float
    // Allowing for a more natural syntax, as you know what each cases expects
    // compared to U2, U3, etc. for which you have to look at the type definition.
    // | ErasedUnion of FSharpUnion
    | Module of FSharpModule
    | Interface of FSharpInterface
    | Unsupported of Ts.SyntaxKind
    | Mapped of FSharpMapped
    | Primitive of FSharpPrimitive
    | Alias of FSharpTypeAlias
    // | Class of FSharpClass
    | Discard

type FSharpOutFile =
    {
        Name : string
        Opens : string list
    }
