/// The type parameters a declaration, a member or a type names
module Glutinum.Converter.Transformer.TypeParameters

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST

/// A type parameter without constraint nor default
let unconstrained (name: string) : GlueTypeParameter =
    {
        Name = name
        Constraint = None
        Default = None
    }

/// The type parameters a type declares, from their names
let declaredTypeParameters (names: string list) : FSharpTypeParameter list =
    names
    |> List.map (fun name ->
        FSharpTypeParameterInfo.Create name |> FSharpTypeParameter.FSharpTypeParameter
    )

/// The type parameters a reference passes along, from their names
let typeArguments (names: string list) : FSharpTypeParameter list =
    names |> List.map (FSharpType.TypeParameter >> FSharpTypeParameter.FSharpType)

let rec typeParameterNames (glueType: GlueType) : string list =
    match glueType with
    | GlueType.TypeParameter name -> [ name ]
    | GlueType.TypeReference typeReference ->
        typeReference.TypeArguments |> List.collect typeParameterNames
    | GlueType.Array glueType
    | GlueType.ReadOnly glueType
    | GlueType.OptionalType glueType -> typeParameterNames glueType
    | GlueType.Union(GlueTypeUnion cases) -> cases |> List.collect typeParameterNames
    | GlueType.TupleType glueTypes -> glueTypes |> List.collect typeParameterNames
    // The function's own type parameters are bound by it
    | GlueType.FunctionType functionType ->
        typeParameterNames functionType.Type
        @ (functionType.Parameters
           |> List.collect (fun parameter -> typeParameterNames parameter.Type))
        |> List.filter (fun name -> not (List.contains name functionType.OwnTypeParameterNames))
    | GlueType.TypeLiteral typeLiteral ->
        typeLiteral.Members |> List.collect memberTypeParameterNames
    | _ -> []

and memberTypeParameterNames (glueMember: GlueMember) : string list =
    // A method declares the type parameters of its own signature, the ones it takes from an
    // enclosing scope are declared by the type holding it
    let fromSignature
        (own: GlueTypeParameter list)
        (parameters: GlueParameter list)
        (returnType: GlueType)
        =
        let own = own |> List.map _.Name |> Set.ofList

        (typeParameterNames returnType
         @ (parameters |> List.collect (fun parameter -> typeParameterNames parameter.Type)))
        |> List.filter (fun name -> not (own.Contains name))

    match glueMember with
    | GlueMember.Property { Type = typ }
    | GlueMember.GetAccessor { Type = typ }
    | GlueMember.SetAccessor { ArgumentType = typ }
    | GlueMember.IndexSignature { Type = typ } -> typeParameterNames typ
    | GlueMember.Method info -> fromSignature info.TypeParameters info.Parameters info.Type
    | GlueMember.MethodSignature info -> fromSignature info.TypeParameters info.Parameters info.Type
    | GlueMember.CallSignature info -> fromSignature info.TypeParameters info.Parameters info.Type
    | GlueMember.ConstructSignature info -> fromSignature [] info.Parameters info.Type

let rec mentionsTypeParameter (name: string) (glueType: GlueType) : bool =
    let mentions = mentionsTypeParameter name

    match glueType with
    | GlueType.TypeParameter typeParameterName -> typeParameterName = name
    | GlueType.TypeReference typeReference -> typeReference.TypeArguments |> List.exists mentions
    | GlueType.ThisType thisType ->
        thisType.TypeParameters
        |> List.exists (fun typeParameter -> typeParameter.Name = name)
    | GlueType.Array glueType
    | GlueType.ReadOnly glueType
    | GlueType.OptionalType glueType -> mentions glueType
    | GlueType.Union(GlueTypeUnion cases) -> cases |> List.exists mentions
    | GlueType.TupleType glueTypes -> glueTypes |> List.exists mentions
    | GlueType.FunctionType functionType ->
        not (List.contains name functionType.OwnTypeParameterNames)
        && (mentions functionType.Type
            || functionType.Parameters
               |> List.exists (fun parameter -> mentions parameter.Type))
    | GlueType.TypeLiteral typeLiteral ->
        typeLiteral.Members
        |> List.exists (
            function
            | GlueMember.Property property -> mentions property.Type
            | GlueMember.MethodSignature methodSignature ->
                mentions methodSignature.Type
                || methodSignature.Parameters
                   |> List.exists (fun parameter -> mentions parameter.Type)
            | _ -> false
        )
    | _ -> false

