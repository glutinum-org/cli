module Glutinum.Converter.Transformer.TypeParameter

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Transformer.Utils

type SealedTypeInfo =
    {
        TypeParameterName: string
        FSharpType: FSharpType
    }

type TransformResult =
    {
        FSharpTypeParameter: FSharpTypeParameter
        SealedTypeOpt: SealedTypeInfo option
    }

    static member Create
        (
            typeParameterName: string,
            default_: FSharpType option,
            ?constraint_: FSharpType,
            ?sealedType: FSharpType
        )
        =
        let fsharpTypeParameter =
            FSharpTypeParameterInfo.Create(
                typeParameterName,
                ?default_ = default_,
                ?constraint_ = constraint_
            )
            |> FSharpTypeParameter.FSharpTypeParameter

        let sealedType =
            match sealedType with
            | Some sealedType ->
                {
                    TypeParameterName = typeParameterName
                    FSharpType = sealedType
                }
                |> Some
            | None -> None

        {
            FSharpTypeParameter = fsharpTypeParameter
            SealedTypeOpt = sealedType
        }

/// `withDefault` transforms the default, which exposes the types it names. A signature
/// never prints a default, F# has none, and exposing them there names the type parameters
/// of the enclosing declaration, which the exposed type can't declare.

let rec mapFSharpType (seadledTypes: SealedTypeInfo list) (typ: FSharpType) =
    match typ with
    | FSharpType.TypeParameter typeParameter ->
        let needMapping =
            seadledTypes
            |> List.tryFind (fun mapperInfo -> mapperInfo.TypeParameterName = typeParameter)

        match needMapping with
        | Some mapperInfo -> mapperInfo.FSharpType
        | None -> typ
    // A nested anonymous type is a `Mapped`, a type parameter one named `'T`
    | FSharpType.Mapped info when info.Name.StartsWith "'" ->
        match
            seadledTypes
            |> List.tryFind (fun mapperInfo -> mapperInfo.TypeParameterName = info.Name.Substring 1)
        with
        | Some mapperInfo -> mapperInfo.FSharpType
        | None -> typ
    | FSharpType.Mapped info ->
        { info with
            TypeParameters =
                info.TypeParameters
                |> List.map (
                    function
                    | FSharpTypeParameter.FSharpType typ ->
                        mapFSharpType seadledTypes typ |> FSharpTypeParameter.FSharpType
                    | FSharpTypeParameter.FSharpTypeParameter paramInfo ->
                        match
                            seadledTypes
                            |> List.tryFind (fun mapperInfo ->
                                mapperInfo.TypeParameterName = paramInfo.Name
                            )
                        with
                        | Some mapperInfo -> FSharpTypeParameter.FSharpType mapperInfo.FSharpType
                        | None -> FSharpTypeParameter.FSharpTypeParameter paramInfo
                )
        }
        |> FSharpType.Mapped
    | FSharpType.Option innerType -> FSharpType.Option(mapFSharpType seadledTypes innerType)
    | FSharpType.TypeReference typeReference ->
        { typeReference with
            TypeArguments = typeReference.TypeArguments |> List.map (mapFSharpType seadledTypes)
        }
        |> FSharpType.TypeReference
    | FSharpType.Union unionInfo ->
        { unionInfo with
            Cases =
                unionInfo.Cases
                |> List.map (fun case ->
                    match case with
                    | FSharpUnionCase.Named _ -> case
                    | FSharpUnionCase.Typed typ ->
                        mapFSharpType seadledTypes typ |> FSharpUnionCase.Typed
                    | FSharpUnionCase.Field(name, typ) ->
                        FSharpUnionCase.Field(name, mapFSharpType seadledTypes typ)
                    | FSharpUnionCase.NamedFields(caseInfo, fields) ->
                        FSharpUnionCase.NamedFields(
                            caseInfo,
                            fields
                            |> List.map (fun (name, typ) -> name, mapFSharpType seadledTypes typ)
                        )
                )
        }
        |> FSharpType.Union

    | FSharpType.Tuple types -> types |> List.map (mapFSharpType seadledTypes) |> FSharpType.Tuple

    | FSharpType.ResizeArray typ -> mapFSharpType seadledTypes typ |> FSharpType.ResizeArray

    | FSharpType.Function functionInfo ->
        { functionInfo with
            Parameters = functionInfo.Parameters |> List.map (mapFsharpParameter seadledTypes)
            ReturnType = mapFSharpType seadledTypes functionInfo.ReturnType
        }
        |> FSharpType.Function

    // A delegate is referred to through its alias, with the type parameters it takes
    | FSharpType.TypeAlias alias ->
        { alias with
            TypeParameters =
                alias.TypeParameters
                |> List.map (
                    function
                    | FSharpTypeParameter.FSharpTypeParameter info as typeParameter ->
                        match
                            seadledTypes
                            |> List.tryFind (fun mapperInfo ->
                                mapperInfo.TypeParameterName = info.Name
                            )
                        with
                        | Some mapperInfo -> FSharpTypeParameter.FSharpType mapperInfo.FSharpType
                        | None -> typeParameter
                    | FSharpTypeParameter.FSharpType typ ->
                        FSharpTypeParameter.FSharpType(mapFSharpType seadledTypes typ)
                )
        }
        |> FSharpType.TypeAlias

    | FSharpType.Enum _
    | FSharpType.SingleErasedCaseUnion _
    | FSharpType.Module _
    | FSharpType.Interface _
    | FSharpType.Unsupported _
    | FSharpType.Primitive _
    | FSharpType.Discard
    | FSharpType.ThisType _
    | FSharpType.Class _
    | FSharpType.Object
    | FSharpType.JSApi _
    | FSharpType.Delegate _
    | FSharpType.TypeExtension _ -> typ

// `options?: Options` with `Options extends X | undefined` sealed to `X option` is `?options: X`
and mapFsharpParameter (seadledTypes: SealedTypeInfo list) (parameter: FSharpParameter) =
    let typ = mapFSharpType seadledTypes parameter.Type

    { parameter with
        Type =
            if parameter.IsOptional then
                tryUnwrapOption typ |> Option.defaultValue typ
            else
                typ
    }

type TransformTypeParametersResult =
    {
        TypeParameters: FSharpTypeParameter list
        SealedTypes: SealedTypeInfo list
    }

type TransformDeclarationTypeParametersResult =
    {
        TypeParameters: FSharpTypeParameter list
        SealedTypeArguments: FSharpTypeParameter list option
    }
