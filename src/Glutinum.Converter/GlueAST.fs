/// <summary>
/// Glue AST aims to reflect the TypeScript AST as closely as possible
/// while being easy to manipulate for converting to F# AST and "optimising"
/// portion of the code.
///
/// Optimising in this context means for example converting a method taking
/// literal union into F# overloaded methods. This makes the F# API feels more
/// native and easier to use.
///
/// Sementics in the Glue AST follows the TypeScript sementics.
/// </summary>
module rec Glutinum.Converter.GlueAST

type GlueParameter =
    {
        Name: string
        IsOptional: bool
        Type: GlueType
    }

type GlueMethod =
    {
        Name: string
        Parameters: GlueParameter list
        Type: GlueType
        IsOptional: bool
        IsStatic: bool
    }

type GlueCallSignature =
    {
        Parameters: GlueParameter list
        Type: GlueType
    }

[<RequireQualifiedAccess>]
type GlueAccessor =
    | ReadOnly
    | WriteOnly
    | ReadWrite

type GlueProperty =
    {
        Name: string
        Type: GlueType
        IsStatic: bool
        Accessor: GlueAccessor
    }

[<RequireQualifiedAccess>]
type GlueMember =
    | Method of GlueMethod
    | Property of GlueProperty
    | CallSignature of GlueCallSignature

type GlueInterface =
    {
        Name: string
        Members: GlueMember list
    }

type GlueVariable = { Name: string; Type: GlueType }

[<RequireQualifiedAccess>]
type GluePrimitive =
    | String
    | Int
    | Float
    | Bool
    | Unit
    | Number
    | Any
    | Null
    | Undefined

[<RequireQualifiedAccess>]
type GlueLiteral =
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

type GlueEnumMember =
    {
        Name : string
        Value : GlueLiteral
    }

type GlueEnum =
    {
        Name : string
        Members : GlueEnumMember list
    }

type GlueTypeAliasDeclaration =
    {
        Name : string
        Types : GlueType list
    }

type GlueFunctionDeclaration =
    {
        IsDeclared : bool
        Name : string
        Type : GlueType
        Parameters : GlueParameter list
    }

type GlueTypeModuleDeclaration =
    {
        Name : string
        IsNamespace : bool
        IsRecursive : bool
        Types : GlueType list
    }

type GlueConstructor =
    GlueConstructor of GlueParameter list

type GlueTypeClassDeclaration =
    {
        Name : string
        Constructors : GlueConstructor list
    }

type GlueTypeTypeReference =
    {
        Name : string
        FullName : string
    }

[<RequireQualifiedAccess>]
type GlueType =
    | Discard
    | Interface of GlueInterface
    | Variable of GlueVariable
    | Primitive of GluePrimitive
    | Enum of GlueEnum
    | TypeAliasDeclaration of GlueTypeAliasDeclaration
    | FunctionDeclaration of GlueFunctionDeclaration
    | Union of GlueType list
    | Literal of GlueLiteral
    | KeyOf of GlueType
    | IndexedAccessType of GlueType
    | ModuleDeclaration of GlueTypeModuleDeclaration
    | ClassDeclaration of GlueTypeClassDeclaration
    | TypeReference of GlueTypeTypeReference

    member this.Name with get () =
        match this with
        | Interface info -> info.Name
        | Variable info -> info.Name
        | Primitive info ->
            match info with
            | GluePrimitive.String -> "string"
            | GluePrimitive.Int -> "int"
            | GluePrimitive.Float -> "float"
            | GluePrimitive.Bool -> "bool"
            | GluePrimitive.Unit -> "unit"
            | GluePrimitive.Number -> "float"
            | GluePrimitive.Any -> "obj"
            | GluePrimitive.Null -> "obj option"
            | GluePrimitive.Undefined -> "obj"
        | Enum info -> info.Name
        | TypeAliasDeclaration info -> info.Name
        | Union _ -> "obj"
        | Literal info -> info.ToText()
        | KeyOf _ -> "string"
        | Discard -> "obj"
        | IndexedAccessType _ -> "obj"
        | FunctionDeclaration info -> info.Name
        | ModuleDeclaration info -> info.Name
        | ClassDeclaration info -> info.Name
        | TypeReference info -> info.Name
