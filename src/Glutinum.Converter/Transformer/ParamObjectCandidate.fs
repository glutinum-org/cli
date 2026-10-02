module Glutinum.Converter.Transformer.ParamObjectCandidate

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open System.Collections.Generic
open Glutinum.Converter.Transformer.Utils
open Glutinum.Converter.Transformer.TypeParameters

let rec private typeReferenceFullNames (glueType: GlueType) =
    match glueType with
    | GlueType.TypeReference typeReference -> [ typeReference.FullName ]
    | GlueType.Union(GlueTypeUnion cases) -> cases |> List.collect typeReferenceFullNames
    | _ -> []

let private memberParameters (glueMember: GlueMember) =
    match glueMember with
    | GlueMember.Method info -> info.Parameters
    | GlueMember.MethodSignature info -> info.Parameters
    | GlueMember.CallSignature info -> info.Parameters
    | GlueMember.ConstructSignature info -> info.Parameters
    | GlueMember.Property _
    | GlueMember.GetAccessor _
    | GlueMember.SetAccessor _
    | GlueMember.IndexSignature _ -> []

let rec private parameters (glueType: GlueType) =
    match glueType with
    | GlueType.FunctionDeclaration info -> info.Parameters
    | GlueType.ClassDeclaration info ->
        (info.Constructors |> List.collect _.Parameters)
        @ (info.Members |> List.collect memberParameters)
    | GlueType.Interface info -> info.Members |> List.collect memberParameters
    | GlueType.TypeLiteral info -> info.Members |> List.collect memberParameters
    | GlueType.FunctionType info -> info.Parameters
    | GlueType.ConstructorType info -> info.Parameters
    | GlueType.TypeAliasDeclaration info -> parameters info.Type
    | GlueType.Variable info -> parameters info.Type
    | GlueType.ExportDefault innerType -> parameters innerType
    | _ -> []

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

                // `ReadableOptions<T> extends StreamOptions<T>` passes its own parameter on,
                // the members of the base name it the same way
                | GlueType.TypeReference typeReference ->
                    tryFindInterface typeReference.FullName |> Option.bind (resolve visited)

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
let private constrainedParameterFullNames (glueType: GlueType) =
    let ofSignature (typeParameters: GlueTypeParameter list) (parameters: GlueParameter list) =
        parameters
        |> List.collect (fun parameter ->
            match parameter.Type with
            | GlueType.TypeParameter name ->
                typeParameters
                |> List.tryFind (fun typeParameter -> typeParameter.Name = name)
                |> Option.bind _.Constraint
                |> Option.map typeReferenceFullNames
                |> Option.defaultValue []
            | _ -> []
        )

    let ofMember (glueMember: GlueMember) =
        match glueMember with
        | GlueMember.Method info -> ofSignature info.TypeParameters info.Parameters
        | GlueMember.MethodSignature info -> ofSignature info.TypeParameters info.Parameters
        | GlueMember.CallSignature info -> ofSignature info.TypeParameters info.Parameters
        | _ -> []

    let rec collect (glueType: GlueType) =
        match glueType with
        | GlueType.FunctionDeclaration info -> ofSignature info.TypeParameters info.Parameters
        | GlueType.ClassDeclaration info -> info.Members |> List.collect ofMember
        | GlueType.Interface info -> info.Members |> List.collect ofMember
        | GlueType.TypeLiteral info -> info.Members |> List.collect ofMember
        | GlueType.TypeAliasDeclaration info -> collect info.Type
        | GlueType.Variable info -> collect info.Type
        | GlueType.ExportDefault innerType -> collect innerType
        | _ -> []

    collect glueType

let rec private propertyTypeReferences (glueType: GlueType) =
    match glueType with
    | GlueType.TypeReference typeReference ->
        typeReference.FullName
        :: (typeReference.TypeArguments |> List.collect propertyTypeReferences)
    | GlueType.Union(GlueTypeUnion cases) -> cases |> List.collect propertyTypeReferences
    | GlueType.Array innerType
    | GlueType.ReadOnly innerType
    | GlueType.OptionalType innerType -> propertyTypeReferences innerType
    | GlueType.TupleType elements -> elements |> List.collect propertyTypeReferences
    | _ -> []

type State =
    {
        /// `WorkerOptions.resourceLimits` is the only place `ResourceLimits` appears: a declaration
        /// reached through the properties of a param object is built by the same caller
        ArgumentReachable: Set<string>
    }

let create (typeMemory: GlueType list) : State =
    let argumentReachable = HashSet<string>()

    let optionBags = Dictionary<string, GlueInterface>()

    for glueType in typeMemory do
        match glueType with
        | GlueType.Interface info when
            not info.Members.IsEmpty
            && info.Members
               |> List.forall (
                   function
                   | GlueMember.Property _ -> true
                   | _ -> false
               )
            ->
            optionBags.[info.FullName] <- info
        | _ -> ()

    for glueType in typeMemory do
        for parameter in parameters glueType do
            for fullName in typeReferenceFullNames parameter.Type do
                argumentReachable.Add fullName |> ignore

        for fullName in constrainedParameterFullNames glueType do
            argumentReachable.Add fullName |> ignore

    let mutable changed = true

    while changed do
        changed <- false

        for fullName in List.ofSeq argumentReachable do
            match optionBags.TryGetValue fullName with
            | true, info ->
                for glueMember in info.Members do
                    match glueMember with
                    | GlueMember.Property property ->
                        for referenced in propertyTypeReferences property.Type do
                            if argumentReachable.Add referenced then
                                changed <- true
                    | _ -> ()
            | _ -> ()

    {
        ArgumentReachable = Set.ofSeq argumentReachable
    }

let isCandidate (state: State) (typeMemory: GlueType list) (info: GlueInterface) =
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

    let isUsedAsArgument = state.ArgumentReachable.Contains info.FullName

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

    hasOnlyProperties
    && typeParametersAreInferable
    && isUsedAsArgument
    && isDeclaredOnce
