/// What an F# interface can inherit from the heritage clauses of a declaration
module Glutinum.Converter.Transformer.Heritage

open Fable.Core
open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST

// `inherit obj` or `inherit JS.Uint8Array` is invalid, those base types are not interfaces
/// `interface dirFS extends String {}`: the lib type is mapped to the F# primitive by name
let private primitiveNames =
    set [ "string"; "float"; "int"; "bool"; "obj"; "unit"; "bigint" ]

let isInheritableType (typ: FSharpType) =
    match typ with
    | FSharpType.Object
    | FSharpType.Primitive _ -> false
    | FSharpType.TypeReference typeReference ->
        not (typeReference.Name.StartsWith "JS.")
        && typeReference.Name <> "Action"
        && not (primitiveNames.Contains typeReference.Name)
    | _ -> true

/// `declare abstract class XRSystem implements XRSystem {}` merged with the interface
let isSelfHeritage (fullName: string) (heritageClause: GlueType) =
    match heritageClause with
    | GlueType.TypeReference typeReference -> fullName <> "" && typeReference.FullName = fullName
    | _ -> false

let isErrorHeritage (heritageClause: GlueType) =
    match heritageClause with
    | GlueType.TypeReference typeReference ->
        typeReference.IsStandardLibrary && typeReference.Name = "Error"
    | _ -> false

// `Array<T>` is generated as `ResizeArray<T>`, a class, which an interface can't inherit
let isArrayHeritage (heritageClause: GlueType) =
    match heritageClause with
    | GlueType.TypeReference typeReference ->
        typeReference.IsStandardLibrary && typeReference.Name = "Array"
    | _ -> false

/// F# rejects an abstract property redeclared with the signature of a base class one
let withoutBaseClassProperties
    (typeMemory: GlueType list)
    (heritageClauses: GlueType list)
    (members: GlueMember list)
    : GlueMember list
    =
    // The base class can be declared in a namespace
    let rec tryFindClass (name: string) (glueTypes: GlueType list) : GlueClassDeclaration option =
        glueTypes
        |> List.tryPick (fun glueType ->
            match glueType with
            | GlueType.ClassDeclaration candidate when candidate.Name = name -> Some candidate
            | GlueType.ModuleDeclaration moduleDeclaration ->
                tryFindClass name moduleDeclaration.Types
            | GlueType.FileModule fileModule -> tryFindClass name fileModule.Types
            | _ -> None
        )

    let rec baseProperties (visited: Set<string>) (heritageClauses: GlueType list) =
        heritageClauses
        |> List.collect (fun heritageClause ->
            match heritageClause with
            | GlueType.TypeReference typeReference when not (visited.Contains typeReference.Name) ->
                tryFindClass typeReference.Name typeMemory
                |> Option.map (fun baseClass ->
                    (baseClass.Members
                     |> List.choose (
                         function
                         | GlueMember.Property property -> Some property.Name
                         | _ -> None
                     ))
                    @ baseProperties (visited.Add typeReference.Name) baseClass.HeritageClauses
                )
                |> Option.defaultValue []
            | _ -> []
        )

    let inherited = baseProperties Set.empty heritageClauses |> set

    members
    |> List.filter (
        function
        | GlueMember.Property property -> not (inherited.Contains property.Name)
        | _ -> true
    )

type private BaseDeclaration =
    {
        TypeParameters: GlueTypeParameter list
        Members: GlueMember list
        HeritageClauses: GlueType list
    }

let rec private declarations (glueTypes: GlueType list) =
    glueTypes
    |> List.collect (fun glueType ->
        match glueType with
        | GlueType.ClassDeclaration info ->
            [
                info.FullName,
                info.Name,
                {
                    TypeParameters = info.TypeParameters
                    Members = info.Members
                    HeritageClauses = info.HeritageClauses
                }
            ]
        | GlueType.Interface info ->
            [
                info.FullName,
                info.Name,
                {
                    TypeParameters = info.TypeParameters
                    Members = info.Members
                    HeritageClauses = info.HeritageClauses
                }
            ]
        | GlueType.ModuleDeclaration info -> declarations info.Types
        | GlueType.FileModule info -> declarations info.Types
        | _ -> []
    )

/// An interface declared several times is one declaration, a name alone has to be unique
let private tryFindBase (typeMemory: GlueType list) (typeReference: GlueTypeReference) =
    let candidates = declarations typeMemory

    let found =
        if typeReference.FullName <> "" then
            candidates
            |> List.filter (fun (fullName, _, _) -> fullName = typeReference.FullName)
        else
            match candidates |> List.filter (fun (_, name, _) -> name = typeReference.Name) with
            | [ single ] -> [ single ]
            | _ -> []
        |> List.map (fun (_, _, declaration) -> declaration)

    match found with
    | [] -> None
    | first :: _ ->
        Some
            { first with
                Members = found |> List.collect _.Members
                HeritageClauses = found |> List.collect _.HeritageClauses
            }

let private memberName (glueMember: GlueMember) =
    match glueMember with
    | GlueMember.Method info -> Some info.Name
    | GlueMember.MethodSignature info -> Some info.Name
    | GlueMember.Property info -> Some info.Name
    | GlueMember.GetAccessor info -> Some info.Name
    | GlueMember.SetAccessor info -> Some info.Name
    | GlueMember.CallSignature _
    | GlueMember.IndexSignature _
    | GlueMember.ConstructSignature _ -> None

