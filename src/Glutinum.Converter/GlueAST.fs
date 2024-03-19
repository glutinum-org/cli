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

open Thoth.Json.Core

type GlueParameter =
    {
        Name: string
        IsOptional: bool
        Type: GlueType
    }

    static member Encoder(value: GlueParameter) =
        Encode.object
            [
                "Name", Encode.string value.Name
                "IsOptional", Encode.bool value.IsOptional
                "Type", GlueType.Encoder value.Type
            ]

type GlueTypeParameter =
    {
        Name: string
        Constraint: GlueType option
        Default: GlueType option
    }

    static member Encoder(value: GlueTypeParameter) =
        Encode.object
            [
                "Name", Encode.string value.Name
                "Constraint", Encode.option GlueType.Encoder value.Constraint
                "Default", Encode.option GlueType.Encoder value.Default
            ]

type GlueMethod =
    {
        Name: string
        Parameters: GlueParameter list
        Type: GlueType
        IsOptional: bool
        IsStatic: bool
    }

    static member Encoder(value: GlueMethod) =
        Encode.object
            [
                "Name", Encode.string value.Name
                "Parameters",
                value.Parameters
                |> List.map GlueParameter.Encoder
                |> Encode.list
                "Type", GlueType.Encoder value.Type
                "IsOptional", Encode.bool value.IsOptional
                "IsStatic", Encode.bool value.IsStatic
            ]

type GlueCallSignature =
    {
        Parameters: GlueParameter list
        Type: GlueType
    }

    static member Encoder(value: GlueCallSignature) =
        Encode.object
            [
                "Parameters",
                value.Parameters
                |> List.map GlueParameter.Encoder
                |> Encode.list
                "Type", GlueType.Encoder value.Type
            ]

[<RequireQualifiedAccess>]
type GlueAccessor =
    | ReadOnly
    | WriteOnly
    | ReadWrite

    static member Encoder(value: GlueAccessor) =
        match value with
        | ReadOnly -> Encode.string "ReadOnly"
        | WriteOnly -> Encode.string "WriteOnly"
        | ReadWrite -> Encode.string "ReadWrite"

type GlueProperty =
    {
        Name: string
        Type: GlueType
        IsStatic: bool
        Accessor: GlueAccessor
    }

    static member Encoder(value: GlueProperty) =
        Encode.object
            [
                "Name", Encode.string value.Name
                "Type", GlueType.Encoder value.Type
                "IsStatic", Encode.bool value.IsStatic
                "Accessor", GlueAccessor.Encoder value.Accessor
            ]

type GlueIndexSignature =
    {
        Parameters: GlueParameter list
        Type: GlueType
    }

    static member Encoder(value: GlueIndexSignature) =
        Encode.object
            [
                "Parameters",
                value.Parameters
                |> List.map GlueParameter.Encoder
                |> Encode.list
                "Type", GlueType.Encoder value.Type
            ]

type GlueMethodSignature =
    {
        Name: string
        Parameters: GlueParameter list
        Type: GlueType
    }

    static member Encoder(value: GlueMethodSignature) =
        Encode.object
            [
                "Name", Encode.string value.Name
                "Parameters",
                value.Parameters
                |> List.map GlueParameter.Encoder
                |> Encode.list
                "Type", GlueType.Encoder value.Type
            ]

[<RequireQualifiedAccess>]
type GlueMember =
    | Method of GlueMethod
    | Property of GlueProperty
    | CallSignature of GlueCallSignature
    | IndexSignature of GlueIndexSignature
    | MethodSignature of GlueMethodSignature

    static member Encoder(value: GlueMember) =
        match value with
        | Method info -> Encode.object [ "Method", GlueMethod.Encoder info ]
        | Property info ->
            Encode.object [ "Property", GlueProperty.Encoder info ]
        | CallSignature info ->
            Encode.object [ "CallSignature", GlueCallSignature.Encoder info ]
        | IndexSignature info ->
            Encode.object [ "IndexSignature", GlueIndexSignature.Encoder info ]
        | MethodSignature info ->
            Encode.object
                [ "MethodSignature", GlueMethodSignature.Encoder info ]

type GlueInterface =
    {
        Name: string
        Members: GlueMember list
        TypeParameters: GlueTypeParameter list
    }

    static member Encoder(value: GlueInterface) =
        Encode.object
            [
                "Name", Encode.string value.Name
                "Members",
                value.Members |> List.map GlueMember.Encoder |> Encode.list
                "TypeParameters",
                value.TypeParameters
                |> List.map GlueTypeParameter.Encoder
                |> Encode.list
            ]

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
        | String value -> value
        | Int value -> string value
        | Float value -> string value
        | Bool value -> string value

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
        | IndexedAccessType _
        | Union _
        | Partial _
        | FunctionType _
        | TupleType _
        | Discard -> "obj"

    static member Encoder(value: GlueType) =
        match value with
        | Interface interfaceInfo ->
            Encode.object [ "Interface", GlueInterface.Encoder interfaceInfo ]
        | _ -> Encode.string "not yet implemented"
