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
