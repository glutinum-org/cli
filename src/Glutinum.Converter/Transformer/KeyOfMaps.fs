/// <summary>
/// <c>addEventListener&lt;K extends keyof HTMLElementEventMap&gt;(type: K, listener: (ev: HTMLElementEventMap[K]) =&gt; any)</c>
/// is generic in the event: the key is a <c>HTMLElementEventMap.Key&lt;'K&gt;</c> and <c>HTMLElementEventMap[K]</c>
/// is <c>'K</c>. The map declares <c>Key&lt;'V&gt;</c> and one typed key per member in <c>Keys</c>.
/// </summary>
module Glutinum.Converter.Transformer.KeyOfMaps

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open System.Collections.Generic
open Glutinum.Converter.Transformer.TypeParameters

type State =
    {
        /// Full names of the interfaces used as `keyof` constraint by a method
        Maps: Set<string>
        /// Names of those interfaces, TypeScript 6 gives an augmented interface a full name that
        /// differs from the one the constraint records
        MapNames: Set<string>
        /// `type Value<K extends keyof Map> = Map[K]` by its full name, gives the map
        IndexedAliases: Map<string, string>
    }

let create (typeMemory: GlueType list) : State =
    let maps = HashSet<string>()
    let mapNames = Dictionary<string, string>()
    let indexedAliases = Dictionary<string, string>()

    let collectFromTypeParameters (typeParameters: GlueTypeParameter list) =
        for typeParameter in typeParameters do
            match typeParameter.Constraint with
            | Some(GlueType.KeyOf(GlueType.TypeReference map)) when not map.IsStandardLibrary ->
                maps.Add map.FullName |> ignore
                mapNames.[map.FullName] <- map.Name
            | _ -> ()

    let collectFromMembers (members: GlueMember list) =
        for glueMember in members do
            match glueMember with
            | GlueMember.Method info -> collectFromTypeParameters info.TypeParameters
            | GlueMember.MethodSignature info -> collectFromTypeParameters info.TypeParameters
            // A callable interface is inlined as a method by the use site
            | GlueMember.CallSignature info -> collectFromTypeParameters info.TypeParameters
            | _ -> ()

    let rec collect (glueType: GlueType) =
        match glueType with
        | GlueType.Interface info -> collectFromMembers info.Members
        | GlueType.ClassDeclaration info -> collectFromMembers info.Members
        | GlueType.FunctionDeclaration info -> collectFromTypeParameters info.TypeParameters
        | GlueType.ExportDefault(GlueType.FunctionDeclaration info) ->
            collectFromTypeParameters info.TypeParameters
        | GlueType.TypeAliasDeclaration {
                                            FullName = fullName
                                            TypeParameters = [ { Name = name } ]
                                            Type = GlueType.IndexedAccessType {
                                                                                  ObjectType = GlueType.TypeReference map
                                                                                  IndexType = GlueType.TypeParameter indexName
                                                                              }
                                        } when name = indexName ->
            indexedAliases.[fullName] <- map.FullName
        | GlueType.ModuleDeclaration info -> info.Types |> List.iter collect
        | GlueType.FileModule info -> info.Types |> List.iter collect
        | _ -> ()

    typeMemory |> List.iter collect

    // A map another binding declares has no `Key` here
    let declared = HashSet<string>()

    let rec collectDeclared (glueType: GlueType) =
        match glueType with
        | GlueType.Interface info -> declared.Add info.FullName |> ignore
        | GlueType.ClassDeclaration info -> declared.Add info.FullName |> ignore
        | GlueType.TypeAliasDeclaration info -> declared.Add info.FullName |> ignore
        | GlueType.ModuleDeclaration info -> info.Types |> List.iter collectDeclared
        | GlueType.FileModule info -> info.Types |> List.iter collectDeclared
        | _ -> ()

    typeMemory |> List.iter collectDeclared

    let maps = maps |> Seq.filter declared.Contains |> Seq.toList

    {
        Maps = Set.ofList maps
        MapNames = maps |> List.map (fun fullName -> mapNames.[fullName]) |> Set.ofList
        IndexedAliases =
            indexedAliases
            |> Seq.map (fun (KeyValue(key, value)) -> key, value)
            |> Map.ofSeq
    }

let isMap (state: State) (fullName: string) = state.Maps.Contains fullName

/// The `keyof` constraint and the interface declaration can carry different full names
let isMapNamed (state: State) (fullName: string) (name: string) =
    state.Maps.Contains fullName || state.MapNames.Contains name

let private keyReference (map: GlueTypeReference) (typeArgument: GlueType) =
    ({
        Name = "Key"
        FullName = ""
        ModulePath = map.ModulePath @ [ Naming.sanitizeTypeName map.Name ]
        TypeArguments = [ typeArgument ]
        IsStandardLibrary = false
    }
    : GlueTypeReference)
    |> GlueType.TypeReference

