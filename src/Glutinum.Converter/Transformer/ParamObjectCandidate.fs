module Glutinum.Converter.Transformer.ParamObjectCandidate

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Transformer.Utils
open Glutinum.Converter.Transformer.TypeParameters

/// `http.Cookie` of `"file".http.Cookie` or `http.Cookie`: the namespace path declaring the name
let private containerOf (fullName: string) =
    let unquoted =
        if fullName.StartsWith "\"" then
            match fullName.IndexOf("\".", 1) with
            | -1 -> fullName
            | index -> fullName.Substring(index + 2)
        else
            fullName

    match unquoted.LastIndexOf '.' with
    | -1 -> ""
    | index -> unquoted.Substring(0, index)

let private isEnclosing (container: string) (inner: string) =
    container = "" || container = inner || inner.StartsWith(container + ".")

let tryResolveMembers (typeMemory: GlueType list) (info: GlueInterface) =
    let tryFindInterface (fullName: string) =
        typeMemory
        |> List.tryPick (
            function
            | GlueType.Interface candidate when candidate.FullName = fullName -> Some candidate
            | _ -> None
        )

    let rec resolve (visited: Set<string>) (info: GlueInterface) =
        if Set.contains info.FullName visited then
            None
        else
            let visited = Set.add info.FullName visited

            let fromHeritageClause (heritageClause: GlueType) =
                match heritageClause with
                | GlueType.TypeReference typeReference when
                    typeReference.IsStandardLibrary && typeReference.Name = "Partial"
                    ->
                    match typeReference.TypeArguments with
                    | [ GlueType.TypeReference baseReference ] ->
                        tryFindInterface baseReference.FullName
                        |> Option.bind (resolve visited)
                        |> Option.map (
                            List.map (
                                function
                                | GlueMember.Property property ->
                                    GlueMember.Property { property with IsOptional = true }
                                | glueMember -> glueMember
                            )
                        )
                    | _ -> None

                | GlueType.UtilityType(GlueUtilityType.Omit members)
                | GlueType.UtilityType(GlueUtilityType.Pick members) -> Some members

                | GlueType.TypeReference typeReference ->
                    match tryFindInterface typeReference.FullName with
                    | Some baseInterface ->
                        let substitutions =
                            GlueSubstitution.ofTypeArguments
                                baseInterface.TypeParameters
                                typeReference.TypeArguments

                        // `IRoute<Route>` names itself in its members, the heritage clause knows
                        // the module
                        let rec qualifySelf (glueType: GlueType) =
                            match glueType with
                            | GlueType.ThisType thisType when thisType.Name = baseInterface.Name ->
                                GlueType.TypeReference
                                    {
                                        Name = baseInterface.Name
                                        FullName = baseInterface.FullName
                                        ModulePath = typeReference.ModulePath
                                        TypeArguments =
                                            thisType.TypeParameters
                                            |> List.map (fun typeParameter ->
                                                GlueType.TypeParameter typeParameter.Name
                                                |> GlueSubstitution.substitute substitutions
                                            )
                                        IsStandardLibrary = false
                                    }
                            | GlueType.TypeReference reference when
                                reference.FullName = baseInterface.FullName
                                && reference.ModulePath.IsEmpty
                                ->
                                GlueType.TypeReference
                                    { reference with
                                        ModulePath = typeReference.ModulePath
                                        TypeArguments =
                                            reference.TypeArguments |> List.map qualifySelf
                                    }
                            | GlueType.TypeReference reference ->
                                GlueType.TypeReference
                                    { reference with
                                        TypeArguments =
                                            reference.TypeArguments |> List.map qualifySelf
                                    }
                            | GlueType.Union(GlueTypeUnion cases) ->
                                GlueType.Union(GlueTypeUnion(cases |> List.map qualifySelf))
                            | GlueType.Array elementType -> GlueType.Array(qualifySelf elementType)
                            | GlueType.OptionalType innerType ->
                                GlueType.OptionalType(qualifySelf innerType)
                            | glueType -> glueType

                        // The members of `interface Cookie extends http.Cookie {}` name the types
                        // of `http` unqualified, they were read there
                        let rec namesForeignType (glueType: GlueType) =
                            match glueType with
                            | GlueType.TypeReference reference ->
                                (reference.ModulePath.IsEmpty
                                 && not reference.IsStandardLibrary
                                 && not (
                                     isEnclosing
                                         (containerOf reference.FullName)
                                         (containerOf info.FullName)
                                 ))
                                || reference.TypeArguments |> List.exists namesForeignType
                            | GlueType.Union(GlueTypeUnion cases) ->
                                cases |> List.exists namesForeignType
                            | GlueType.Array innerType
                            | GlueType.ReadOnly innerType
                            | GlueType.OptionalType innerType -> namesForeignType innerType
                            | GlueType.TupleType elements ->
                                elements |> List.exists namesForeignType
                            | GlueType.FunctionType functionType ->
                                namesForeignType functionType.Type
                                || functionType.Parameters
                                   |> List.exists (fun parameter -> namesForeignType parameter.Type)
                            | _ -> false

                        resolve visited baseInterface
                        |> Option.map (
                            List.map (fun glueMember ->
                                match
                                    GlueSubstitution.substituteMember substitutions glueMember
                                with
                                | GlueMember.Property property ->
                                    GlueMember.Property
                                        { property with
                                            Type = qualifySelf property.Type
                                        }
                                | glueMember -> glueMember
                            )
                        )
                        |> Option.filter (
                            List.forall (
                                function
                                | GlueMember.Property property ->
                                    not (namesForeignType property.Type)
                                | _ -> true
                            )
                        )
                    | None -> None

                | _ -> None

            (Some [], info.HeritageClauses)
            ||> List.fold (fun acc heritageClause ->
                match acc, fromHeritageClause heritageClause with
                | Some acc, Some members -> Some(acc @ members)
                | _ -> None
            )
            |> Option.map (fun inheritedMembers ->
                let ownNames =
                    info.Members
                    |> List.choose (
                        function
                        | GlueMember.Property property -> Some property.Name
                        | _ -> None
                    )
                    |> Set.ofList

                let inheritedMembers =
                    inheritedMembers
                    |> List.filter (
                        function
                        | GlueMember.Property property -> not (Set.contains property.Name ownNames)
                        | _ -> true
                    )
                    // A base interface reached through several heritage clauses
                    |> List.distinctBy (
                        function
                        | GlueMember.Property property -> Choice1Of2 property.Name
                        | glueMember -> Choice2Of2 glueMember
                    )

                inheritedMembers @ info.Members
            )

    resolve Set.empty info

