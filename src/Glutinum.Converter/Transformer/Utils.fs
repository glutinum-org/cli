module Glutinum.Converter.Transformer.Utils

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST

let typedArrayNames =
    set
        [
            "Int8Array"
            "Uint8Array"
            "Uint8ClampedArray"
            "Int16Array"
            "Uint16Array"
            "Int32Array"
            "Uint32Array"
            "Float32Array"
            "Float64Array"
            "BigInt64Array"
            "BigUint64Array"
        ]

/// The iterators of the standard library are iterable, `Iterable<T>` of their first argument
let iteratorNames =
    set
        [
            "IterableIterator"
            "IteratorObject"
            "ArrayIterator"
            "MapIterator"
            "SetIterator"
            "StringIterator"
        ]

let tryUnwrapOption (typ: FSharpType) =
    match typ with
    | FSharpType.Option underlyingType -> Some underlyingType
    | FSharpType.Union unionInfo when unionInfo.IsOptional ->
        FSharpType.Union { unionInfo with IsOptional = false } |> Some
    | _ -> None

/// A `[<ParamObject>]` class only makes sense for a plain data object: a callable, constructable
/// or indexed type literal is generated as an interface
let isDataObject (members: GlueMember list) =
    members
    |> List.forall (
        function
        | GlueMember.IndexSignature _
        | GlueMember.CallSignature _
        | GlueMember.ConstructSignature _ -> false
        | GlueMember.MethodSignature _
        | GlueMember.Property _
        | GlueMember.GetAccessor _
        | GlueMember.SetAccessor _
        | GlueMember.Method _ -> true
    )

let transformLiteral (glueLiteral: GlueLiteral) : FSharpLiteral =
    match glueLiteral with
    | GlueLiteral.String value -> FSharpLiteral.String value
    | GlueLiteral.Int value -> FSharpLiteral.Int value
    | GlueLiteral.Float value -> FSharpLiteral.Float value
    | GlueLiteral.Bool value -> FSharpLiteral.Bool value
    | GlueLiteral.Null -> FSharpLiteral.Null

let transformPrimitive (gluePrimitive: GluePrimitive) : FSharpPrimitive =
    match gluePrimitive with
    | GluePrimitive.String -> FSharpPrimitive.String
    | GluePrimitive.Int -> FSharpPrimitive.Int
    | GluePrimitive.Float -> FSharpPrimitive.Float
    | GluePrimitive.Bool -> FSharpPrimitive.Bool
    | GluePrimitive.Unit -> FSharpPrimitive.Unit
    | GluePrimitive.Number -> FSharpPrimitive.Number
    | GluePrimitive.Any -> FSharpPrimitive.Null
    | GluePrimitive.Null -> FSharpPrimitive.Null
    | GluePrimitive.Undefined -> FSharpPrimitive.Null
    | GluePrimitive.Object -> FSharpPrimitive.Null
    | GluePrimitive.Symbol -> FSharpPrimitive.Null
    | GluePrimitive.BigInt -> FSharpPrimitive.BigInt
    | GluePrimitive.Never -> FSharpPrimitive.Null

// F# wants the optional parameters last, `(n?: number, ...targets: T[])` can't keep `n` optional
let requiredBeforeParamArray (parameters: FSharpParameter list) : FSharpParameter list =
    let isParamArray (parameter: FSharpParameter) =
        parameter.Attributes |> List.contains FSharpAttribute.ParamArray

    if parameters |> List.exists isParamArray then
        parameters
        |> List.map (fun parameter ->
            if isParamArray parameter then
                parameter
            else
                { parameter with IsOptional = false }
        )
    else
        parameters

[<Literal>]
let MAX_GENERATED_CONSTRUCTORS = 12

/// `type Key = string | Key[]`: an F# abbreviation can't refer to itself
let rec replaceSelfReference (name: string) (glueType: GlueType) : GlueType =
    let replace = replaceSelfReference name

    match glueType with
    | GlueType.TypeReference typeReference when
        typeReference.Name = name && not typeReference.IsStandardLibrary
        ->
        GlueType.Primitive GluePrimitive.Any
    | GlueType.TypeReference typeReference ->
        GlueType.TypeReference
            { typeReference with
                TypeArguments = typeReference.TypeArguments |> List.map replace
            }
    | GlueType.Union(GlueTypeUnion cases) ->
        GlueType.Union(GlueTypeUnion(cases |> List.map replace))
    | GlueType.Array elementType -> GlueType.Array(replace elementType)
    | GlueType.ReadOnly innerType -> GlueType.ReadOnly(replace innerType)
    | GlueType.OptionalType innerType -> GlueType.OptionalType(replace innerType)
    | GlueType.TupleType elements -> GlueType.TupleType(elements |> List.map replace)
    | _ -> glueType