let rec private substitute
    (state: State)
    (name: string)
    (map: GlueTypeReference)
    (glueType: GlueType)
    : GlueType
    =
    let substitute = substitute state name map

    match glueType with
    | GlueType.TypeParameter parameterName when parameterName = name ->
        keyReference map (GlueType.TypeParameter name)
    | GlueType.IndexedAccessType {
                                     ObjectType = GlueType.TypeReference object
                                     IndexType = GlueType.TypeParameter parameterName
                                 } when parameterName = name && object.FullName = map.FullName ->
        GlueType.TypeParameter name
    | GlueType.TypeReference {
                                 FullName = aliasName
                                 TypeArguments = [ GlueType.TypeParameter parameterName ]
                             } when
        parameterName = name
        && state.IndexedAliases.TryFind aliasName = Some map.FullName
        ->
        GlueType.TypeParameter name
    | GlueType.TypeReference typeReference ->
        GlueType.TypeReference
            { typeReference with
                TypeArguments = typeReference.TypeArguments |> List.map substitute
            }
    | GlueType.Union(GlueTypeUnion cases) ->
        GlueType.Union(GlueTypeUnion(cases |> List.map substitute))
    | GlueType.Array elementType -> GlueType.Array(substitute elementType)
    | GlueType.ReadOnly innerType -> GlueType.ReadOnly(substitute innerType)
    | GlueType.OptionalType innerType -> GlueType.OptionalType(substitute innerType)
    | GlueType.TupleType elements -> GlueType.TupleType(elements |> List.map substitute)
    | GlueType.FunctionType functionType ->
        GlueType.FunctionType
            { functionType with
                Parameters =
                    functionType.Parameters
                    |> List.map (fun parameter ->
                        { parameter with
                            Type = substitute parameter.Type
                        }
                    )
                Type = substitute functionType.Type
            }
    | _ -> glueType

let private expand
    (state: State)
    (typeParameters: GlueTypeParameter list)
    (parameters: GlueParameter list)
    (returnType: GlueType)
    =
    ((typeParameters, parameters, returnType), typeParameters)
    ||> List.fold (fun (typeParameters, parameters, returnType) typeParameter ->
        match typeParameter.Constraint with
        | Some(GlueType.KeyOf(GlueType.TypeReference map)) when isMap state map.FullName ->
            let substitute = substitute state typeParameter.Name map

            typeParameters
            |> List.map (fun candidate ->
                if candidate.Name = typeParameter.Name then
                    { candidate with Constraint = None }
                else
                    candidate
            ),
            parameters
            |> List.map (fun parameter ->
                { parameter with
                    Type = substitute parameter.Type
                }
            ),
            substitute returnType
        | _ -> typeParameters, parameters, returnType
    )

let expandFunction (state: State) (info: GlueFunctionDeclaration) : GlueFunctionDeclaration =
    let typeParameters, parameters, returnType =
        expand state info.TypeParameters info.Parameters info.Type

    { info with
        TypeParameters = typeParameters
        Parameters = parameters
        Type = returnType
    }

let expandMembers (state: State) (members: GlueMember list) : GlueMember list =
    members
    |> List.map (fun glueMember ->
        match glueMember with
        | GlueMember.MethodSignature info ->
            let typeParameters, parameters, returnType =
                expand state info.TypeParameters info.Parameters info.Type

            GlueMember.MethodSignature
                { info with
                    TypeParameters = typeParameters
                    Parameters = parameters
                    Type = returnType
                }
        | GlueMember.Method info ->
            let typeParameters, parameters, returnType =
                expand state info.TypeParameters info.Parameters info.Type

            GlueMember.Method
                { info with
                    TypeParameters = typeParameters
                    Parameters = parameters
                    Type = returnType
                }
        | _ -> glueMember
    )

/// The module of a map: `Key<'V>` and the typed keys
/// `transformType` takes the member the type belongs to, its delegate is named after it
let keysModule
    (transformType: string -> GlueType -> FSharpType)
    (name: string)
    (members: GlueMember list)
    : FSharpType
    =
    let keyType (valueType: FSharpType) =
        ({
            Name = "Key"
            FullName = ""
            ModulePath = []
            TypeArguments = [ valueType ]
            Type = FSharpType.Discard
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    let keys =
        members
        |> List.choose (
            function
            | GlueMember.Property property ->
                {
                    Attributes = [ FSharpAttribute.Text $"Emit(\"\\\"%s{property.Name}\\\"\")" ]
                    Name = Naming.sanitizeName property.Name
                    OriginalName = property.Name
                    Parameters = []
                    TypeParameters = []
                    Type = keyType (transformType property.Name property.Type)
                    IsOptional = false
                    IsStatic = true
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Property
                |> Some
            | _ -> None
        )

    ({
        Name = name
        IsRecursive = false
        ImportSpecifier = None
        Types =
            [
                ({
                    Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
                    Name = "Key"
                    XmlDoc = []
                    OriginalName = "Key"
                    TypeParameters = declaredTypeParameters [ "V" ]
                    Members = []
                    Inheritance = []
                }
                : FSharpInterface)
                |> FSharpType.Interface

                // `[<AbstractClass>]` on a type without a member is an interface to F#
                if not keys.IsEmpty then
                    ({
                        Attributes = [ FSharpAttribute.AbstractClass; FSharpAttribute.Erase ]
                        Name = "Keys"
                        XmlDoc = []
                        OriginalName = "Keys"
                        TypeParameters = []
                        Members = keys
                        Inheritance = []
                    }
                    : FSharpInterface)
                    |> FSharpType.Interface
            ]
    }
    : FSharpModule)
    |> FSharpType.Module
