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

type GlueCommentParam =
    { Name: string; Content: string option }

[<RequireQualifiedAccess>]
type GlueComment =
    | Summary of string list
    | Returns of string
    | Param of GlueCommentParam
    | Deprecated of string option
    | Remarks of string
    | DefaultValue of string

type GlueParameter =
    {
        Name: string
        IsOptional: bool
        IsSpread: bool
        Type: GlueType
    }

type GlueTypeParameter =
    {
        Name: string
        Constraint: GlueType option
        Default: GlueType option
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
        Documentation: GlueComment list
        Type: GlueType
        IsStatic: bool
        IsOptional: bool
        Accessor: GlueAccessor
        IsPrivate: bool
    }

type GlueIndexSignature =
    {
        Parameters: GlueParameter list
        Type: GlueType
    }

type GlueMethodSignature =
    {
        Name: string
        Parameters: GlueParameter list
        Type: GlueType
    }

type GlueConstructSignature =
    {
        Parameters: GlueParameter list
        Type: GlueType
    }

[<RequireQualifiedAccess>]
type GlueMember =
    | Method of GlueMethod
    | Property of GlueProperty
    | CallSignature of GlueCallSignature
    | IndexSignature of GlueIndexSignature
    | MethodSignature of GlueMethodSignature
    | ConstructSignature of GlueConstructSignature

type GlueInterface =
    {
        Name: string
        Members: GlueMember list
        TypeParameters: GlueTypeParameter list
    }

type GlueTypeLiteral = { Members: GlueMember list }

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
    | Object

[<RequireQualifiedAccess>]
type GlueLiteral =
    | String of string
    | Int of int
    | Float of float
    | Bool of bool
    | Null

    member this.ToText() =
        match this with
        | String value -> value
        | Int value -> string value
        | Float value -> string value
        | Bool value -> string value
        | Null -> "null"

type GlueEnumMember = { Name: string; Value: GlueLiteral }

type GlueEnum =
    {
        Name: string
        Members: GlueEnumMember list
    }

type GlueTypeAliasDeclaration =
    {
        Name: string
        Type: GlueType
        TypeParameters: GlueTypeParameter list
    }

type GlueFunctionDeclaration =
    {
        Documentation: GlueComment list
        IsDeclared: bool
        Name: string
        Type: GlueType
        Parameters: GlueParameter list
        TypeParameters: GlueTypeParameter list
    }

type GlueModuleDeclaration =
    {
        Name: string
        IsNamespace: bool
        IsRecursive: bool
        Types: GlueType list
    }

type GlueConstructor = GlueConstructor of GlueParameter list

type GlueClassDeclaration =
    {
        Name: string
        Constructors: GlueConstructor list
        Members: GlueMember list
        TypeParameters: GlueTypeParameter list
    }

type GlueTypeReference =
    {
        Name: string
        FullName: string
        TypeArguments: GlueType list
    }

type GlueTypeUnion = GlueTypeUnion of GlueType list

[<RequireQualifiedAccess>]
type ExcludedMember = Literal of GlueLiteral
// | Function

type GlueFunctionType =
    {
        Type: GlueType
        Parameters: GlueParameter list
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
    | Union of GlueTypeUnion
    | Literal of GlueLiteral
    | KeyOf of GlueType
    | IndexedAccessType of GlueType
    | ModuleDeclaration of GlueModuleDeclaration
    | ClassDeclaration of GlueClassDeclaration
    | TypeReference of GlueTypeReference
    | Partial of GlueInterface
    | Array of GlueType
    | FunctionType of GlueFunctionType
    | TypeParameter of string
    | ThisType of typeName: string
    | TupleType of GlueType list
    | IntersectionType of GlueMember list
    | TypeLiteral of GlueTypeLiteral
    | OptionalType of GlueType
    | Unknown
    | ExportDefault of GlueType

    member this.Name =
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
            | GluePrimitive.Object -> "obj"
        | Enum info -> info.Name
        | TypeAliasDeclaration info -> info.Name
        | TypeParameter name -> name
        | Literal info -> info.ToText()
        | KeyOf _ -> "string"
        | FunctionDeclaration info -> info.Name
        | ModuleDeclaration info -> info.Name
        | ClassDeclaration info -> info.Name
        | TypeReference info -> info.Name
        | Array info -> $"ResizeArray<{info.Name}>"
        | ThisType typeName -> typeName
        | TypeLiteral _
        | IntersectionType _
        | IndexedAccessType _
        | Union _
        | Partial _
        | FunctionType _
        | TupleType _
        | OptionalType _ // TODO: Should we take the name of the underlying type and add option to it?
        | Discard
        | ExportDefault _
        | Unknown -> "obj"