/// The reader records the enclosing type parameters and the documentation on a function type
let rec private comparable (glueType: GlueType) : GlueType =
    match glueType with
    | GlueType.FunctionType functionType ->
        GlueType.FunctionType
            { functionType with
                Documentation = []
                TypeParameters = []
                OwnTypeParameterNames = []
                Type = comparable functionType.Type
                Parameters =
                    functionType.Parameters
                    |> List.map (fun parameter ->
                        { parameter with
                            Name = ""
                            Type = comparable parameter.Type
                        }
                    )
            }
    | GlueType.Union(GlueTypeUnion cases) ->
        GlueType.Union(GlueTypeUnion(cases |> List.map comparable))
    | GlueType.Array elementType -> GlueType.Array(comparable elementType)
    | GlueType.ReadOnly innerType -> GlueType.ReadOnly(comparable innerType)
    | GlueType.OptionalType innerType -> GlueType.OptionalType(comparable innerType)
    | GlueType.TupleType elements -> GlueType.TupleType(elements |> List.map comparable)
    | GlueType.TypeReference typeReference ->
        GlueType.TypeReference
            { typeReference with
                TypeArguments = typeReference.TypeArguments |> List.map comparable
            }
    | _ -> glueType

/// The parameters with the type parameters of the signature replaced by their constraint
let private shape (typeParameters: GlueTypeParameter list) (parameters: GlueParameter list) =
    let substitutions =
        typeParameters
        |> List.map (fun typeParameter ->
            typeParameter.Name,
            typeParameter.Constraint
            |> Option.defaultValue (GlueType.Primitive GluePrimitive.Any)
        )
        |> Map.ofList

    parameters
    |> List.map (GlueSubstitution.substituteParameter substitutions)
    |> List.map (fun parameter ->
        comparable parameter.Type, parameter.IsOptional, parameter.IsSpread
    )

let private tryShape (glueMember: GlueMember) =
    match glueMember with
    | GlueMember.Method info -> Some(info.Name, shape info.TypeParameters info.Parameters)
    | GlueMember.MethodSignature info -> Some(info.Name, shape info.TypeParameters info.Parameters)
    | _ -> None

/// F# takes the derived one of two identical signatures and the one given all its arguments,
/// it cannot choose when both leave an optional argument out
let private collides
    (derived: (GlueType * bool * bool) list)
    (baseShape: (GlueType * bool * bool) list)
    =
    let required (parameters: (GlueType * bool * bool) list) =
        parameters
        |> List.filter (fun (_, isOptional, _) -> not isOptional)
        |> List.length

    let shared = max (required derived) (required baseShape)

    derived <> baseShape
    && shared < derived.Length
    && shared < baseShape.Length
    && List.forall2
        (fun (derivedType, _, _) (baseType, _, _) -> derivedType = baseType)
        (List.take shared derived)
        (List.take shared baseShape)

let private hasCollision (members: GlueMember list) (baseMembers: GlueMember list) =
    let derived = members |> List.choose tryShape

    baseMembers
    |> List.choose tryShape
    |> List.exists (fun (baseName, baseShape) ->
        derived
        |> List.exists (fun (name, derivedShape) ->
            name = baseName && collides derivedShape baseShape
        )
    )

let private isSameKind (a: GlueMember) (b: GlueMember) =
    match a, b with
    | GlueMember.CallSignature _, GlueMember.CallSignature _
    | GlueMember.IndexSignature _, GlueMember.IndexSignature _
    | GlueMember.ConstructSignature _, GlueMember.ConstructSignature _ -> true
    | _ -> false

/// `class RecordService extends CrudService { getOne(id, options?: RecordOptions) }` has one
/// `getOne` in TypeScript, the subclass takes the base heritage and the members it does not redeclare
let withoutRedeclaredBases
    (typeMemory: GlueType list)
    (members: GlueMember list)
    (heritageClauses: GlueType list)
    : GlueType list * GlueMember list
    =
    let rec resolve
        (visited: Set<string>)
        (members: GlueMember list)
        (heritageClauses: GlueType list)
        =
        ((members, []), heritageClauses)
        ||> List.fold (fun (members, kept) heritageClause ->
            match heritageClause with
            | GlueType.TypeReference typeReference ->
                let key =
                    if typeReference.FullName <> "" then
                        typeReference.FullName
                    else
                        typeReference.Name

                match tryFindBase typeMemory typeReference with
                | Some baseDeclaration when not (visited.Contains key) ->
                    let substitutions =
                        GlueSubstitution.ofTypeArguments
                            baseDeclaration.TypeParameters
                            typeReference.TypeArguments

                    let baseMembers =
                        baseDeclaration.Members
                        |> List.map (GlueSubstitution.substituteMember substitutions)

                    if hasCollision members baseMembers then
                        let names = members |> List.choose memberName |> Set.ofList

                        let copied =
                            baseMembers
                            |> List.filter (fun baseMember ->
                                match memberName baseMember with
                                | Some name -> not (names.Contains name)
                                | None -> not (members |> List.exists (isSameKind baseMember))
                            )

                        let baseClauses =
                            baseDeclaration.HeritageClauses
                            |> List.map (GlueSubstitution.substitute substitutions)

                        let members, inner =
                            resolve (visited.Add key) (members @ copied) baseClauses

                        members, kept @ inner
                    else
                        members, kept @ [ heritageClause ]
                | _ -> members, kept @ [ heritageClause ]
            | _ -> members, kept @ [ heritageClause ]
        )

    let members, kept = resolve Set.empty members heritageClauses
    kept, members
