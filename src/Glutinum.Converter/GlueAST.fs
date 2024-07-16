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

type GlueCommentTypeParam =
    {
        TypeName: string
        Content: string option
    }

[<RequireQualifiedAccess>]
type GlueComment =
    | Summary of string list
    | Returns of string
    | Param of GlueCommentParam
    | Deprecated of string option
    | Remarks of string
    | DefaultValue of string
    | Example of string
    | TypeParam of GlueCommentTypeParam
    | Throws of string

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

type GlueSetAccessor =
    {
        Name: string
        Documentation: GlueComment list
        ArgumentType: GlueType
        IsStatic: bool
        IsPrivate: bool
    }

type GlueGetAccessor =
    {
        Name: string
        Documentation: GlueComment list
        Type: GlueType
        IsStatic: bool
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
        Documentation: GlueComment list
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
    | GetAccessor of GlueGetAccessor
    | SetAccessor of GlueSetAccessor
    | CallSignature of GlueCallSignature
    | IndexSignature of GlueIndexSignature
    | MethodSignature of GlueMethodSignature
    | ConstructSignature of GlueConstructSignature

type GlueInterface =
    {
        Name: string
        Members: GlueMember list
        TypeParameters: GlueTypeParameter list
        HeritageClauses: GlueType list
    }

type GlueTypeLiteral = { Members: GlueMember list }

type GlueVariable =
    {
        Documentation: GlueComment list
        Name: string
        Type: GlueType
    }

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
    | Symbol

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
        Documentation: GlueComment list
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

type GlueConstructor =
    {
        Documentation: GlueComment list
        Parameters: GlueParameter list
    }

type GlueClassDeclaration =
    {
        Name: string
        Constructors: GlueConstructor list
        Members: GlueMember list
        TypeParameters: GlueTypeParameter list
        HeritageClauses: GlueType list
    }

type GlueTypeReference =
    {
        Name: string
        FullName: string
        TypeArguments: GlueType list
        // Should we replace that with a real computation of the FullName
        // and use it to determine if it's from standard library?
        // I think right now, we don't have a correct FullName because I didn't know how to get it
        // but a few days ago, I found a way to access the source file information
        // this is how the isStandardLibrary is determined
        // so we can perhaps revisit the FullName reader logic with this new knowledge
        IsStandardLibrary: bool
    }

type GlueTypeUnion = GlueTypeUnion of GlueType list

[<RequireQualifiedAccess>]
type ExcludedMember = Literal of GlueLiteral
// | Function

type GlueFunctionType =
    {
        Documentation: GlueComment list
        Type: GlueType
        Parameters: GlueParameter list
    }

type NamedTupleType = { Name: string; Type: GlueType }

type GlueRecord =
    {
        KeyType: GlueType
        ValueType: GlueType
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
    | Record of GlueRecord
    | Array of GlueType
    | FunctionType of GlueFunctionType
    | TypeParameter of string
    | ThisType of typeName: string
    | TupleType of GlueType list
    | NamedTupleType of NamedTupleType
    | IntersectionType of GlueMember list
    | TypeLiteral of GlueTypeLiteral
    | OptionalType of GlueType
    | Unknown
    | ExportDefault of GlueType
    | TemplateLiteral

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
            | GluePrimitive.Symbol -> "obj"
        | TemplateLiteral -> "string"
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
        | NamedTupleType _
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
        | Record _
        | Unknown -> "obj"