/// The type parameters an F# type names. A nested anonymous type is a `Mapped` whose arguments
/// are themselves `Mapped`, named `'VF` with the tick already in the name.
let rec namedTypeParameters (typ: FSharpType) : string list =
    let ofTypeParameter (typeParameter: FSharpTypeParameter) =
        match typeParameter with
        | FSharpTypeParameter.FSharpTypeParameter info -> [ info.Name ]
        | FSharpTypeParameter.FSharpType typ -> namedTypeParameters typ

    match typ with
    | FSharpType.TypeParameter name -> [ name ]
    | FSharpType.Mapped info ->
        [
            if info.Name.StartsWith "'" then
                info.Name.Substring 1

            yield! info.TypeParameters |> List.collect ofTypeParameter
        ]
    | typ -> FSharpType.children typ |> List.collect namedTypeParameters

/// An interface can't absorb a type parameter it does not declare, a class definition
/// generalizes it. The object type stays a class until the generator tracks it.
let membersNameUndeclaredTypeParameters (declared: string list) (members: FSharpMember list) =
    let declared = set declared

    // A member declares the type parameters of its own signature
    let names (own: FSharpTypeParameter list) (typ: FSharpType) (parameters: FSharpParameter list) =
        let own =
            own
            |> List.choose (
                function
                | FSharpTypeParameter.FSharpTypeParameter info -> Some info.Name
                | FSharpTypeParameter.FSharpType _ -> None
            )
            |> Set.ofList

        (namedTypeParameters typ
         @ (parameters |> List.collect (fun parameter -> namedTypeParameters parameter.Type)))
        |> List.exists (fun name -> not (declared.Contains name || own.Contains name))

    members
    |> List.exists (
        function
        | FSharpMember.Method info
        | FSharpMember.Property info -> names info.TypeParameters info.Type info.Parameters
        | FSharpMember.StaticMember info -> names info.TypeParameters info.Type info.Parameters
    )

/// The type each own type parameter of a function type stands for outside a generic position.
/// `<P, P2 = P>`: the default of `P2` is another own parameter, which has none itself.
let ownTypeParameterDefaults (functionTypeInfo: GlueFunctionType) =
    let listed = functionTypeInfo.TypeParameters |> List.map _.Name |> Set.ofList

    let own =
        functionTypeInfo.TypeParameters
        |> List.filter (fun typeParameter ->
            List.contains typeParameter.Name functionTypeInfo.OwnTypeParameterNames
        )

    // An inherited signature names its own type parameters without listing them
    let inherited =
        functionTypeInfo.OwnTypeParameterNames
        |> List.filter (fun name -> not (listed.Contains name))
        |> List.map unconstrained

    let declared = own @ inherited

    let byName =
        declared
        |> List.map (fun typeParameter -> typeParameter.Name, typeParameter)
        |> Map.ofList

    // A parameter with no default falls back to its constraint when F# seals it,
    // `P extends string` can only ever be `string`
    let withoutDefault (typeParameter: GlueTypeParameter) =
        match typeParameter.Constraint with
        | Some(GlueType.Primitive _ as constraintType) -> constraintType
        | _ -> GlueType.Primitive GluePrimitive.Any

    let rec resolve (seen: Set<string>) (typeParameter: GlueTypeParameter) =
        match typeParameter.Default with
        | None -> withoutDefault typeParameter
        | Some(GlueType.TypeParameter name) when byName.ContainsKey name ->
            if seen.Contains name then
                GlueType.Primitive GluePrimitive.Any
            else
                resolve (Set.add name seen) byName.[name]
        | Some glueType -> glueType

    let resolved =
        declared
        |> List.map (fun typeParameter ->
            typeParameter.Name, resolve (Set.singleton typeParameter.Name) typeParameter
        )
        |> Map.ofList

    // `VF = (c: Context<any, P2>) => any`: the default mentions another own parameter
    resolved
    |> Map.map (fun name glueType ->
        GlueSubstitution.substitute (Map.remove name resolved) glueType
    )

// When a class or interface is declared with a generic and a default type, we need to expose an alias
// for all the version with the default type set instead of a generic parameter.
//
// TypeScript:
//
// interface Task {}
// export class Type1<A extends Task = Task> {}
//
// F#:
//
// type User<'T when 'T :> Task> = interface end
// type User = User<Task>
let aliasArities (typeParameters: FSharpTypeParameter list) (aliases: FSharpType list) =
    typeParameters.Length
    :: (aliases
        |> List.choose (
            function
            | FSharpType.TypeAlias aliasInfo -> Some aliasInfo.TypeParameters.Length
            | _ -> None
        ))
