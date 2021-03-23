module FSharp

[<RequireQualifiedAccess>]
type FSharpEnumCaseType =
    | String
    | Numeric

type FSharpEnumCase =
    {
        Name : string
        Type : FSharpEnumCaseType
        Value : string
    }

type FSharpEnum =
    {
        Name : string
        Cases : FSharpEnumCase list
    }

type FSharpUnionCase =
    {
        Name : string
        Value : string
    }

type FSharpUnion =
    {
        Name : string
        Cases : FSharpUnionCase list
    }

[<RequireQualifiedAccess>]
type FSharpType =
    | Enum of FSharpEnum
    | Union of FSharpUnion
    | Discard