// `option<O extends Options>(key, options: O)`: `Options` is the argument through `O`
let isCandidate (typeMemory: GlueType list) (info: GlueInterface) =
    let members = tryResolveMembers typeMemory info

    // `[key: string]: any` beside the properties takes the extra fields, `Create` names the properties
    let takesExtraFields (indexSignature: GlueIndexSignature) =
        not indexSignature.IsReadOnly
        && (indexSignature.Parameters
            |> List.forall (fun parameter ->
                parameter.Type = GlueType.Primitive GluePrimitive.String
            ))

    let hasOnlyProperties =
        match members with
        | Some members ->
            members
            |> List.exists (
                function
                | GlueMember.Property _ -> true
                | _ -> false
            )
            && members
               |> List.forall (
                   function
                   | GlueMember.Property property -> not (isUnionCaseName property.Name)
                   | GlueMember.IndexSignature indexSignature -> takesExtraFields indexSignature
                   | _ -> false
               )
        | None -> false

    // A type parameter no member names is bound by the specialized alias when it has a
    // default, and can only be given explicitly at the call site otherwise
    let typeParametersAreInferable =
        let mentioned =
            members
            |> Option.defaultValue info.Members
            |> List.collect memberTypeParameterNames
            |> Set.ofList

        info.TypeParameters
        |> List.forall (fun typeParameter ->
            mentioned.Contains typeParameter.Name || typeParameter.Default.IsSome
        )

    // The declarations of a merged interface are generated as one interface
    let isDeclaredOnce =
        typeMemory
        |> List.choose (
            function
            | GlueType.Interface candidate when candidate.FullName = info.FullName -> Some candidate
            | _ -> None
        )
        |> List.distinct
        |> List.length
        |> fun count -> count <= 1

    hasOnlyProperties && typeParametersAreInferable && isDeclaredOnce