module Interface =

    let makePartial (name: string) (originalInterface: FSharpInterface) =
        { originalInterface with
            Name = name
            Members =
                originalInterface.Members
                |> List.map (fun m ->
                    match m with
                    | FSharpMember.Property property ->
                        match tryUnwrapOption property.Type with
                        | Some _ -> m
                        | None -> { property with IsOptional = true } |> FSharpMember.Property
                    | _ -> m
                )
        }

/// `type Payload = void`, a property of that type can't have a setter either
let isUnitAlias (typeMemory: GlueType list) (fullName: string) =
    typeMemory
    |> List.exists (
        function
        | GlueType.TypeAliasDeclaration {
                                            FullName = aliasFullName
                                            Type = GlueType.Primitive GluePrimitive.Unit
                                        } -> aliasFullName = fullName
        | _ -> false
    )

// `Node.Exports.os.hostname ()`: the `Exports` of the files of a package gathered in one module,
// as type abbreviations so that `open Node.Exports` gives `os.hostname ()`
let aggregatedExports (types: FSharpType list) : FSharpType list =
    let isStatic (fsharpMember: FSharpMember) =
        match fsharpMember with
        | FSharpMember.Method info
        | FSharpMember.Property info -> info.IsStatic
        | FSharpMember.StaticMember _ -> true

    // The entry file exporting the package itself, the globals of the package are not exports
    let hasEntryExports =
        types
        |> List.exists (
            function
            | FSharpType.Interface { Name = "Exports"; Members = members } ->
                let isGlobal (attributes: FSharpAttribute list) =
                    attributes
                    |> List.exists (
                        function
                        | FSharpAttribute.Global _ -> true
                        | _ -> false
                    )

                members
                |> List.exists (
                    function
                    | FSharpMember.Method info
                    | FSharpMember.Property info -> not (isGlobal info.Attributes)
                    | FSharpMember.StaticMember info -> not (isGlobal info.Attributes)
                )
            | _ -> false
        )

    // `fs.promises` is a module nested in `fs`, the abbreviations follow the same nesting so
    // `open Node.Exports` gives `fs.promises.access ()`
    let rec abbreviationsOf (path: string) (types: FSharpType list) : FSharpType list =
        types
        |> List.collect (
            function
            | FSharpType.Module fileModule ->
                let own =
                    fileModule.Types
                    |> List.tryPick (
                        function
                        | FSharpType.Interface { Name = "Exports"; Members = members } when
                            not members.IsEmpty && members |> List.forall isStatic
                            ->
                            ({
                                Attributes = []
                                XmlDoc = []
                                Name = fileModule.Name
                                Type =
                                    ({
                                        Name = $"{path}{fileModule.Name}.Exports"
                                        TypeParameters = []
                                    }
                                    : FSharpMapped)
                                    |> FSharpType.Mapped
                                TypeParameters = []
                            }
                            : FSharpTypeAlias)
                            |> FSharpType.TypeAlias
                            |> Some
                        | _ -> None
                    )

                let nested = abbreviationsOf $"{path}{fileModule.Name}." fileModule.Types

                [
                    yield! Option.toList own

                    if not nested.IsEmpty then
                        ({
                            Name = fileModule.Name
                            IsRecursive = false
                            ImportSpecifier = None
                            Types = nested
                        }
                        : FSharpModule)
                        |> FSharpType.Module
                ]
            | _ -> []
        )

    let abbreviations = abbreviationsOf "" types

    if abbreviations.IsEmpty || hasEntryExports then
        []
    else
        [
            ({
                Name = "Exports"
                IsRecursive = false
                ImportSpecifier = None
                Types = abbreviations
            }
            : FSharpModule)
            |> FSharpType.Module
        ]
