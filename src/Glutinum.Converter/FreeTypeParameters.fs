/// A type parameter used where nothing binds it cannot compile, `obj` can.
/// A type only sees the parameters it declares and the ones of the member holding it: a
/// nested module resets the scope, F# modules bind no type parameter.
module Glutinum.Converter.FreeTypeParameters

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST

let private declaredNames (typeParameters: FSharpTypeParameter list) =
    typeParameters
    |> List.choose (
        function
        | FSharpTypeParameter.FSharpTypeParameter info -> Some info.Name
        | FSharpTypeParameter.FSharpType _ -> None
    )
    |> Set.ofList

let rec private eraseType (bound: Set<string>) (typ: FSharpType) : FSharpType =
    let erase = eraseType bound

    let eraseTypeParameters (typeParameters: FSharpTypeParameter list) =
        typeParameters
        |> List.map (
            function
            | FSharpTypeParameter.FSharpType typ -> FSharpTypeParameter.FSharpType(erase typ)
            | FSharpTypeParameter.FSharpTypeParameter info as typeParameter ->
                if bound.Contains info.Name then
                    typeParameter
                else
                    FSharpTypeParameter.FSharpType FSharpType.Object
        )

    match typ with
    | FSharpType.TypeParameter name when not (bound.Contains name) -> FSharpType.Object
    // A type parameter also reaches the printer as a `Mapped` named `'T`
    | FSharpType.Mapped info when
        info.Name.StartsWith "'" && not (bound.Contains(info.Name.Substring 1))
        ->
        FSharpType.Object
    | FSharpType.Mapped info ->
        { info with
            TypeParameters = eraseTypeParameters info.TypeParameters
        }
        |> FSharpType.Mapped
    | FSharpType.TypeReference typeReference ->
        { typeReference with
            TypeArguments = typeReference.TypeArguments |> List.map erase
        }
        |> FSharpType.TypeReference
    | FSharpType.ThisType thisType ->
        { thisType with
            TypeParameters = eraseTypeParameters thisType.TypeParameters
        }
        |> FSharpType.ThisType
    | FSharpType.Option typ -> FSharpType.Option(erase typ)
    | FSharpType.ResizeArray typ -> FSharpType.ResizeArray(erase typ)
    | FSharpType.JSApi(FSharpJSApi.ReadonlyArray typ) ->
        FSharpType.JSApi(FSharpJSApi.ReadonlyArray(erase typ))
    | FSharpType.Tuple types -> FSharpType.Tuple(types |> List.map erase)
    | FSharpType.Union unionInfo ->
        let bound = Set.union bound (declaredNames unionInfo.TypeParameters)
        let erase = eraseType bound

        { unionInfo with
            Cases =
                unionInfo.Cases
                |> List.map (
                    function
                    | FSharpUnionCase.Typed typ -> FSharpUnionCase.Typed(erase typ)
                    | FSharpUnionCase.Field(name, typ) -> FSharpUnionCase.Field(name, erase typ)
                    | FSharpUnionCase.NamedFields(caseInfo, fields) ->
                        FSharpUnionCase.NamedFields(
                            caseInfo,
                            fields |> List.map (fun (name, typ) -> name, erase typ)
                        )
                    | case -> case
                )
        }
        |> FSharpType.Union
    | FSharpType.Function functionType ->
        { functionType with
            Parameters =
                functionType.Parameters
                |> List.map (fun parameter ->
                    { parameter with
                        Type = erase parameter.Type
                    }
                )
            ReturnType = erase functionType.ReturnType
        }
        |> FSharpType.Function
    | typ -> typ

let private eraseParameters (bound: Set<string>) (parameters: FSharpParameter list) =
    parameters
    |> List.map (fun parameter ->
        { parameter with
            Type = eraseType bound parameter.Type
        }
    )

// F# generalises an undeclared type parameter of a method that declares none. A property
// cannot be generic, and next to explicit parameters nothing is generalised
let private eraseMember (bound: Set<string>) (fsharpMember: FSharpMember) =
    let erase isProperty (typeParameters: FSharpTypeParameter list) parameters typ =
        if typeParameters.IsEmpty && not isProperty then
            parameters, typ
        else
            let bound = Set.union bound (declaredNames typeParameters)
            eraseParameters bound parameters, eraseType bound typ

    match fsharpMember with
    | FSharpMember.Method info ->
        let parameters, typ =
            erase info.Accessor.IsSome info.TypeParameters info.Parameters info.Type

        FSharpMember.Method
            { info with
                Parameters = parameters
                Type = typ
            }
    | FSharpMember.Property info ->
        let parameters, typ = erase true info.TypeParameters info.Parameters info.Type

        FSharpMember.Property
            { info with
                Parameters = parameters
                Type = typ
            }
    | FSharpMember.StaticMember info ->
        let parameters, typ =
            erase info.Accessor.IsSome info.TypeParameters info.Parameters info.Type

        FSharpMember.StaticMember
            { info with
                Parameters = parameters
                Type = typ
            }

let rec apply (types: FSharpType list) : FSharpType list =
    types
    |> List.map (
        function
        | FSharpType.Interface interfaceInfo ->
            let bound = declaredNames interfaceInfo.TypeParameters

            FSharpType.Interface
                { interfaceInfo with
                    Members = interfaceInfo.Members |> List.map (eraseMember bound)
                }
        // A delegate, a union and an abbreviation generalise nothing
        | FSharpType.Delegate delegateInfo ->
            let bound = declaredNames delegateInfo.TypeParameters

            FSharpType.Delegate
                { delegateInfo with
                    Parameters = eraseParameters bound delegateInfo.Parameters
                    ReturnType = eraseType bound delegateInfo.ReturnType
                }
        | FSharpType.Union _ as typ -> eraseType Set.empty typ
        | FSharpType.TypeAlias aliasInfo ->
            let bound = declaredNames aliasInfo.TypeParameters

            FSharpType.TypeAlias
                { aliasInfo with
                    Type = eraseType bound aliasInfo.Type
                }
        // A module binds no type parameter, the types inside start from their own
        | FSharpType.Module moduleInfo ->
            FSharpType.Module
                { moduleInfo with
                    Types = apply moduleInfo.Types
                }
        | typ -> typ
    )
