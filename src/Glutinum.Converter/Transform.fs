module rec Glutinum.Converter.Transform

open Fable.Core
open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open System.Collections.Generic
open Glutinum.Converter.Transformer
open Glutinum.Converter.Transformer.Context
open Glutinum.Converter.Transformer.Comment
open Glutinum.Converter.Transformer.Utils
open Glutinum.Converter.Transformer.CallableProperties
open Glutinum.Converter.Transformer.Enum
open Glutinum.Converter.Transformer.TypeParameters
open Glutinum.Converter.Transformer.Heritage
open Glutinum.Converter.Transformer.TypeParameter

let private mapTypeNameToFableCoreAwareName
    (context: TransformContext)
    (typeReference: GlueTypeReference)
    =
    let mappedName =
        // When exposing a type, we also need to do it
        if typeReference.IsStandardLibrary then
            match typeReference.Name with
            | name when iteratorNames.Contains name -> "Iterable"
            // `Date` and its constructor are the ones of `Glutinum.Types`
            | "Date" ->
                context.ExposeReadonlyArray()
                "Date"
            | "Promise" -> "JS.Promise"
            | "Uint8Array" -> "JS.Uint8Array"
            | "Int8Array" -> "JS.Int8Array"
            | "Uint8ClampedArray" -> "JS.Uint8ClampedArray"
            | "Int16Array" -> "JS.Int16Array"
            | "Uint16Array" -> "JS.Uint16Array"
            | "Int32Array" -> "JS.Int32Array"
            | "Uint32Array" -> "JS.Uint32Array"
            | "Float32Array" -> "JS.Float32Array"
            | "Float64Array" -> "JS.Float64Array"
            | "Array" -> "ResizeArray"
            | "ArrayBuffer" -> "JS.ArrayBuffer"
            | "ArrayBufferView" -> "JS.ArrayBufferView"
            | "DataView" -> "JS.DataView"
            | "Map" -> "JS.Map"
            | "Set" -> "JS.Set"
            | "WeakMap" -> "JS.WeakMap"
            | "WeakSet" -> "JS.WeakSet"
            | "FlatArray"
            | "Symbol"
            | "Object" -> "obj"
            // A JavaScript number is a `float` and a string a `string`, the interfaces are not types
            | "Number" -> "float"
            | "String" -> "string"
            // The `Intl` namespace is not generated yet
            | _ when typeReference.FullName.StartsWith "Intl." -> "obj"
            | "Boolean" -> "bool"
            | "Function" -> "Action"
            | "Error"
            | "EvalError"
            | "RangeError"
            | "ReferenceError"
            | "SyntaxError"
            | "TypeError"
            | "URIError" -> "Exception"
            | "BigInt" -> "bigint"
            | "PromiseConstructor"
            | "PromiseConstructorLike" -> "obj"
            | name -> name
        else
            typeReference.Name

    context.ExposeTypeAlias mappedName

    mappedName

let private unwrapOptionIfAlreadyOptional
    (context: TransformContext)
    (typ: GlueType)
    (isOptional: bool)
    =
    // If the property is optional, we want to unwrap the option type
    // This is to prevent generating a `string option option`
    let typ' = transformType context typ

    if isOptional then
        tryUnwrapOption typ' |> Option.defaultValue typ'
    else
        typ'

let private transformTupleType (context: TransformContext) (glueTypes: GlueType list) : FSharpType =
    match glueTypes with
    | [] -> FSharpType.Object
    // `[S1]` is an array of one element, F# has no tuple of one
    | [ single ] -> transformType context single |> FSharpType.ResizeArray
    | _ -> glueTypes |> List.map (transformType context) |> FSharpType.Tuple

/// The anonymous interface of the scope, `OriginalName` is the scope as written
let private anonymousInterface
    (context: TransformContext)
    (name: string)
    (typeParameterNames: string list)
    (members: FSharpMember list)
    (inheritance: FSharpType list)
    : FSharpInterface
    =
    {
        XmlDoc = []
        Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
        Name = name
        OriginalName = context.OriginalScopeName
        TypeParameters = declaredTypeParameters typeParameterNames
        Members = members
        Inheritance = inheritance
    }

/// The reference to an anonymous type exposed by the scope
let private mappedReference
    (context: TransformContext)
    (name: string)
    (typeParameterNames: string list)
    : FSharpType
    =
    ({
        Name = context.ReferenceName name
        TypeParameters = typeArguments typeParameterNames
    }
    : FSharpMapped)
    |> FSharpType.Mapped

module TypeLiteral =

    let tryFindIterableType (context: TransformContext) (members: GlueMember list) =

        let makeIterable (typeParameter: FSharpTypeParameter) =
            ({
                Name = "Iterable"
                TypeParameters = [ typeParameter ]
            }
            : FSharpMapped)
            |> FSharpType.Mapped

        let makeIterableObj () =
            FSharpType.Object |> FSharpTypeParameter.FSharpType |> makeIterable

        members
        |> List.choose (
            function
            | GlueMember.MethodSignature methodSignature ->
                if methodSignature.Name = "[Symbol.iterator]" then
                    context.ExposeIterable()

                    Some methodSignature
                else
                    None
            | _ -> None
        )
        |> List.tryHead
        |> Option.map (fun methodSignature ->
            match methodSignature.Type with
            // `IterableIterator<T>`, `ArrayIterator<T>`, `Iterator<T>`: the element is the first argument
            | GlueType.TypeReference { TypeArguments = elementType :: _ } ->
                transformType context elementType
                |> FSharpTypeParameter.FSharpType
                |> makeIterable

            | GlueType.TypeReference _ -> makeIterableObj ()

            // `{ next(): { value: T } }`: the element is the `value` of `next`
            | GlueType.TypeLiteral typeLiteralInfo ->
                typeLiteralInfo.Members
                |> List.collect (
                    function
                    | GlueMember.MethodSignature {
                                                     Name = "next"
                                                     Type = GlueType.TypeLiteral nextTypeLiteralInfo
                                                 } ->
                        nextTypeLiteralInfo.Members
                        |> List.choose (
                            function
                            | GlueMember.Property property when property.Name = "value" ->
                                Some(transformType context property.Type)
                            | _ -> None
                        )
                    | _ -> []
                )
                |> List.tryLast
                |> Option.defaultValue FSharpType.Object
                |> FSharpTypeParameter.FSharpType
                |> makeIterable

            | _ -> makeIterableObj ()
        )

module private UtilityType =
    /// `Readonly<{ a: string }>`: the interface of the members, every one read-only
    let private readonlyInterface (context: TransformContext) (members: GlueMember list) =
        let typeParameterNames =
            members |> List.collect memberTypeParameterNames |> List.distinct

        let members =
            TransformMembers.toFSharpMember context members
            |> TransformMembers.forceReadonly

        typeParameterNames,
        anonymousInterface context context.CurrentScopeName typeParameterNames members []

    /// `Readonly<A | B>`: a union of the read-only interfaces
    let private readonlyUnion (context: TransformContext) (interfaces: GlueInterface list) =
        let isLarge = interfaces.Length > 9

        let name =
            if isLarge then
                context.NewTypeName()
            else
                $"U{interfaces.Length}"

        // An erased union takes the scope itself
        let caseContext =
            if isLarge then
                context
            else
                sanitizeNameAndPushScope name context |> snd

        let caseTypes =
            interfaces
            |> List.map (fun glueInterface ->
                let context = caseContext.PushScope $"ReadOnly{glueInterface.Name}"
                let initialInterface = transformInterface context glueInterface

                let adaptedInterface =
                    { initialInterface with
                        Members = initialInterface.Members |> TransformMembers.forceReadonly
                    }

                { adaptedInterface with
                    Name = context.CurrentScopeName
                }
                |> FSharpType.Interface
                |> context.ExposeType

                { adaptedInterface with
                    Name = context.FullName
                }
                |> FSharpType.Interface
            )

        if isLarge then
            erasedUnion name [] caseTypes |> FSharpType.Union |> context.ExposeType

            referenceToExposed context name []
        else
            ({
                Attributes = []
                Name = name
                Cases = caseTypes |> List.map FSharpUnionCase.Typed
                IsOptional = false
                TypeParameters = []
                Constants = []
            }
            : FSharpUnion)
            |> FSharpType.Union

    /// `Readonly<T>` used as a type: the interface is exposed to the scope and referenced
    let transformReadOnlyInline (context: TransformContext) (readonlyInfo: GlueReadonly) =
        match readonlyInfo with
        | GlueReadonly.Members members ->
            let typeParameterNames, interfaceInfo = readonlyInterface context members

            context.ExposeType(FSharpType.Interface interfaceInfo)

            ({
                Name = context.FullName
                TypeParameters = typeArguments typeParameterNames
            }
            : FSharpMapped)
            |> FSharpType.Mapped

        | GlueReadonly.Union interfaces -> readonlyUnion context interfaces

    /// `type X = Readonly<T>`: the declaration of `X`, the union through its alias
    let transformReadOnlyDeclaration
        (context: TransformContext)
        (readonlyInfo: GlueReadonly)
        (makeTypeAlias: FSharpType -> FSharpType)
        =
        match readonlyInfo with
        | GlueReadonly.Members members ->
            readonlyInterface context members |> snd |> FSharpType.Interface
        | GlueReadonly.Union interfaces -> readonlyUnion context interfaces |> makeTypeAlias

    /// `Partial<T>`, `Record<K, V>`, `Omit<T, K>`, `Pick<T, K>`, `ReturnType<T>` and `Readonly<T>`
    let transform (context: TransformContext) (utilityType: GlueUtilityType) : FSharpType =
        match utilityType with
        | GlueUtilityType.Partial interfaceInfo ->
            let name = context.NewTypeName()

            let freeTypeParameterNames =
                if interfaceInfo.TypeParameters.IsEmpty then
                    interfaceInfo.Members |> List.collect memberTypeParameterNames |> List.distinct
                else
                    []

            let interfaceInfo =
                { interfaceInfo with
                    TypeParameters =
                        interfaceInfo.TypeParameters @ List.map unconstrained freeTypeParameterNames
                }

            transformInterface context interfaceInfo
            |> Interface.makePartial name
            |> FSharpType.Interface
            |> context.ExposeType

            ({
                Name = context.ReferenceName name
                FullName = context.FullName
                ModulePath = []
                TypeArguments = freeTypeParameterNames |> List.map FSharpType.TypeParameter
                Type = FSharpType.Discard
            }
            : FSharpTypeReference)
            |> FSharpType.TypeReference

        // `Record<string, any>` is any object, a nominal type per use site would keep them apart
        | GlueUtilityType.Record {
                                     KeyType = GlueType.Primitive GluePrimitive.String
                                     ValueType = GlueType.Primitive GluePrimitive.Any
                                 } -> FSharpType.Object

        | GlueUtilityType.Record recordInfo ->
            let name = context.NewTypeName()

            let freeTypeParameterNames =
                typeParameterNames recordInfo.KeyType @ typeParameterNames recordInfo.ValueType
                |> List.distinct

            let typeParameters = List.map unconstrained freeTypeParameterNames

            transformRecord context name typeParameters recordInfo |> context.ExposeType

            mappedReference context name freeTypeParameterNames

        | GlueUtilityType.ReturnType innerType
        | GlueUtilityType.ThisParameterType innerType -> transformType context innerType

        | GlueUtilityType.Omit members
        | GlueUtilityType.Pick members ->
            let name = context.NewTypeName()

            // `Omit<TimeoutConfig<T, any, M>, "with">` keeps the type parameters of the enclosing declaration
            let freeTypeParameterNames =
                members |> List.collect memberTypeParameterNames |> List.distinct

            let freeTypeParameters = declaredTypeParameters freeTypeParameterNames

            let creates =
                if not members.IsEmpty && isDataObject members then
                    let returnType =
                        ({
                            Name = name
                            TypeParameters = typeArguments freeTypeParameterNames
                        }
                        : FSharpMapped)
                        |> FSharpType.Mapped

                    paramObjectCreateMembers context returnType members
                else
                    []

            anonymousInterface
                context
                name
                freeTypeParameterNames
                (TransformMembers.toFSharpMember context members @ creates)
                []
            |> FSharpType.Interface
            |> context.ExposeType

            ({
                Name = context.ReferenceName name
                TypeParameters = freeTypeParameters
            }
            : FSharpMapped)
            |> FSharpType.Mapped

        | GlueUtilityType.Readonly readonlyInfo ->
            UtilityType.transformReadOnlyInline context readonlyInfo

/// An F# interface can only inherit another interface
let private inheritableBases (context: TransformContext) (references: GlueType list) =
    references
    |> List.filter (
        function
        | GlueType.TypeReference reference ->
            Conditionals.isInterfaceDeclaration context.State.Conditionals reference.FullName
        | _ -> false
    )

/// The name a type literal already generated in the scope is referenced by
let private (|RememberedTypeLiteral|_|) (context: TransformContext) (glueType: GlueType) =
    match glueType with
    | GlueType.TypeLiteral { Id = Some id } ->
        context.TypeLiteralsMemory.TryReference(id, context.Root)
    | _ -> None

/// Fable.Core stops at `U9`, a larger union is an erased union of its own with a case per type
let private erasedUnion
    (name: string)
    (typeParameters: FSharpTypeParameter list)
    (caseTypes: FSharpType list)
    : FSharpUnion
    =
    {
        Attributes = [ FSharpAttribute.RequireQualifiedAccess; FSharpAttribute.Erase ]
        Name = name
        Cases =
            caseTypes
            |> List.mapi (fun index caseType ->
                FSharpUnionCase.Field($"Case%i{index + 1}", caseType)
            )
        IsOptional = false
        TypeParameters = typeParameters
        Constants = []
    }

/// The reference to a type exposed in the scope under `name`
let private referenceToExposed
    (context: TransformContext)
    (name: string)
    (typeParameterNames: string list)
    =
    let fullName = context.ReferenceName name

    ({
        Name = fullName
        FullName = fullName
        ModulePath = []
        TypeArguments = typeParameterNames |> List.map FSharpType.TypeParameter
        Type = FSharpType.Discard
    }
    : FSharpTypeReference)
    |> FSharpType.TypeReference

let private transformUnionType (context: TransformContext) (cases: GlueType list) : FSharpType =
    let optionalTypes, others =
        cases
        |> List.partition (fun glueType ->
            match glueType with
            | GlueType.Primitive primitiveInfo ->
                match primitiveInfo with
                | GluePrimitive.Null
                | GluePrimitive.Undefined -> true
                | _ -> false
            | _ -> false
        )

    let isOptional = not optionalTypes.IsEmpty

    if isOptional && others.Length = 1 then
        FSharpType.Option(transformType context others.Head)
    else if others.IsEmpty then
        match optionalTypes with
        | [] -> FSharpType.Object
        | optionalType :: _ -> transformType context optionalType
    // Don't wrap in a U1 if there is only one case
    else if others.Length = 1 then
        transformType context others.Head
    else
        match tryOptimizeUnionType context context.CurrentScopeName others with
        | Some _ ->
            // Named like a type literal, so that the next anonymous type of the scope
            // doesn't take the same name
            let name = context.NewTypeName()

            // The cases can name the type parameters of the enclosing declaration
            let typeParameterNames = others |> List.collect typeParameterNames |> List.distinct

            let withTypeParameters (typ: FSharpType) =
                match typ with
                | FSharpType.Union unionInfo ->
                    FSharpType.Union
                        { unionInfo with
                            TypeParameters = declaredTypeParameters typeParameterNames
                        }
                | typ -> typ

            tryOptimizeUnionType context name others
            |> Option.map withTypeParameters
            |> Option.iter context.ExposeType

            referenceToExposed context name typeParameterNames

        | None ->
            let isLarge = others.Length > 9

            // The anonymous types of the cases are named under the union as written
            let name =
                if isLarge then
                    context.NewTypeName()
                else
                    $"U{others.Length}"

            // An erased union takes the scope itself
            let caseContext =
                if isLarge then
                    context
                else
                    sanitizeNameAndPushScope name context |> snd

            // `Intl.Locale | string` with both cases mapped to `obj` is one `obj`
            let caseTypes =
                others
                |> List.mapi (fun index caseType ->
                    let context = caseContext.PushScope $"Case%i{index + 1}"

                    transformType context caseType
                )
                |> List.distinct

            match caseTypes with
            | [ single ] -> single
            | _ when isLarge ->
                let typeParameterNames = others |> List.collect typeParameterNames |> List.distinct

                erasedUnion name (declaredTypeParameters typeParameterNames) caseTypes
                |> FSharpType.Union
                |> context.ExposeType

                let reference = referenceToExposed context name typeParameterNames

                if isOptional then
                    FSharpType.Option reference
                else
                    reference
            | _ ->
                let cases = caseTypes |> List.map FSharpUnionCase.Typed

                {
                    Attributes = []
                    Name = $"U{caseTypes.Length}"
                    Cases = cases
                    IsOptional = isOptional
                    TypeParameters = []
                    Constants = []
                }
                |> FSharpType.Union

/// A named function type follows the rule of the anonymous ones: with one parameter it is
/// an F# lambda, when F# can abbreviate it, with several a delegate
let private isLambdaSignature
    (parameters: GlueParameter list)
    (ownTypeParameters: FSharpTypeParameter list)
    (declaredTypeParameters: FSharpTypeParameter list)
    =
    // An abbreviation declares no constraint, a lambda no type parameter of its own
    let hasConstraint =
        declaredTypeParameters
        |> List.exists (
            function
            | FSharpTypeParameter.FSharpTypeParameter { Constraint = Some _ } -> true
            | _ -> false
        )

    parameters.Length <= 1
    && not (parameters |> List.exists _.IsSpread)
    && ownTypeParameters.IsEmpty
    && not hasConstraint

let private transformFunctionType
    (context: TransformContext)
    (functionTypeInfo: GlueFunctionType)
    : FSharpType
    =
    let paremeters =
        functionTypeInfo.Parameters
        // TypeScript allows to annotate the `this` parameter but it is not actually part
        // of the function signature that the user will call.
        |> List.filter (fun parameter -> parameter.Name <> "this")

    match paremeters with
    // If there is no parameter or only one parameter, we want to generate a lambda
    // We consider that using a delegate is not necessary in this case
    // it adds an unnecessary complexity for the user
    | []
    | _ :: [] ->
        // `<RG = Default>(req: Req<RG>) => void`: an F# function can't declare the type
        // parameters of the function, they are their default
        let ownDefaults = ownTypeParameterDefaults functionTypeInfo

        let paremeters =
            paremeters |> List.map (GlueSubstitution.substituteParameter ownDefaults)

        ({
            Parameters =
                paremeters |> List.map (transformParameter context) |> requiredBeforeParamArray
            ReturnType =
                transformCallbackReturnType
                    context
                    (GlueSubstitution.substitute ownDefaults functionTypeInfo.Type)
        }
        : FSharpFunctionType)
        |> FSharpType.Function

    // More than 1 parameter, we generate a delegate
    | _ ->
        let typParameters =
            // An inherited signature names its own type parameters without listing them
            let declared = functionTypeInfo.TypeParameters |> List.map _.Name |> Set.ofList

            let inherited =
                functionTypeInfo.OwnTypeParameterNames
                |> List.filter (fun name -> not (declared.Contains name))
                |> List.map unconstrained

            functionTypeInfo.TypeParameters @ inherited
            // TypeParameters are coming from the parent scope
            // so we need to filter them to only keep the ones that are used
            // See file://./../../tests/specs/references/functionType/interface/generics/moreGenericsOnParentThanNeeded.d.ts
            |> List.filter (fun typeParameter ->
                let usedInParameters =
                    paremeters
                    |> List.exists (fun parameter ->
                        mentionsTypeParameter typeParameter.Name parameter.Type
                    )

                let usedInReturnType =
                    mentionsTypeParameter typeParameter.Name functionTypeInfo.Type

                usedInParameters || usedInReturnType
            )
            // The default of an enclosing type parameter belongs to its declaration
            |> List.map (fun typeParameter ->
                if List.contains typeParameter.Name functionTypeInfo.OwnTypeParameterNames then
                    typeParameter
                else
                    { typeParameter with Default = None }
            )
            |> transformTypeParameters context

        let name = context.NewTypeName()

        ({
            XmlDoc = []
            Name = name
            TypeParameters = typParameters.TypeParameters
            Parameters =
                paremeters
                |> List.map (
                    transformParameter context
                    >> TypeParameter.mapFsharpParameter typParameters.SealedTypes
                )
                |> requiredBeforeParamArray
            // The anonymous type a delegate returns is named after the delegate otherwise
            ReturnType =
                transformCallbackReturnType (context.PushScope "ReturnType") functionTypeInfo.Type
                |> TypeParameter.mapFSharpType typParameters.SealedTypes
        }
        : FSharpDelegate)
        |> FSharpType.Delegate
        |> context.ExposeType

        // `mount: <Id>(id: Id) => ...`: a property or a type argument can't be generic,
        // the function's own type parameters are their default, else `obj`
        let ownDefaults =
            ownTypeParameterDefaults functionTypeInfo
            |> Map.map (fun _ glueType -> transformType context glueType)

        ({
            Attributes = []
            Name = context.ReferenceName name
            TypeParameters =
                typParameters.TypeParameters
                |> List.map (fun typeParameter ->
                    match typeParameter with
                    | FSharpTypeParameter.FSharpTypeParameter info when
                        ownDefaults.ContainsKey info.Name
                        ->
                        FSharpTypeParameter.FSharpType ownDefaults.[info.Name]
                    | _ -> typeParameter
                )
            XmlDoc = []
            Type = FSharpType.Discard
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

let private transformTypeLiteral
    (context: TransformContext)
    (typeLiteralInfo: GlueTypeLiteral)
    : FSharpType
    =
    let isParamObjectCandidate = isDataObject typeLiteralInfo.Members

    let typeParameterNames =
        typeLiteralInfo.Members
        |> List.collect memberTypeParameterNames
        |> List.distinct

    let name = context.NewTypeName()

    let transformedMembers =
        TransformMembers.toFSharpMember context typeLiteralInfo.Members

    if
        isParamObjectCandidate
        && membersNameUndeclaredTypeParameters typeParameterNames transformedMembers
    then
        transformParamObjectClass context name [] typeLiteralInfo.Members
        |> FSharpType.Class
        |> context.ExposeType
    else
        let creates =
            if isParamObjectCandidate then
                let returnType =
                    ({
                        Name = name
                        TypeParameters = typeArguments typeParameterNames
                    }
                    : FSharpMapped)
                    |> FSharpType.Mapped

                paramObjectCreateMembers context returnType typeLiteralInfo.Members
            else
                []

        let inheritance =
            TypeLiteral.tryFindIterableType context typeLiteralInfo.Members |> Option.toList

        { anonymousInterface
              context
              name
              typeParameterNames
              (transformedMembers @ creates)
              inheritance with
            OriginalName = ""
        }
        |> FSharpType.Interface
        |> context.ExposeType

    let name = context.ReferenceName name

    // A generic anonymous type is not the same type at every use site
    if typeParameterNames.IsEmpty then
        typeLiteralInfo.Id
        |> Option.iter (fun id -> context.TypeLiteralsMemory.Remember(id, context.Root, name))

    ({
        Name = name
        FullName = context.FullName
        ModulePath = []
        TypeArguments =
            typeParameterNames
            |> List.map (fun name ->
                ({
                    Name = $"'%s{name}"
                    TypeParameters = []
                }
                : FSharpMapped)
                |> FSharpType.Mapped
            )
        Type = FSharpType.Discard
    }
    : FSharpTypeReference)
    |> FSharpType.TypeReference

let private transformIntersectionOfReferences
    (context: TransformContext)
    (references: GlueType list)
    (members: GlueMember list)
    : FSharpType
    =
    // An interface can only inherit another interface, a reference to anything else
    // would not compile
    let bases = inheritableBases context references

    if bases.IsEmpty then
        FSharpType.Object
    else
        let typeParameterNames =
            references |> List.collect typeParameterNames |> List.distinct

        let name = context.NewTypeName()

        anonymousInterface
            context
            name
            typeParameterNames
            (TransformMembers.toFSharpMember context members)
            (bases |> List.map (transformType context))
        |> FSharpType.Interface
        |> context.ExposeType

        mappedReference context name typeParameterNames

let private transformIntersectionType
    (context: TransformContext)
    (members: GlueMember list)
    : FSharpType
    =
    if members.IsEmpty then
        FSharpType.Object
    else
        // The type parameters of the enclosing declaration used by the members
        let typeParameterNames =
            members |> List.collect memberTypeParameterNames |> List.distinct

        let name = context.NewTypeName()

        let creates =
            if isDataObject members then
                let returnType =
                    ({
                        Name = name
                        TypeParameters = typeArguments typeParameterNames
                    }
                    : FSharpMapped)
                    |> FSharpType.Mapped

                paramObjectCreateMembers context returnType members
            else
                []

        anonymousInterface
            context
            name
            typeParameterNames
            (TransformMembers.toFSharpMember context members @ creates)
            []
        |> FSharpType.Interface
        |> context.ExposeType

        mappedReference context name typeParameterNames

let private transformMappedType
    (context: TransformContext)
    (mappedType: GlueMappedType)
    : FSharpType
    =
    let members =
        transformMappedTypeMembers context mappedType
        |> TransformMembers.toFSharpMember context

    if members.IsEmpty then
        FSharpType.Object
    else
        let name = context.NewTypeName()

        // `{ [key in KEY]?: T }` inside `createHashMap<T, KEY>` is generic
        let freeTypeParameterNames =
            mappedType.Type
            |> Option.map typeParameterNames
            |> Option.defaultValue []
            |> List.filter (fun typeParameterName ->
                typeParameterName <> mappedType.TypeParameter.Name
            )
            |> List.distinct

        anonymousInterface context name freeTypeParameterNames members []
        |> FSharpType.Interface
        |> context.ExposeType

        mappedReference context name freeTypeParameterNames

let rec private transformType (context: TransformContext) (glueType: GlueType) : FSharpType =
    match glueType with
    | GlueType.Unknown -> FSharpType.Object

    | GlueType.ConstructorType constructSignature ->
        ({
            Members = [ GlueMember.ConstructSignature constructSignature ]
            Id = None
        }
        : GlueTypeLiteral)
        |> GlueType.TypeLiteral
        |> transformType context

    | GlueType.Primitive primitiveInfo -> transformPrimitive primitiveInfo |> FSharpType.Primitive

    | GlueType.TemplateLiteral -> FSharpType.Primitive FSharpPrimitive.String

    | GlueType.OptionalType glueType -> transformType context glueType |> FSharpType.Option

    | GlueType.ThisType thisTypeInfo ->
        ({
            // The declaration sanitizes its name, `$ZodRegistry` is `_DOLLAR_ZodRegistry`
            Name = Naming.sanitizeTypeName thisTypeInfo.Name
            TypeParameters =
                thisTypeInfo.TypeParameters |> List.map _.Name |> declaredTypeParameters
        }
        : FSharpThisType)
        |> FSharpType.ThisType

    | GlueType.TupleType glueTypes -> transformTupleType context glueTypes

    | GlueType.ReadOnly glueType -> transformReadOnlyModifier context glueType

    | GlueType.Union(GlueTypeUnion cases) -> transformUnionType context cases

    // `Key<K, T>` standing for a conditional type is the unresolved type itself
    | GlueType.TypeReference typeReference when
        Conditionals.isConditionalAlias context.State.Conditionals typeReference.FullName
        ->
        FSharpType.Object

    // `FlatArray<A, D>` has no counterpart, the type arguments go with it
    | GlueType.TypeReference typeReference when
        mapTypeNameToFableCoreAwareName context typeReference = "obj"
        ->
        FSharpType.Object

    // `Readonly<A>` with `A` unknown is `A`
    | GlueType.TypeReference {
                                 Name = "Readonly"
                                 IsStandardLibrary = true
                                 TypeArguments = [ argument ]
                             } -> transformType context argument

    | GlueType.TypeReference typeReference ->
        ({
            Name = mapTypeNameToFableCoreAwareName context typeReference
            FullName = typeReference.FullName
            ModulePath = typeReference.ModulePath
            TypeArguments =
                // The typed arrays of Fable.Core are not generic (`Uint8Array<TArrayBuffer>`)
                if
                    typeReference.IsStandardLibrary && typedArrayNames.Contains typeReference.Name
                then
                    []
                // `IterableIterator<T, TReturn, TNext>` is an `Iterable<T>`
                elif
                    typeReference.IsStandardLibrary && iteratorNames.Contains typeReference.Name
                then
                    typeReference.TypeArguments
                    |> List.truncate 1
                    |> List.map (transformType context)
                else
                    typeReference.TypeArguments |> List.map (transformType context)
            Type = FSharpType.Discard
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.Array glueType ->
        transformType (context.PushScope "Item") glueType |> FSharpType.ResizeArray

    | GlueType.ClassDeclaration classDeclaration ->
        ({
            Name = classDeclaration.Name
            FullName = classDeclaration.Name
            ModulePath = []
            TypeArguments = []
            Type = FSharpType.Discard // TODO: Retrieve the type
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.TypeParameter name -> FSharpType.TypeParameter name

    | GlueType.FunctionType functionTypeInfo -> transformFunctionType context functionTypeInfo

    | GlueType.Interface interfaceInfo ->
        match tryTransformCallableInterface context interfaceInfo with
        | Some delegateType -> delegateType
        | None -> FSharpType.Interface(transformInterface context interfaceInfo)

    // `{}` is any non-null value
    | GlueType.TypeLiteral { Members = [] } -> FSharpType.Object

    // The same declaration reached twice, `ReturnType<typeof f>` and the return of `f`
    | RememberedTypeLiteral context name ->
        ({
            Name = name
            FullName = context.FullName
            ModulePath = []
            TypeArguments = []
            Type = FSharpType.Discard
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.TypeLiteral typeLiteralInfo -> transformTypeLiteral context typeLiteralInfo

    | GlueType.ExportDefault glueType -> transformType context glueType

    | GlueType.Variable info -> transformType context info.Type

    | GlueType.KeyOf innerGlueType ->
        match TypeAliasDeclaration.tryTransformKeyOf context.CurrentScopeName innerGlueType with
        | Some typ ->
            context.ExposeType typ

            ({
                Name = context.FullName
                FullName = context.FullName
                ModulePath = []
                TypeArguments = []
                Type = FSharpType.Discard
            }
            : FSharpTypeReference)
            |> FSharpType.TypeReference

        | None -> FSharpType.Object

    | GlueType.NamedTupleType namedTupleType -> transformType context namedTupleType.Type

    | GlueType.Discard -> FSharpType.Object

    | GlueType.Literal glueLiteral ->
        match glueLiteral with
        | GlueLiteral.String _ -> FSharpType.Primitive FSharpPrimitive.String
        | GlueLiteral.Int _ -> FSharpType.Primitive FSharpPrimitive.Int
        | GlueLiteral.Float _ -> FSharpType.Primitive FSharpPrimitive.Float
        | GlueLiteral.Bool _ -> FSharpType.Primitive FSharpPrimitive.Bool
        | GlueLiteral.Null -> FSharpType.Primitive FSharpPrimitive.Null

    // `typeof f` where `f` is generic: F# has no generic function values
    | GlueType.FunctionDeclaration functionDeclaration when
        not functionDeclaration.TypeParameters.IsEmpty
        ->
        FSharpType.Object

    | GlueType.FunctionDeclaration functionDeclaration ->
        ({
            Parameters =
                functionDeclaration.Parameters
                |> List.map (transformParameter context)
                |> requiredBeforeParamArray
            ReturnType = transformType context functionDeclaration.Type
        }
        : FSharpFunctionType)
        |> FSharpType.Function

    | GlueType.IntersectionOfReferences(references, members) ->
        transformIntersectionOfReferences context references members

    | GlueType.IntersectionType members -> transformIntersectionType context members

    | GlueType.UtilityType utilityType -> UtilityType.transform context utilityType

    | GlueType.TypeAliasDeclaration typeAliasDeclaration ->
        ({
            Name = typeAliasDeclaration.Name
            TypeParameters = []
        }
        : FSharpMapped)
        |> FSharpType.Mapped

    | GlueType.MappedType mappedType -> transformMappedType context mappedType

    // `T[K]` outside of a mapped type has no equivalent in F#
    | GlueType.IndexedAccessType _ -> FSharpType.Object

    // `typeof import('./errors').default` of a namespace, the namespace object has no F# type
    | GlueType.ModuleDeclaration _ -> FSharpType.Object

    // A conditional type the declaration's defaults can't resolve
    | GlueType.ConditionalType _ -> FSharpType.Object

    | GlueType.FileModule _
    | GlueType.ReExport _
    | GlueType.Enum _ ->
        context.AddError $"Could not transform type: %A{glueType}"
        FSharpType.Discard

let private transformParameter
    (context: TransformContext)
    (parameter: GlueParameter)
    : FSharpParameter
    =
    let name, context = sanitizeNameAndPushScope parameter.Name context

    // `readonly string[]` given to a function accepts any array, a `ResizeArray` is the F# one
    let rec asArray (glueType: GlueType) =
        match glueType with
        | GlueType.ReadOnly(GlueType.Array elementType) -> GlueType.Array elementType
        | GlueType.TypeReference {
                                     Name = "ReadonlyArray"
                                     IsStandardLibrary = true
                                     TypeArguments = [ elementType ]
                                 } -> GlueType.Array elementType
        | GlueType.Union(GlueTypeUnion cases) ->
            GlueType.Union(GlueTypeUnion(cases |> List.map asArray))
        | GlueType.OptionalType glueType -> GlueType.OptionalType(asArray glueType)
        | glueType -> glueType

    let parameter =
        { parameter with
            Type = asArray parameter.Type
        }

    let typ =
        let computedType =
            unwrapOptionIfAlreadyOptional context parameter.Type parameter.IsOptional

        // In TypeScript, if an argument is marked as spread, users is forced to
        // use an array. We want to remove the default transformation for that
        // array and use the underlying type instead
        // By default, an array is transformed to ResizeArray in F#
        if parameter.IsSpread then
            match computedType with
            | FSharpType.ResizeArray underlyingType -> underlyingType
            | FSharpType.TypeReference {
                                           Name = "ResizeArray"
                                           TypeArguments = [ underlyingType ]
                                       } -> underlyingType
            | _ -> computedType
        else
            computedType

    {
        Attributes =
            [
                if parameter.IsSpread then
                    FSharpAttribute.ParamArray
            ]
        Name = name
        IsOptional = parameter.IsOptional
        Type = typ
        OriginalGlueMember = None
    }

let private transformAccessor (accessor: GlueAccessor) : FSharpAccessor =
    match accessor with
    | GlueAccessor.ReadOnly -> FSharpAccessor.ReadOnly
    | GlueAccessor.WriteOnly -> FSharpAccessor.WriteOnly
    | GlueAccessor.ReadWrite -> FSharpAccessor.ReadWrite

/// A method set as a value is a function and not the type it returns, as a param object field
/// or as an optional member
let private methodGlueFunctionType
    (documentation: GlueComment list)
    (typeParameters: GlueTypeParameter list)
    (parameters: GlueParameter list)
    (returnType: GlueType)
    : GlueType
    =
    ({
        Documentation = documentation
        Type = returnType
        TypeParameters = typeParameters
        OwnTypeParameterNames = typeParameters |> List.map _.Name
        Parameters = parameters
    }
    : GlueFunctionType)
    |> GlueType.FunctionType

let private methodFunctionType
    (context: TransformContext)
    (documentation: GlueComment list)
    (typeParameters: GlueTypeParameter list)
    (parameters: GlueParameter list)
    (returnType: GlueType)
    : FSharpType
    =
    methodGlueFunctionType documentation typeParameters parameters returnType
    |> transformType context

module private TransformMembers =

    // A reference constraint of a member is read for the ParamObject analysis, the transform
    // never sealed a member on one
    let private withoutReferenceConstraints (typeParameters: GlueTypeParameter list) =
        typeParameters
        |> List.map (fun typeParameter ->
            match typeParameter.Constraint with
            | Some(GlueType.KeyOf _)
            | None -> typeParameter
            | Some _ -> { typeParameter with Constraint = None }
        )

    // A computed property name such as `[Symbol.toStringTag]` has no F# equivalent
    /// The disposal protocol, the other computed names are iteration handled by the
    /// `Iterable<T>` inheritance and formatting hooks a binding has no use for
    let private disposalMemberName (name: string) =
        match name with
        | "[Symbol.dispose]" -> Some "dispose"
        | "[Symbol.asyncDispose]" -> Some "asyncDispose"
        | _ -> None

    let private hasComputedName (glueMember: GlueMember) =
        match glueMember with
        | GlueMember.Property { Name = name }
        | GlueMember.Method { Name = name }
        | GlueMember.MethodSignature { Name = name }
        | GlueMember.GetAccessor { Name = name }
        | GlueMember.SetAccessor { Name = name } ->
            name.StartsWith "[" && (disposalMemberName name).IsNone
        | GlueMember.CallSignature _
        | GlueMember.ConstructSignature _
        | GlueMember.IndexSignature _ -> false

    let withoutComputedNames (members: GlueMember list) =
        members |> List.filter (hasComputedName >> not)

    // A static member prints an inline import of its class from the package
    let private withoutStaticMembers (members: GlueMember list) =
        members
        |> List.filter (
            function
            | GlueMember.Method { IsStatic = isStatic }
            | GlueMember.Property { IsStatic = isStatic }
            | GlueMember.GetAccessor { IsStatic = isStatic }
            | GlueMember.SetAccessor { IsStatic = isStatic } -> not isStatic
            | GlueMember.CallSignature _
            | GlueMember.ConstructSignature _
            | GlueMember.IndexSignature _
            | GlueMember.MethodSignature _ -> true
        )

    /// Declared next to the overload made of the defaults, so F# tells the two apart
    let private defaultsOf
        (parameters: GlueParameter list)
        (typeParameters: GlueTypeParameter list)
        =
        // `eachDay<Options = undefined>(options?: Options)`: `options?: obj` is no overload
        let isParameterType (name: string) =
            parameters
            |> List.exists (fun parameter ->
                match parameter.Type with
                | GlueType.TypeParameter parameterName
                | GlueType.OptionalType(GlueType.TypeParameter parameterName) ->
                    parameterName = name
                | _ -> false
            )

        let defaults =
            typeParameters
            |> List.choose (fun typeParameter ->
                match typeParameter.Default with
                | Some(GlueType.Primitive GluePrimitive.Undefined) when
                    isParameterType typeParameter.Name
                    ->
                    None
                | Some(GlueType.KeyOf _)
                | Some(GlueType.IndexedAccessType _)
                | None -> None
                | Some default_ -> Some(typeParameter.Name, default_)
            )
            |> Map.ofList

        // `add<DateType, ResultDate = DateType>`: F# can't choose between two generic overloads
        if
            typeParameters
            |> List.forall (fun typeParameter -> defaults.ContainsKey typeParameter.Name)
        then
            // `<T = any[], R = T>`: `R` is `any[]`
            defaults
            |> Map.map (fun _ default_ -> GlueSubstitution.substitute defaults default_)
        else
            Map.empty

    // `getModel<T = unknown>(): Model<T>`: F# rejects two parameterless methods differing
    // by their generic arity only
    let private usesDefaulted
        (substitutions: Map<string, GlueType>)
        (parameters: GlueParameter list)
        (typ: GlueType)
        =
        substitutions
        |> Map.exists (fun name _ ->
            (not parameters.IsEmpty && mentionsTypeParameter name typ)
            || parameters
               |> List.exists (fun parameter -> mentionsTypeParameter name parameter.Type)
        )

    /// The signature with the defaulted type parameters replaced by their default, `None` when
    /// the defaults make no other signature
    let private tryDefaultedSignature
        (typeParameters: GlueTypeParameter list)
        (parameters: GlueParameter list)
        (returnType: GlueType)
        =
        let substitutions = defaultsOf parameters typeParameters

        if usesDefaulted substitutions parameters returnType then
            Some(
                typeParameters |> List.filter _.Default.IsNone,
                parameters |> List.map (GlueSubstitution.substituteParameter substitutions),
                GlueSubstitution.substitute substitutions returnType
            )
        else
            None

    /// `{ [key: string]: any }` takes any object
    let private isAnyDictionary (glueType: GlueType) =
        match glueType with
        | GlueType.TypeLiteral { Members = members } when not members.IsEmpty ->
            members
            |> List.forall (
                function
                | GlueMember.IndexSignature {
                                                Type = GlueType.Primitive GluePrimitive.Any
                                            }
                | GlueMember.IndexSignature { Type = GlueType.Unknown } -> true
                | _ -> false
            )
        | _ -> false

    /// F# cannot pick between `create<'T, 'Body>` and `create<'Body>`: a type parameter told by the
    /// result alone gets no overload
    let private tryDictionarySignature
        (typeParameters: GlueTypeParameter list)
        (parameters: GlueParameter list)
        =
        let told =
            parameters
            |> List.collect (fun parameter ->
                GlueSubstitution.mentionedTypeParameters parameter.Type
            )
            |> Set.ofList

        let hasDictionary =
            parameters |> List.exists (fun parameter -> isAnyDictionary parameter.Type)

        let allTold =
            typeParameters
            |> List.forall (fun typeParameter -> told.Contains typeParameter.Name)

        if hasDictionary && allTold then
            let taken = HashSet<string>(typeParameters |> List.map _.Name)

            let fresh (parameter: GlueParameter) =
                let name = parameter.Name |> String.capitalizeFirstLetter |> Naming.sanitizeTypeName

                let rec pick (candidate: string) (index: int) =
                    if taken.Add candidate then
                        candidate
                    else
                        pick (name + string index) (index + 1)

                pick name 1

            let added = ResizeArray<GlueTypeParameter>()

            let parameters =
                parameters
                |> List.map (fun parameter ->
                    if isAnyDictionary parameter.Type then
                        let name = fresh parameter

                        added.Add
                            {
                                Name = name
                                Constraint = None
                                Default = None
                            }

                        { parameter with
                            Type = GlueType.TypeParameter name
                        }
                    else
                        parameter
                )

            Some(typeParameters @ List.ofSeq added, parameters)
        else
            None

    /// `create(bodyParams: { [key: string]: any })` also gets `create<'BodyParams>(bodyParams: 'BodyParams)`
    let private withDictionaryParameterOverloads (members: GlueMember list) =
        members
        |> List.collect (fun glueMember ->
            match glueMember with
            | GlueMember.MethodSignature info ->
                match tryDictionarySignature info.TypeParameters info.Parameters with
                | Some(typeParameters, parameters) ->
                    [
                        glueMember
                        GlueMember.MethodSignature
                            { info with
                                TypeParameters = typeParameters
                                Parameters = parameters
                            }
                    ]
                | None -> [ glueMember ]
            | GlueMember.Method info ->
                match tryDictionarySignature info.TypeParameters info.Parameters with
                | Some(typeParameters, parameters) ->
                    [
                        glueMember
                        GlueMember.Method
                            { info with
                                TypeParameters = typeParameters
                                Parameters = parameters
                            }
                    ]
                | None -> [ glueMember ]
            | _ -> [ glueMember ]
        )

    let withDictionaryParameterOverloadsOfFunction (info: GlueFunctionDeclaration) =
        match tryDictionarySignature info.TypeParameters info.Parameters with
        | Some(typeParameters, parameters) ->
            [
                info
                { info with
                    TypeParameters = typeParameters
                    Parameters = parameters
                }
            ]
        | None -> [ info ]

    /// `layerGroup<P = any>(layers: Layer[]): LayerGroup<P>` also gets
    /// `layerGroup(layers: Layer[]): LayerGroup<any>`
    let withDefaultedTypeParameterOverloadsOfFunction (info: GlueFunctionDeclaration) =
        match tryDefaultedSignature info.TypeParameters info.Parameters info.Type with
        | Some(typeParameters, parameters, returnType) ->
            [
                info
                { info with
                    TypeParameters = typeParameters
                    Parameters = parameters
                    Type = returnType
                }
            ]
        | None -> [ info ]

    /// `querySelector<E = Element>(s: string): E` also gets `querySelector(s: string): Element`,
    /// the F# call without a type argument resolves to it
    let private withDefaultedTypeParameterOverloads (members: GlueMember list) =
        members
        |> List.collect (fun glueMember ->
            match glueMember with
            | GlueMember.MethodSignature info ->
                match tryDefaultedSignature info.TypeParameters info.Parameters info.Type with
                | Some(typeParameters, parameters, returnType) ->
                    [
                        glueMember
                        GlueMember.MethodSignature
                            { info with
                                TypeParameters = typeParameters
                                Parameters = parameters
                                Type = returnType
                            }
                    ]
                | None -> [ glueMember ]

            | GlueMember.Method info ->
                match tryDefaultedSignature info.TypeParameters info.Parameters info.Type with
                | Some(typeParameters, parameters, returnType) ->
                    [
                        glueMember
                        GlueMember.Method
                            { info with
                                TypeParameters = typeParameters
                                Parameters = parameters
                                Type = returnType
                            }
                    ]
                | None -> [ glueMember ]

            | _ -> [ glueMember ]
        )

    /// <summary>
    /// Detect methods whose first parameter is a string literal (the
    /// event-emitter pattern, e.g. <c>on(event: 'console', listener)</c>) and
    /// turn them into a dedicated member (e.g. <c>on_console</c>) where the
    /// literal is baked into an <c>Emit</c> so the caller doesn't pass it.
    /// </summary>
    /// <returns>
    /// <c>Some (emitText, specializedName, remainingParameters)</c> when the
    /// pattern matches, otherwise <c>None</c>.
    /// </returns>
    let private trySpecializeStringLiteralMethod
        (methodName: string)
        (parameters: GlueParameter list)
        =
        match parameters with
        | firstParameter :: remainingParameters ->
            match firstParameter.Type with
            | GlueType.Literal(GlueLiteral.String literalValue) ->
                let specializedName = $"{methodName}_{literalValue}"

                let emitText =
                    if remainingParameters.IsEmpty then
                        $"Emit(\"$0.{methodName}('{literalValue}')\")"
                    else
                        $"Emit(\"$0.{methodName}('{literalValue}',$1...)\")"

                Some(emitText, specializedName, remainingParameters)
            | _ -> None
        | [] -> None

    /// Whether the F# type names the type parameter
    let rec private fsharpTypeMentions (name: string) (typ: FSharpType) : bool =
        let mentions = fsharpTypeMentions name

        let typeParameterMentions (typeParameter: FSharpTypeParameter) =
            match typeParameter with
            | FSharpTypeParameter.FSharpType typ -> mentions typ
            | FSharpTypeParameter.FSharpTypeParameter info -> info.Name = name

        match typ with
        | FSharpType.TypeParameter typeParameter -> typeParameter = name
        | FSharpType.TypeReference typeReference ->
            typeReference.TypeArguments |> List.exists mentions
        | FSharpType.Option inner
        | FSharpType.ResizeArray inner -> mentions inner
        | FSharpType.JSApi(FSharpJSApi.ReadonlyArray inner) -> mentions inner
        | FSharpType.Union unionInfo ->
            unionInfo.Cases
            |> List.exists (
                function
                | FSharpUnionCase.Typed typ
                | FSharpUnionCase.Field(_, typ) -> mentions typ
                | FSharpUnionCase.NamedFields(_, fields) -> fields |> List.exists (snd >> mentions)
                | FSharpUnionCase.Named _ -> false
            )
        | FSharpType.Tuple elements -> elements |> List.exists mentions
        | FSharpType.Function functionType ->
            mentions functionType.ReturnType
            || functionType.Parameters
               |> List.exists (fun parameter -> mentions parameter.Type)
        // A type argument that is a type parameter is a `Mapped` named `'T`
        | FSharpType.Mapped mapped ->
            mapped.Name = "'" + name
            || mapped.TypeParameters |> List.exists typeParameterMentions
        // A delegate is referred to through its alias, with the type parameters it takes
        | FSharpType.TypeAlias alias -> alias.TypeParameters |> List.exists typeParameterMentions
        | _ -> false

    /// `on<K>(event: Key<K, T>, listener: Listener<K, T>)` resolved to `on(event: obj, listener)`
    /// has nothing left for `'K`, a caller could never give it
    let private withoutUnmentionedTypeParameters (members: FSharpMember list) =
        let keep
            (typeParameters: FSharpTypeParameter list)
            (parameters: FSharpParameter list)
            (returnType: FSharpType)
            =
            typeParameters
            |> List.filter (
                function
                | FSharpTypeParameter.FSharpTypeParameter info ->
                    fsharpTypeMentions info.Name returnType
                    || parameters
                       |> List.exists (fun parameter -> fsharpTypeMentions info.Name parameter.Type)
                | FSharpTypeParameter.FSharpType _ -> true
            )

        members
        |> List.map (
            function
            | FSharpMember.Method info ->
                FSharpMember.Method
                    { info with
                        TypeParameters = keep info.TypeParameters info.Parameters info.Type
                    }
            | FSharpMember.StaticMember info ->
                FSharpMember.StaticMember
                    { info with
                        TypeParameters = keep info.TypeParameters info.Parameters info.Type
                    }
            | fsharpMember -> fsharpMember
        )

    /// A getter and a setter of the same name are one read-write property, `declared` are the
    /// members as declared
    let private mergeAccessors (declared: GlueMember list) (members: GlueMember list) =
        let hasAccessor (name: string) (isSetter: bool) =
            declared
            |> List.exists (
                function
                | GlueMember.SetAccessor info -> isSetter && info.Name = name
                | GlueMember.GetAccessor info -> not isSetter && info.Name = name
                | _ -> false
            )

        members
        |> List.choose (
            function
            | GlueMember.GetAccessor info when hasAccessor info.Name true ->
                {
                    Name = info.Name
                    Documentation = info.Documentation
                    Type = info.Type
                    IsOptional = false
                    IsStatic = info.IsStatic
                    Accessor = GlueAccessor.ReadWrite
                    IsPrivate = info.IsPrivate
                }
                |> GlueMember.Property
                |> Some
            | GlueMember.SetAccessor info when hasAccessor info.Name false -> None
            | glueMember -> Some glueMember
        )

    /// A method of the object, a static one through an inline import of the class
    let private methodMember (context: TransformContext) (methodInfo: GlueMethod) =
        let xmlDocInfo = transformComment methodInfo.Documentation

        // Only an instance method is specialized on a literal, the `$0` of the `Emit` is the instance
        let methodName, parameters, emitAttributes =
            if methodInfo.IsStatic then
                methodInfo.Name, methodInfo.Parameters, []
            else
                match trySpecializeStringLiteralMethod methodInfo.Name methodInfo.Parameters with
                | Some(emitText, specializedName, remainingParameters) ->
                    specializedName, remainingParameters, [ FSharpAttribute.Text emitText ]
                | None -> methodInfo.Name, methodInfo.Parameters, []

        let name, context =
            sanitizeMemberNameAndPushScope methodInfo.IsStatic methodName context

        if methodInfo.IsStatic then
            {
                Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
                Name = name
                OriginalName = methodInfo.Name
                Parameters =
                    parameters |> List.map (transformParameter context) |> requiredBeforeParamArray
                Type = transformType context methodInfo.Type
                TypeParameters = []
                IsOptional = methodInfo.IsOptional
                Accessor = None
                Accessibility = FSharpAccessibility.Public
                XmlDoc = xmlDocInfo.XmlDoc
            }
            |> FSharpMember.StaticMember
        else
            // `<E extends SVGElement | HTMLElement>` is sealed, `'E` is the union in the signature
            let typeParameters =
                transformTypeParameters
                    context
                    (withoutReferenceConstraints methodInfo.TypeParameters)

            {
                Attributes = [ yield! xmlDocInfo.ObsoleteAttributes; yield! emitAttributes ]
                Name = name
                OriginalName = methodInfo.Name
                Parameters =
                    parameters
                    |> List.map (
                        transformParameter context
                        >> TypeParameter.mapFsharpParameter typeParameters.SealedTypes
                    )
                    |> requiredBeforeParamArray
                Type =
                    transformType context methodInfo.Type
                    |> TypeParameter.mapFSharpType typeParameters.SealedTypes
                TypeParameters = typeParameters.TypeParameters
                IsOptional = methodInfo.IsOptional
                IsStatic = methodInfo.IsStatic
                Accessor = None
                Accessibility = FSharpAccessibility.Public
                XmlDoc = xmlDocInfo.XmlDoc
                Body = FSharpMemberInfoBody.NativeOnly
            }
            |> FSharpMember.Method

    /// `(x: number): string` is `Invoke`
    let private callSignatureMember (context: TransformContext) (info: GlueCallSignature) =
        let name, context = sanitizeNameAndPushScope "Invoke" context

        let typeParameters =
            transformTypeParameters context (withoutReferenceConstraints info.TypeParameters)

        {
            Attributes = [ FSharpAttribute.EmitSelfInvoke ]
            Name = name
            OriginalName = "Invoke"
            Parameters =
                info.Parameters
                |> List.map (
                    transformParameter context
                    >> TypeParameter.mapFsharpParameter typeParameters.SealedTypes
                )
                |> requiredBeforeParamArray
            Type =
                transformType context info.Type
                |> TypeParameter.mapFSharpType typeParameters.SealedTypes
            TypeParameters = typeParameters.TypeParameters
            IsOptional = false
            IsStatic = false
            Accessor = None
            Accessibility = FSharpAccessibility.Public
            XmlDoc = []
            Body = FSharpMemberInfoBody.NativeOnly
        }
        |> FSharpMember.Method

    /// `None` for a private instance property, an F# interface has none
    let private propertyMember (context: TransformContext) (info: GlueProperty) =
        let name, context = sanitizeMemberNameAndPushScope info.IsStatic info.Name context

        let xmlDocInfo = transformComment info.Documentation

        if info.IsPrivate && not info.IsStatic then
            None
        else
            {
                Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
                Name = name
                OriginalName = info.Name
                Parameters = []
                Type =
                    // A `void` brand property has no valid setter type in F#
                    match unwrapOptionIfAlreadyOptional context info.Type info.IsOptional with
                    | FSharpType.Primitive FSharpPrimitive.Unit -> FSharpType.Object
                    | FSharpType.TypeReference typeReference when
                        isUnitAlias context.TypeMemory typeReference.FullName
                        ->
                        FSharpType.Object
                    | typ -> typ
                TypeParameters = []
                IsOptional = info.IsOptional
                IsStatic = info.IsStatic
                Accessor = transformAccessor info.Accessor |> Some
                Accessibility =
                    if info.IsPrivate then
                        FSharpAccessibility.Private
                    else
                        FSharpAccessibility.Public
                XmlDoc = xmlDocInfo.XmlDoc
                Body = FSharpMemberInfoBody.JavaScriptStaticProperty
            }
            |> FSharpMember.Property
            |> Some

    /// A getter or a setter left alone is a property with the one accessor
    let private accessorMember
        (context: TransformContext)
        (isStatic: bool)
        (originalName: string)
        (documentation: GlueComment list)
        (typ: GlueType)
        (accessor: FSharpAccessor)
        =
        let name, context = sanitizeMemberNameAndPushScope isStatic originalName context
        let xmlDocInfo = transformComment documentation

        {
            Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
            Name = name
            OriginalName = originalName
            Parameters = []
            Type = transformType context typ
            TypeParameters = []
            IsOptional = false
            IsStatic = isStatic
            Accessor = Some accessor
            Accessibility = FSharpAccessibility.Public
            XmlDoc = xmlDocInfo.XmlDoc
            Body = FSharpMemberInfoBody.NativeOnly
        }
        |> FSharpMember.Property

    /// `[key: string]: T` is the `Item` indexer
    let private indexSignatureMember (context: TransformContext) (info: GlueIndexSignature) =
        let name, context = sanitizeNameAndPushScope "Item" context

        // `[index: number]: T` is indexed with an `int`
        let parameters =
            info.Parameters
            |> List.map (fun parameter ->
                match parameter.Type with
                | GlueType.Primitive GluePrimitive.Number ->
                    { parameter with
                        Type = GlueType.Primitive GluePrimitive.Int
                    }
                | _ -> parameter
            )

        {
            Attributes = [ FSharpAttribute.EmitIndexer ]
            Name = name
            OriginalName = "Item"
            Parameters =
                parameters |> List.map (transformParameter context) |> requiredBeforeParamArray
            Type = transformType context info.Type
            TypeParameters = []
            IsOptional = false
            IsStatic = false
            Accessor =
                if info.IsReadOnly then
                    Some FSharpAccessor.ReadOnly
                else
                    Some FSharpAccessor.ReadWrite
            Accessibility = FSharpAccessibility.Public
            XmlDoc = []
            Body = FSharpMemberInfoBody.NativeOnly
        }
        |> FSharpMember.Property

    /// `f?(): void`: an F# interface has no optional method, the member holds the function instead
    let private optionalMethodSignatureMember
        (context: TransformContext)
        (info: GlueMethodSignature)
        =
        let name, context = sanitizeNameAndPushScope info.Name context
        let xmlDocInfo = transformComment info.Documentation

        {
            Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
            Name = name
            OriginalName = info.Name
            Parameters = []
            Type =
                unwrapOptionIfAlreadyOptional
                    context
                    (methodGlueFunctionType
                        info.Documentation
                        info.TypeParameters
                        info.Parameters
                        info.Type)
                    true
            TypeParameters = []
            IsOptional = true
            IsStatic = false
            Accessor = Some FSharpAccessor.ReadWrite
            Accessibility = FSharpAccessibility.Public
            XmlDoc = xmlDocInfo.XmlDoc
            Body = FSharpMemberInfoBody.JavaScriptStaticProperty
        }
        |> FSharpMember.Property

    let private methodSignatureMember (context: TransformContext) (info: GlueMethodSignature) =
        let xmlDocInfo = transformComment info.Documentation

        let methodName, parameters, emitAttributes =
            match trySpecializeStringLiteralMethod info.Name info.Parameters with
            | Some(emitText, specializedName, remainingParameters) ->
                specializedName, remainingParameters, [ FSharpAttribute.Text emitText ]
            | None -> info.Name, info.Parameters, []

        let name, context = sanitizeNameAndPushScope methodName context

        let typeParameters =
            transformTypeParameters context (withoutReferenceConstraints info.TypeParameters)

        {
            Attributes = [ yield! xmlDocInfo.ObsoleteAttributes; yield! emitAttributes ]
            Name = name
            OriginalName = info.Name
            Parameters =
                parameters
                |> List.map (
                    transformParameter context
                    >> TypeParameter.mapFsharpParameter typeParameters.SealedTypes
                )
                |> requiredBeforeParamArray
            Type =
                transformType context info.Type
                |> TypeParameter.mapFSharpType typeParameters.SealedTypes
            TypeParameters = typeParameters.TypeParameters
            IsOptional = false
            IsStatic = false
            Accessor = None
            Accessibility = FSharpAccessibility.Public
            XmlDoc = xmlDocInfo.XmlDoc
            Body = FSharpMemberInfoBody.NativeOnly
        }
        |> FSharpMember.Method

    /// `new (x: number): T` is `Create`
    let private constructSignatureMember
        (context: TransformContext)
        (info: GlueConstructSignature)
        =
        let name, context = sanitizeNameAndPushScope "Create" context

        {
            Attributes = [ FSharpAttribute.EmitConstructor ]
            Name = name
            OriginalName = "Create"
            Parameters =
                info.Parameters
                |> List.map (transformParameter context)
                |> requiredBeforeParamArray
            Type = transformType context info.Type
            TypeParameters = []
            IsOptional = false
            IsStatic = false
            Accessor = None
            Accessibility = FSharpAccessibility.Public
            XmlDoc = []
            Body = FSharpMemberInfoBody.NativeOnly
        }
        |> FSharpMember.Method

    /// `[Symbol.dispose]` is `dispose`, called through an `Emit`
    let private withDisposalNames (members: FSharpMember list) =
        members
        |> List.map (
            function
            | FSharpMember.Method info as fsharpMember ->
                match disposalMemberName info.OriginalName with
                | Some name ->
                    FSharpMember.Method
                        { info with
                            Name = name
                            Attributes =
                                info.Attributes
                                @ [
                                    FSharpAttribute.Text $"Emit(\"$0%s{info.OriginalName}($1...)\")"
                                ]
                        }
                | None -> fsharpMember
            | fsharpMember -> fsharpMember
        )

    let toFSharpMember (context: TransformContext) (members: GlueMember list) : FSharpMember list =
        members
        // The iterator information is stored in the Iterable<T> inheritance
        |> withoutComputedNames
        |> (if context.ImportSource = ImportSource.NoRuntime then
                withoutStaticMembers
            else
                id)
        |> KeyOfMaps.expandMembers context.State.KeyOfMaps
        |> withDefaultedTypeParameterOverloads
        |> UnionOverloads.expandMembers context.State.MaxOverloads context.TypeMemory
        |> withDictionaryParameterOverloads
        |> mergeAccessors members
        |> List.choose (
            function
            | GlueMember.Method info -> Some(methodMember context info)
            | GlueMember.CallSignature info -> Some(callSignatureMember context info)
            | GlueMember.Property info -> propertyMember context info
            | GlueMember.GetAccessor info ->
                accessorMember
                    context
                    info.IsStatic
                    info.Name
                    info.Documentation
                    info.Type
                    FSharpAccessor.ReadOnly
                |> Some
            | GlueMember.SetAccessor info ->
                accessorMember
                    context
                    info.IsStatic
                    info.Name
                    info.Documentation
                    info.ArgumentType
                    FSharpAccessor.WriteOnly
                |> Some
            | GlueMember.IndexSignature info -> Some(indexSignatureMember context info)
            | GlueMember.MethodSignature info when info.IsOptional ->
                Some(optionalMethodSignatureMember context info)
            | GlueMember.MethodSignature info -> Some(methodSignatureMember context info)
            | GlueMember.ConstructSignature info -> Some(constructSignatureMember context info)
        )
        |> AnyFunctionOverloads.expandMembers
        |> Merge.distinctBySignature Merge.Aliases.Empty
        |> withoutUnmentionedTypeParameters
        |> withDisposalNames

    let forceReadonly (members: FSharpMember list) =
        members
        |> List.map (
            function
            | FSharpMember.Property property ->
                { property with
                    Accessor = Some FSharpAccessor.ReadOnly
                }
                |> FSharpMember.Property
            | m -> m
        )

    /// `Pick<Link | Image, "title">` picks `title` from each case, one optional and one not:
    /// a parameter list can't name it twice, and the optional one takes both calls
    let private distinctByName (parameters: FSharpParameter list) =
        parameters
        |> List.groupBy _.Name
        |> List.map (fun (_, group) ->
            match group |> List.tryFind _.IsOptional with
            | Some optional -> optional
            | None -> List.head group
        )

    let toFSharpParameters
        (context: TransformContext)
        (members: GlueMember list)
        : FSharpParameter list
        =
        members
        |> withoutComputedNames
        |> List.map (fun glueMember ->
            let parameter (name: string) (isOptional: bool) (typ: FSharpType) : FSharpParameter =
                {
                    Attributes = []
                    Name = name
                    IsOptional = isOptional
                    Type = typ
                    OriginalGlueMember = Some glueMember
                }

            match glueMember with
            | GlueMember.Method info ->
                let name, context = sanitizeNameAndPushScope info.Name context

                methodFunctionType
                    context
                    info.Documentation
                    info.TypeParameters
                    info.Parameters
                    info.Type
                |> parameter name info.IsOptional

            | GlueMember.MethodSignature info ->
                let name, context = sanitizeNameAndPushScope info.Name context

                methodFunctionType
                    context
                    info.Documentation
                    info.TypeParameters
                    info.Parameters
                    info.Type
                |> parameter name info.IsOptional

            | GlueMember.Property info ->
                let name, context = sanitizeNameAndPushScope info.Name context
                transformType context info.Type |> parameter name info.IsOptional

            | GlueMember.GetAccessor info ->
                let name, context = sanitizeMemberNameAndPushScope info.IsStatic info.Name context
                transformType context info.Type |> parameter name false

            | GlueMember.SetAccessor info ->
                let name, context = sanitizeMemberNameAndPushScope info.IsStatic info.Name context
                transformType context info.ArgumentType |> parameter name false

            | GlueMember.IndexSignature info ->
                let name, context = sanitizeNameAndPushScope "Item" context
                transformType context info.Type |> parameter name false

            | GlueMember.CallSignature info ->
                let name, context = sanitizeNameAndPushScope "Invoke" context
                transformType context info.Type |> parameter name false

            | GlueMember.ConstructSignature info ->
                let name, context = sanitizeNameAndPushScope "Create" context
                transformType context info.Type |> parameter name false
        )
        |> distinctByName

/// The parameter sets of the `Create` members of an object type: one per combination of the
/// cases of its union properties, a single one when there is nothing to expand
/// The properties of a data object as the parameters of its `Create` or constructor, an
/// optional property is an optional parameter, not one of an option type
let private paramObjectParameters
    (context: TransformContext)
    (members: GlueMember list)
    : FSharpParameter list
    =
    members
    |> List.filter (
        function
        | GlueMember.IndexSignature _ -> false
        | _ -> true
    )
    |> TransformMembers.toFSharpParameters context
    |> List.map (fun parameter ->
        match tryUnwrapOption parameter.Type with
        | Some underlyingType ->
            { parameter with
                Type = underlyingType
                IsOptional = true
            }
        | None -> parameter
    )
    |> List.sortBy _.IsOptional

/// One parameter list per combination of the erased union cases of the parameters, `None` when
/// the parameters stay as they are: no union, too many combinations, or combinations F# can't
/// tell apart
let private tryParameterCombinations
    (parameters: FSharpParameter list)
    : FSharpParameter list list option
    =
    let tryErasedUnionCases (parameter: FSharpParameter) =
        match parameter.Type with
        | FSharpType.Union unionInfo when unionInfo.Cases.Length > 1 ->
            unionInfo.Cases
            |> List.map (
                function
                | FSharpUnionCase.Typed typ -> Some typ
                | _ -> None
            )
            |> fun cases ->
                if List.forall Option.isSome cases then
                    Some(List.choose id cases)
                else
                    None
        | _ -> None

    let variants =
        parameters
        |> List.map (fun parameter ->
            match tryErasedUnionCases parameter with
            | Some cases ->
                [
                    if parameter.IsOptional then
                        None

                    for case in cases do
                        Some
                            { parameter with
                                Type = case
                                IsOptional = false
                            }
                ]
            | None -> [ Some parameter ]
        )

    let combinationsCount =
        (1, variants)
        ||> List.fold (fun count parameterVariants ->
            min (count * parameterVariants.Length) (MAX_GENERATED_CONSTRUCTORS + 1)
        )

    if combinationsCount = 1 || combinationsCount > MAX_GENERATED_CONSTRUCTORS then
        None
    else
        let combinations =
            (variants, [ [] ])
            ||> List.foldBack (fun parameterVariants acc ->
                parameterVariants
                |> List.collect (fun variant -> acc |> List.map (fun tail -> variant :: tail))
            )
            |> List.map (List.choose id >> List.sortBy _.IsOptional)

        let hasDuplicateSignatures =
            let signatures =
                combinations |> List.map (Merge.parametersSignature Merge.Aliases.Empty)

            (List.distinct signatures).Length <> signatures.Length

        if hasDuplicateSignatures then
            None
        else
            Some combinations

let private paramObjectParameterSets
    (context: TransformContext)
    (members: GlueMember list)
    : FSharpParameter list list
    =
    let parameters = paramObjectParameters context members

    tryParameterCombinations parameters |> Option.defaultValue [ parameters ]

/// `[<ParamObject; Emit("$0")>] static member Create(...)` builds the object type as a literal,
/// the type stays an interface so it can be inherited and substituted
let private paramObjectCreateMembers
    (context: TransformContext)
    (returnType: FSharpType)
    (members: GlueMember list)
    : FSharpMember list
    =
    paramObjectParameterSets context members
    |> List.map (fun parameters ->
        {
            Attributes = [ FSharpAttribute.ParamObject; FSharpAttribute.EmitSelf ]
            Name = "Create"
            OriginalName = "Create"
            Parameters = parameters |> requiredBeforeParamArray
            TypeParameters = []
            Type = returnType
            IsOptional = false
            IsStatic = true
            Accessor = None
            Accessibility = FSharpAccessibility.Public
            XmlDoc = []
            Body = FSharpMemberInfoBody.NativeOnly
        }
        |> FSharpMember.Method
    )

let private transformParamObjectClass
    (context: TransformContext)
    (name: string)
    (documentation: GlueComment list)
    (members: GlueMember list)
    : FSharpClass
    =
    let xmlDocInfo = transformComment documentation

    let typeParameterNames =
        members |> List.collect memberTypeParameterNames |> List.distinct

    let typeLiteralParameters = paramObjectParameters context members

    let explicitFields =
        typeLiteralParameters
        |> List.map (fun parameter ->
            {
                Name = parameter.Name
                XmlDoc =
                    match parameter.OriginalGlueMember with
                    | Some glueMember -> (transformComment glueMember.Documentation).XmlDoc
                    | None -> []
                Type =
                    if parameter.IsOptional then
                        FSharpType.Option parameter.Type
                    else
                        parameter.Type
                Accessor =
                    parameter.OriginalGlueMember
                    |> Option.bind _.TryGetAccessor()
                    |> Option.map transformAccessor
            }
            : FSharpExplicitField
        )

    let primaryConstructor, secondaryConstructors =
        let paramObjectConstructor parameters =
            {
                Parameters = parameters
                Attributes = [ FSharpAttribute.ParamObject; FSharpAttribute.EmitSelf ]
                Accessibility = FSharpAccessibility.Public
            }
            : FSharpConstructor

        match tryParameterCombinations typeLiteralParameters with
        | None -> paramObjectConstructor typeLiteralParameters, []
        | Some combinations ->
            // The secondary constructors call the primary one, so it takes no parameter
            let emptyCombination, secondaryCombinations =
                combinations |> List.partition List.isEmpty

            let primaryConstructor =
                if emptyCombination.IsEmpty then
                    {
                        Parameters = []
                        Attributes = []
                        Accessibility = FSharpAccessibility.Private
                    }
                else
                    paramObjectConstructor []

            primaryConstructor, secondaryCombinations |> List.map paramObjectConstructor

    ({
        Attributes =
            [
                yield! xmlDocInfo.ObsoleteAttributes
                FSharpAttribute.Global None
                FSharpAttribute.AllowNullLiteral
            ]
        XmlDoc = xmlDocInfo.XmlDoc
        Name = name
        PrimaryConstructor = primaryConstructor
        SecondaryConstructors = secondaryConstructors
        ExplicitFields = explicitFields
        TypeParameters = declaredTypeParameters typeParameterNames
    }
    : FSharpClass)

/// `(ev: Event) => any`: the result of a callback is ignored by its caller, a lambda returns `unit`
// The caller of a callback returning `any` or `unknown` ignores the value
let private transformCallbackReturnType (context: TransformContext) (returnType: GlueType) =
    match returnType with
    | GlueType.Primitive GluePrimitive.Any
    | GlueType.Unknown -> FSharpType.Primitive FSharpPrimitive.Unit
    | _ -> transformType context returnType

/// `interface Listener { (event: Event): void }` is a function, an F# delegate takes a lambda
let private tryTransformCallableInterface
    (context: TransformContext)
    (info: GlueInterface)
    : FSharpType option
    =
    // A delegate can't carry the members of a merged second declaration
    let isDeclaredOnce =
        context.TypeMemory
        |> List.filter (
            function
            | GlueType.Interface candidate -> candidate.FullName = info.FullName
            | _ -> false
        )
        |> List.length
        <= 1

    match info.Members, info.HeritageClauses with
    | [ GlueMember.CallSignature callSignature ], [] when isDeclaredOnce ->
        let name, context = sanitizeTypeNameAndPushScope info.Name context
        let xmlDocInfo = transformComment info.Documentation

        let typeParameters =
            transformDeclarationTypeParameters
                context
                (info.TypeParameters @ callSignature.TypeParameters)

        typeParameters.TypeParameters
        |> List.rev
        |> exposeSpecializedAlias name [] []
        |> List.iter context.ExposeType

        let ownTypeParameters = transformTypeParameters context callSignature.TypeParameters

        if
            isLambdaSignature
                callSignature.Parameters
                ownTypeParameters.TypeParameters
                typeParameters.TypeParameters
        then
            ({
                Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
                Name = name
                XmlDoc = xmlDocInfo.XmlDoc
                Type =
                    ({
                        Parameters =
                            callSignature.Parameters |> List.map (transformParameter context)
                        ReturnType =
                            transformCallbackReturnType
                                (context.PushScope "ReturnType")
                                callSignature.Type
                    }
                    : FSharpFunctionType)
                    |> FSharpType.Function
                TypeParameters = typeParameters.TypeParameters
            }
            : FSharpTypeAlias)
            |> FSharpType.TypeAlias
            |> Some
        else

            ({
                XmlDoc = xmlDocInfo.XmlDoc
                Name = name
                TypeParameters = typeParameters.TypeParameters
                Parameters =
                    callSignature.Parameters
                    |> List.map (transformParameter context)
                    |> requiredBeforeParamArray
                ReturnType =
                    transformCallbackReturnType (context.PushScope "ReturnType") callSignature.Type
            }
            : FSharpDelegate)
            |> FSharpType.Delegate
            |> Some

    // `interface RequestHandler<P> extends core.RequestHandler<P> {}` is the delegate it extends
    | [], [ GlueType.TypeReference typeReference as heritage ] when
        (tryCallableHeritage context.TypeMemory heritage).IsSome
        ->
        let name, context = sanitizeTypeNameAndPushScope info.Name context
        let xmlDocInfo = transformComment info.Documentation
        let typeParameters = transformDeclarationTypeParameters context info.TypeParameters

        typeParameters.TypeParameters
        |> List.rev
        |> exposeSpecializedAlias name [] []
        |> List.iter context.ExposeType

        ({
            Attributes = []
            XmlDoc = xmlDocInfo.XmlDoc
            Name = name
            TypeParameters = typeParameters.TypeParameters
            Type = transformType context (GlueType.TypeReference typeReference)
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias
        |> Some
    | _ -> None

let private transformInterface (context: TransformContext) (info: GlueInterface) : FSharpInterface =
    let info =
        let heritageClauses, members =
            withoutRedeclaredBases context.TypeMemory info.Members info.HeritageClauses

        { info with
            HeritageClauses = heritageClauses
            Members =
                members
                |> CallableProperties.asMethods context.TypeMemory
                |> Conditionals.resolveMembers context.State.Conditionals
        }

    let name, context = sanitizeTypeNameAndPushScope info.Name context

    let xmlDocInfo = transformComment info.Documentation

    let membersComingFromPartial =
        info.HeritageClauses
        |> List.map context.ExposeTypeAlias
        |> List.choose (fun heritageClause ->
            match heritageClause with
            | GlueType.TypeReference typeReference ->
                if typeReference.IsStandardLibrary && typeReference.Name = "Partial" then
                    if typeReference.TypeArguments.Length = 1 then
                        match typeReference.TypeArguments[0] with
                        | GlueType.TypeReference typeReference -> Some typeReference.FullName
                        | _ -> None
                    else
                        // Should we throw an error or warning here?
                        // Reason: Partial should always have one type argument
                        None
                else
                    None
            | _ -> None
        )
        |> List.map (fun fullName ->
            if context.State.PartialHeritageBeingExpanded.Contains fullName then
                context.AddWarning
                    $"Recursive Partial<%s{fullName}> in a heritage clause is not supported, the inherited members are not generated"

                []
            else
                context.State.PartialHeritageBeingExpanded.Add fullName

                try
                    context.TypeMemory
                    |> List.choose (fun glueType ->
                        match glueType with
                        | GlueType.Interface glueInterface ->
                            if glueInterface.FullName = fullName then
                                transformInterface context glueInterface
                                |> Interface.makePartial "FakeName"
                                |> _.Members
                                |> Some

                            else
                                None
                        | _ -> None
                    )
                    |> List.concat
                finally
                    context.State.PartialHeritageBeingExpanded.RemoveAt(
                        context.State.PartialHeritageBeingExpanded.Count - 1
                    )
        )
        |> List.concat

    // `Omit<T, K>` resolves to an anonymous object, so when used in a heritage
    // clause (e.g. `interface Y extends Omit<X, "a">`) we inline its members
    // rather than generating an unusable `inherit Omit<...>`.
    let membersComingFromOmit =
        info.HeritageClauses
        |> List.map context.ExposeTypeAlias
        |> List.collect (fun heritageClause ->
            match heritageClause with
            | GlueType.UtilityType(GlueUtilityType.Omit members)
            | GlueType.UtilityType(GlueUtilityType.Pick members) ->
                TransformMembers.toFSharpMember context members
            | _ -> []
        )

    let standardMembers = TransformMembers.toFSharpMember context info.Members

    // `interface DebugLogger extends DebugLoggerFunction`: the call is a member instead
    let membersComingFromCallable =
        info.HeritageClauses
        |> List.choose (tryCallableHeritage context.TypeMemory)
        |> List.map GlueMember.CallSignature
        |> TransformMembers.toFSharpMember context

    let inheritance =
        info.HeritageClauses
        |> List.filter (not << isSelfHeritage info.FullName)
        |> List.map context.ExposeTypeAlias
        |> List.filter (fun heritageClause ->
            (tryCallableHeritage context.TypeMemory heritageClause).IsNone
        )
        |> List.filter (fun heritageClause ->
            match heritageClause with
            | GlueType.TypeReference typeReference ->
                not (typeReference.IsStandardLibrary && typeReference.Name = "Partial")
            // Omit members are inlined above, don't keep it as inheritance
            | GlueType.UtilityType(GlueUtilityType.Omit _)
            | GlueType.UtilityType(GlueUtilityType.Pick _) -> false
            // External base types can't be inherited
            | GlueType.Discard -> false
            | _ -> true
        )
        // `interface X extends _TygojaAny {}` of `type _TygojaAny = any`: nothing to inherit
        |> List.filter (fun heritageClause ->
            match heritageClause with
            | GlueType.TypeReference typeReference ->
                match context.State.Conditionals.AllAliases.TryFind typeReference.FullName with
                | Some { Type = GlueType.Primitive _ }
                | Some { Type = GlueType.Unknown } -> false
                | _ -> true
            | _ -> true
        )
        |> List.filter (not << isArrayHeritage)
        // An interface can't inherit the `Exception` class
        |> List.filter (not << isErrorHeritage)

    let typeParametersResult =
        transformDeclarationTypeParameters context info.TypeParameters

    let specializedAliases =
        typeParametersResult.TypeParameters
        |> List.rev
        |> exposeSpecializedAlias name [] []

    specializedAliases |> List.iter context.ExposeType

    typeParametersResult
    |> makeSealedTypeAlias
        name
        (aliasArities typeParametersResult.TypeParameters specializedAliases)
    |> Option.iter context.ExposeType

    {
        XmlDoc = xmlDocInfo.XmlDoc
        Attributes =
            [
                yield! xmlDocInfo.ObsoleteAttributes
                FSharpAttribute.AllowNullLiteral
                FSharpAttribute.Interface
            ]
        Name = name
        OriginalName = info.Name
        Members =
            let ownNames =
                standardMembers
                |> List.map (
                    function
                    | FSharpMember.Method info
                    | FSharpMember.Property info -> info.Name
                    | FSharpMember.StaticMember info -> info.Name
                )
                |> Set.ofList

            let inheritedMembers =
                membersComingFromPartial @ membersComingFromOmit @ membersComingFromCallable
                |> List.filter (
                    function
                    | FSharpMember.Method info
                    | FSharpMember.Property info -> not (Set.contains info.Name ownNames)
                    | FSharpMember.StaticMember info -> not (Set.contains info.Name ownNames)
                )

            standardMembers @ inheritedMembers
        TypeParameters = typeParametersResult.TypeParameters
        Inheritance =
            [
                // `extends Ops<"float">, Ops<"int">` are one instantiation once the literals are `string`
                yield!
                    Heritage.withoutBasesOfBases context.TypeMemory inheritance
                    |> List.map (transformType (context.PushScope "Extends"))
                    |> List.filter isInheritableType
                    |> List.distinct

                // An interface declared several times has the clauses of every declaration
                let heritageClauses =
                    context.TypeMemory
                    |> List.collect (
                        function
                        | GlueType.Interface candidate when candidate.FullName = info.FullName ->
                            candidate.HeritageClauses
                        | _ -> []
                    )
                    |> fun merged ->
                        if merged.IsEmpty then
                            info.HeritageClauses
                        else
                            merged

                // F# rejects two `IEnumerable<_>` instantiations, the one of the base type stays
                if not (inheritsIterable context.TypeMemory heritageClauses) then
                    match TypeLiteral.tryFindIterableType context info.Members with
                    | Some iterableType -> iterableType
                    | None -> ()
            ]
    }

module TypeAliasDeclaration =

    let tryTransformKeyOf (aliasName: string) (glueType: GlueType) : FSharpType option =
        let members =
            match glueType with
            | GlueType.Interface interfaceInfo -> Some interfaceInfo.Members
            | GlueType.TypeLiteral typeLiteral -> Some typeLiteral.Members
            | GlueType.TypeAliasDeclaration {
                                                Type = GlueType.TypeLiteral typeLiteral
                                            } -> Some typeLiteral.Members
            | _ -> None

        let cases =
            match members with
            | Some members ->
                members
                |> List.choose (fun m ->
                    match m with
                    | GlueMember.Method { Name = caseName }
                    | GlueMember.MethodSignature { Name = caseName }
                    | GlueMember.Property { Name = caseName }
                    | GlueMember.GetAccessor { Name = caseName }
                    | GlueMember.SetAccessor { Name = caseName } ->

                        let sanitizeResult = Naming.sanitizeTypeNameWithResult caseName

                        {
                            Attributes =
                                [
                                    if sanitizeResult.IsDifferent then
                                        caseName
                                        |> Naming.removeSurroundingQuotes
                                        |> FSharpAttribute.CompiledName
                                ]
                            Name = sanitizeResult.Name
                        }
                        |> FSharpUnionCase.Named
                        |> Some
                    // Doesn't make sense to have a case for call signature
                    | GlueMember.CallSignature _
                    | GlueMember.ConstructSignature _
                    // Doesn't make sense to have a case for index signature
                    // because index signature is used because we don't know the name
                    // of the properties and so it is used only to describe the
                    // shape of the object
                    | GlueMember.IndexSignature _ -> None
                )
            | None ->
                match glueType with
                | GlueType.Enum enumInfo ->
                    enumInfo.Members
                    |> List.map (fun m ->
                        {
                            Attributes = []
                            Name = Naming.sanitizeTypeName m.Name
                        }
                        |> FSharpUnionCase.Named
                    )
                | _ -> []

        if cases.IsEmpty then
            None
        else
            ({
                Attributes =
                    [
                        FSharpAttribute.RequireQualifiedAccess
                        FSharpAttribute.StringEnum CaseRules.None
                    ]
                Name = Naming.sanitizeTypeName aliasName
                Cases = cases
                IsOptional = false
                TypeParameters = []
                Constants = []
            }
            : FSharpUnion)
            |> FSharpType.Union
            |> Some

    let transformKeyOf
        (context: TransformContext)
        (aliasName: string)
        (glueType: GlueType)
        : FSharpType
        =
        // `keyof ScaleTypeRegistry` where the interface only inherits its members
        let glueType =
            match glueType with
            | GlueType.Interface info ->
                ParamObjectCandidate.tryResolveMembers context.TypeMemory info
                |> Option.map (fun members -> GlueType.Interface { info with Members = members })
                |> Option.defaultValue glueType
            | _ -> glueType

        match tryTransformKeyOf aliasName glueType with
        | Some typ -> typ
        | None ->
            context.AddWarning $"Could not transform KeyOf: {aliasName}"
            FSharpType.Discard

    let transformLiteral
        (xmlDoc: TransformCommentResult)
        (typeAliasName: string)
        (literalInfo: GlueLiteral)
        =
        let makeTypeAlias primitiveType =
            ({
                Attributes = [ yield! xmlDoc.ObsoleteAttributes ]
                XmlDoc = xmlDoc.XmlDoc
                Name = typeAliasName
                Type = primitiveType |> FSharpType.Primitive
                TypeParameters = []
            }
            : FSharpTypeAlias)
            |> FSharpType.TypeAlias

        match literalInfo with
        | GlueLiteral.String value ->
            let sanitizeResult = Naming.sanitizeTypeNameWithResult value

            let case =
                ({
                    Attributes =
                        [
                            if sanitizeResult.IsDifferent then
                                value
                                |> Naming.removeSurroundingQuotes
                                |> FSharpAttribute.CompiledName
                        ]
                    Name = sanitizeResult.Name
                 }
                 |> FSharpUnionCase.Named)

            ({
                Attributes =
                    [
                        FSharpAttribute.RequireQualifiedAccess
                        FSharpAttribute.StringEnum CaseRules.None
                    ]
                Name = typeAliasName
                Cases = [ case ]
                IsOptional = false
                TypeParameters = []
                Constants = []
            }
            : FSharpUnion)
            |> FSharpType.Union

        | GlueLiteral.Int _ -> makeTypeAlias FSharpPrimitive.Int
        | GlueLiteral.Float _ -> makeTypeAlias FSharpPrimitive.Float
        | GlueLiteral.Bool _ -> makeTypeAlias FSharpPrimitive.Bool
        | GlueLiteral.Null -> makeTypeAlias FSharpPrimitive.Null

let private transformRecord
    (context: TransformContext)
    (name: string)
    (typeParameters: GlueTypeParameter list)
    (recordInfo: GlueRecord)
    : FSharpType
    =
    let name, context = sanitizeNameAndPushScope name context

    let parameters =
        let name, context = sanitizeNameAndPushScope "key" context

        {
            Attributes = []
            Name = name
            IsOptional = false
            Type = transformType context recordInfo.KeyType
            OriginalGlueMember = None
        }
        |> List.singleton

    {
        XmlDoc = []
        Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
        Name = name
        OriginalName = ""
        // The interface is a declaration, a constrained parameter stays generic instead of
        // being replaced by its constraint in the members
        TypeParameters =
            transformDeclarationTypeParameters context typeParameters |> _.TypeParameters
        Members =
            {
                Attributes = [ FSharpAttribute.EmitIndexer ]
                Name = "Item"
                OriginalName = "Item"
                Parameters = parameters
                Type = transformType context recordInfo.ValueType
                TypeParameters = []
                IsOptional = false
                IsStatic = false
                Accessor = Some FSharpAccessor.ReadWrite
                Accessibility = FSharpAccessibility.Public
                XmlDoc = []
                Body = FSharpMemberInfoBody.NativeOnly
            }
            |> FSharpMember.Property
            |> List.singleton
        Inheritance = []
    }
    |> FSharpType.Interface

module private TypeParameterTransform =

    let transformWith
        (withDefault: bool)
        (context: TransformContext)
        (typeParameter: GlueTypeParameter)
        : TypeParameter.TransformResult
        =
        // The scope avoids naming an anonymous default after the declaration
        let default_ =
            if withDefault then
                typeParameter.Default
                |> Option.map (
                    transformType (context.PushScope(Naming.sanitizeTypeName typeParameter.Name))
                )
            else
                None

        match typeParameter.Constraint with
        | None -> TypeParameter.TransformResult.Create(typeParameter.Name, default_ = default_)

        | Some(GlueType.KeyOf _ as constraintType) ->
            let context = context.PushScope(Naming.sanitizeTypeName typeParameter.Name)

            match transformType context constraintType with
            | FSharpType.TypeReference _ as fsharpType ->
                TypeParameter.TransformResult.Create(
                    typeParameter.Name,
                    sealedType = fsharpType,
                    default_ = default_
                )
            | fsharpType ->
                TypeParameter.TransformResult.Create(
                    typeParameter.Name,
                    constraint_ = fsharpType,
                    default_ = default_
                )

        // An anonymous type can't be expressed as an F# constraint
        | Some(GlueType.TypeLiteral _)
        | Some(GlueType.IntersectionType _)
        | Some(GlueType.UtilityType _)
        | Some(GlueType.MappedType _)
        | Some(GlueType.FunctionType _)
        | Some(GlueType.ConstructorType _) ->
            TypeParameter.TransformResult.Create(typeParameter.Name, default_ = default_)

        | Some constraintType ->
            // Manual optimization to remove constraints that are not supported by F#
            match
                transformType
                    (context.PushScope(Naming.sanitizeTypeName typeParameter.Name))
                    constraintType
            with
            | FSharpType.Function _
            | FSharpType.TypeReference { Name = "Action" } ->
                TypeParameter.TransformResult.Create(typeParameter.Name, default_ = default_)

            // Try to resolve sealed constraints
            //
            // A sealed constraint means that the type parameter can only be the specified type.
            // Example: <'T when 'T :> string>
            //
            // The above is invalid in F#, so we manually resolve to `string` directly and notify the caller
            // to adap the code accordingly
            | FSharpType.Primitive _
            | FSharpType.Option _
            | FSharpType.Union _ as fsharpType ->
                TypeParameter.TransformResult.Create(
                    typeParameter.Name,
                    sealedType = fsharpType,
                    default_ = default_
                )

            // TypeScript satisfies `T extends Foo` structurally, F# only nominally: the
            // constraint would reject the unions and aliases TypeScript accepts
            | FSharpType.TypeReference _
            | FSharpType.JSApi _
            | FSharpType.ResizeArray _ ->
                TypeParameter.TransformResult.Create(typeParameter.Name, default_ = default_)

            // Anything else (a mapped type, a tuple, ...) is sealed or anonymous
            | _ -> TypeParameter.TransformResult.Create(typeParameter.Name, default_ = default_)

    let transform
        (context: TransformContext)
        (typeParameter: GlueTypeParameter)
        : TypeParameter.TransformResult
        =
        transformWith true context typeParameter

let private transformTypeParameters
    (context: TransformContext)
    (typeParameters: GlueTypeParameter list)
    : TransformTypeParametersResult
    =
    let transformedTypeParameters =
        typeParameters |> List.map (TypeParameterTransform.transformWith false context)

    let sealedTypes = transformedTypeParameters |> List.choose _.SealedTypeOpt

    // Only keep typeParameters not mapped to a sealed type
    // If they are mapped to a sealed type, it means that the constraint is not supported in F#
    // and we directly use the sealed type in the signature, inteas of as a generic parameter
    let typeParameters =
        transformedTypeParameters
        |> List.filter _.SealedTypeOpt.IsNone
        |> List.map _.FSharpTypeParameter

    {
        TypeParameters = typeParameters
        SealedTypes = sealedTypes
    }

let private transformDeclarationTypeParameters
    (context: TransformContext)
    (typeParameters: GlueTypeParameter list)
    : TransformDeclarationTypeParametersResult
    =
    let transformedTypeParameters =
        typeParameters |> List.map (TypeParameterTransform.transform context)

    {
        TypeParameters = transformedTypeParameters |> List.map _.FSharpTypeParameter
        SealedTypeArguments =
            if transformedTypeParameters |> List.exists _.SealedTypeOpt.IsSome then
                transformedTypeParameters
                |> List.map (fun result ->
                    match result.SealedTypeOpt with
                    | Some sealedType -> FSharpTypeParameter.FSharpType sealedType.FSharpType
                    | None -> result.FSharpTypeParameter
                )
                |> Some
            else
                None
    }

let private makeSealedTypeAlias
    (name: string)
    (existingArities: int list)
    (typeParametersResult: TransformDeclarationTypeParametersResult)
    : FSharpType option
    =
    match typeParametersResult.SealedTypeArguments with
    | None -> None
    | Some sealedTypeArguments ->
        let aliasTypeParameters =
            sealedTypeArguments
            |> List.filter (
                function
                | FSharpTypeParameter.FSharpTypeParameter _ -> true
                | FSharpTypeParameter.FSharpType _ -> false
            )

        if List.contains aliasTypeParameters.Length existingArities then
            None
        else
            ({
                Attributes = []
                XmlDoc = []
                Name = name
                Type =
                    ({
                        Name = name
                        TypeParameters = sealedTypeArguments
                    }
                    : FSharpMapped)
                    |> FSharpType.Mapped
                TypeParameters = aliasTypeParameters
            }
            : FSharpTypeAlias)
            |> FSharpType.TypeAlias
            |> Some

let private tryOptimizeUnionType
    (context: TransformContext)
    (typeName: string)
    (cases: GlueType list)
    : FSharpType option
    =
    // Unions can have nested unions, so we need to flatten them
    // TODO: Is there cases where we don't want to flatten?
    // U2<U2<int, string>, bool>
    // `type OpUnitType = UnitType | "week"` where `UnitType` is made of literals: one string enum
    let rec literalAlias (depth: int) (typeReference: GlueTypeReference) : GlueType list option =
        if depth > 5 || not typeReference.TypeArguments.IsEmpty then
            None
        else
            context.TypeMemory
            |> List.tryPick (
                function
                | GlueType.TypeAliasDeclaration {
                                                    FullName = fullName
                                                    TypeParameters = []
                                                    Type = GlueType.Union(GlueTypeUnion aliasCases)
                                                } when
                    fullName = typeReference.FullName && fullName <> ""
                    ->
                    let resolved =
                        aliasCases
                        |> List.map (
                            function
                            | GlueType.Literal _ as literal -> Some [ literal ]
                            | GlueType.TypeReference inner -> literalAlias (depth + 1) inner
                            | GlueType.Primitive GluePrimitive.Null
                            | GlueType.Primitive GluePrimitive.Undefined -> Some []
                            | _ -> None
                        )

                    if resolved |> List.forall Option.isSome then
                        Some(resolved |> List.collect Option.get)
                    else
                        None
                | _ -> None
            )

    let rec flattenCasesWith (aliases: bool) (cases: GlueType list) : GlueType list =
        cases
        |> List.collect (
            function
            // We are inside an union, and have access to the literal types
            | GlueType.Literal _ as literal -> [ literal ]
            | GlueType.Union(GlueTypeUnion cases) -> flattenCasesWith aliases cases
            | GlueType.TypeAliasDeclaration aliasCases as aliasType ->
                match aliasCases.Type with
                | GlueType.Union(GlueTypeUnion cases) -> flattenCasesWith aliases cases
                | _ -> [ aliasType ]
            | GlueType.TypeReference typeReference as referenceType when aliases ->
                match literalAlias 0 typeReference with
                | Some literals -> literals
                | None -> [ referenceType ]
            | GlueType.Primitive GluePrimitive.Null
            | GlueType.Primitive GluePrimitive.Undefined -> []
            | otherType -> [ otherType ]
        )

    let isLiteral (glueType: GlueType) =
        match glueType with
        | GlueType.Literal _ -> true
        | _ -> false

    /// `boolean | "auto"` has two values, an enum case each, where `bool` would be a payload
    let expandBooleanCases (cases: GlueType list) =
        let hasStringLiteral =
            cases
            |> List.exists (
                function
                | GlueType.Literal(GlueLiteral.String _) -> true
                | _ -> false
            )

        if hasStringLiteral then
            cases
            |> List.collect (
                function
                | GlueType.Primitive GluePrimitive.Bool ->
                    [
                        GlueType.Literal(GlueLiteral.Bool true)
                        GlueType.Literal(GlueLiteral.Bool false)
                    ]
                | case -> [ case ]
            )
        else
            cases

    // `Signals | number` keeps `Signals` as a case, only a union made of literals is one enum
    let flattenCases (cases: GlueType list) : GlueType list =
        let withAliases = flattenCasesWith true cases |> expandBooleanCases

        if withAliases |> List.forall isLiteral then
            withAliases
        else
            flattenCasesWith false cases

    let flattenedCases, otherCases = flattenCases cases |> List.partition isLiteral

    // A union made only of string, boolean and integer literals
    // (with at least one string) can be represented as a Fable StringEnum.
    // Boolean and integer cases are emitted using [<CompiledValue(...)>] so that Fable
    // compiles them to the raw value instead of a string.
    let isStringEnumCompatible =
        // If the list is empty, it means that there was no candidates
        // for string literals
        not flattenedCases.IsEmpty
        && flattenedCases
           |> List.exists (
               function
               | GlueType.Literal(GlueLiteral.String _) -> true
               | _ -> false
           )
        && flattenedCases
           |> List.forall (
               function
               | GlueType.Literal(GlueLiteral.String _)
               | GlueType.Literal(GlueLiteral.Bool _)
               | GlueType.Literal(GlueLiteral.Int _) -> true
               | _ -> false
           )

    let isNumericOnly =
        // If the list is empty, it means that there was no candidates
        // for numeric literals
        not flattenedCases.IsEmpty
        && flattenedCases
           |> List.forall (
               function
               | GlueType.Literal(GlueLiteral.Int _) -> true
               | _ -> false
           )

    let transformFieldCases (literalCases: FSharpUnionCase list) =
        let literalNames =
            literalCases
            |> List.choose (
                function
                | FSharpUnionCase.Named caseInfo -> Some caseInfo.Name
                | _ -> None
            )
            |> Set.ofList

        otherCases
        |> List.fold
            (fun (index, fieldCases) caseType ->
                let rec nextName index =
                    let name = $"Case%i{index}"

                    if Set.contains name literalNames then
                        nextName (index + 1)
                    else
                        index, name

                let index, name = nextName index
                let context = context.PushScope("Cases").PushScope name

                index + 1,
                FSharpUnionCase.Field(name, transformType context caseType) :: fieldCases
            )
            (1, [])
        |> snd
        |> List.rev

    let literalCaseInfo (literal: GlueLiteral) : FSharpUnionCaseNamed =
        match literal with
        | GlueLiteral.String value ->
            let sanitizeResult = Naming.sanitizeTypeNameWithResult value

            {
                Attributes =
                    [
                        if sanitizeResult.IsDifferent then
                            value |> Naming.removeSurroundingQuotes |> FSharpAttribute.CompiledName
                    ]
                Name = sanitizeResult.Name
            }
        | GlueLiteral.Bool value ->
            // Booleans can't be represented as a StringEnum case name,
            // so we use [<CompiledValue(...)>] to emit the raw value
            {
                Attributes = [ FSharpAttribute.CompiledValue(FSharpLiteral.Bool value) ]
                Name =
                    if value then
                        "True"
                    else
                        "False"
            }
        | GlueLiteral.Int value ->
            {
                Attributes = [ FSharpAttribute.CompiledValue(FSharpLiteral.Int value) ]
                Name = $"``%i{value}``"
            }
        | GlueLiteral.Float _
        | GlueLiteral.Null -> failwith "Should not happen"

    let deduplicateCaseNames (caseInfos: FSharpUnionCaseNamed list) =
        caseInfos
        |> List.mapFold
            (fun usedNames (caseInfo: FSharpUnionCaseNamed) ->
                if Set.contains caseInfo.Name usedNames then
                    let isEscaped = caseInfo.Name.StartsWith("``")
                    let baseName = caseInfo.Name.Trim('`')

                    let rec nextName index =
                        let candidate =
                            if isEscaped then
                                $"``%s{baseName}_%i{index}``"
                            else
                                $"%s{baseName}_%i{index}"

                        if Set.contains candidate usedNames then
                            nextName (index + 1)
                        else
                            candidate

                    let name = nextName 1

                    let hasCompiledValue =
                        caseInfo.Attributes
                        |> List.exists (
                            function
                            | FSharpAttribute.CompiledName _
                            | FSharpAttribute.CompiledValue _ -> true
                            | _ -> false
                        )

                    {
                        Attributes =
                            if hasCompiledValue then
                                caseInfo.Attributes
                            else
                                caseInfo.Attributes @ [ FSharpAttribute.CompiledName baseName ]
                        Name = name
                    },
                    Set.add name usedNames
                else
                    caseInfo, Set.add caseInfo.Name usedNames
            )
            Set.empty
        |> fst

    let tryFindTaggedUnion () =
        let casesProperties =
            otherCases
            |> List.map (
                function
                | GlueType.TypeLiteral typeLiteral ->
                    typeLiteral.Members
                    |> List.map (
                        function
                        | GlueMember.Property property -> Some property
                        | _ -> None
                    )
                    |> fun properties ->
                        if List.forall Option.isSome properties then
                            properties |> List.choose id |> Some
                        else
                            None
                | _ -> None
            )

        if
            flattenedCases.IsEmpty
            && casesProperties.Length >= 2
            && List.forall Option.isSome casesProperties
        then
            let casesProperties = casesProperties |> List.choose id

            let tagValue (tagName: string) (properties: GlueProperty list) =
                properties
                |> List.tryFind (fun property -> property.Name = tagName)
                |> Option.bind (fun property ->
                    match property.Type with
                    | GlueType.Literal(GlueLiteral.String _ as literal)
                    | GlueType.Literal(GlueLiteral.Int _ as literal)
                    | GlueType.Literal(GlueLiteral.Bool _ as literal) when not property.IsOptional ->
                        Some literal
                    | _ -> None
                )

            casesProperties.Head
            |> List.tryPick (fun candidate ->
                let tagValues = casesProperties |> List.map (tagValue candidate.Name)

                if
                    List.forall Option.isSome tagValues
                    && (List.distinct tagValues).Length = tagValues.Length
                then
                    Some(candidate.Name, List.zip (List.choose id tagValues) casesProperties)
                else
                    None
            )
        else
            None

    match tryFindTaggedUnion () with
    | Some(tagName, taggedCases) ->
        let caseInfos =
            taggedCases |> List.map (fst >> literalCaseInfo) |> deduplicateCaseNames

        let cases =
            List.zip caseInfos taggedCases
            |> List.map (fun (caseInfo, (_, properties)) ->
                let caseContext = context.PushScope("Cases").PushScope(caseInfo.Name.Trim('`'))

                let fields =
                    properties
                    |> List.filter (fun property -> property.Name <> tagName)
                    |> List.map (fun property ->
                        let fieldName, fieldContext =
                            sanitizeNameAndPushScope property.Name caseContext

                        let fieldType =
                            match transformType fieldContext property.Type with
                            | FSharpType.Option _ as optionType -> optionType
                            | fieldType when property.IsOptional -> FSharpType.Option fieldType
                            | fieldType -> fieldType

                        fieldName, fieldType
                    )

                FSharpUnionCase.NamedFields(caseInfo, fields)
            )

        ({
            Attributes =
                [
                    FSharpAttribute.RequireQualifiedAccess
                    FSharpAttribute.TypeScriptTaggedUnion(
                        Naming.removeSurroundingQuotes tagName,
                        CaseRules.None
                    )
                ]
            Name = typeName
            Cases = cases
            IsOptional = false
            TypeParameters = []
            Constants = []
        }
        : FSharpUnion)
        |> FSharpType.Union
        |> Some

    | None ->
        if isStringEnumCompatible then
            let literalCases =
                flattenedCases
                |> List.map (
                    function
                    | GlueType.Literal literal -> literalCaseInfo literal
                    | _ -> failwith "Should not happen"
                )
                |> List.distinct
                |> deduplicateCaseNames
                |> List.map FSharpUnionCase.Named

            let fieldCases = transformFieldCases literalCases

            ({
                Attributes =
                    [
                        FSharpAttribute.RequireQualifiedAccess
                        if fieldCases.IsEmpty then
                            FSharpAttribute.StringEnum CaseRules.None
                        else
                            FSharpAttribute.EraseWithCaseRules CaseRules.None
                    ]
                Name = typeName
                Cases = literalCases @ fieldCases
                IsOptional = false
                TypeParameters = []
                Constants = []
            }
            : FSharpUnion)
            |> FSharpType.Union
            |> Some
        else if isNumericOnly && not otherCases.IsEmpty then
            let literalCases =
                flattenedCases
                |> List.map (fun value ->
                    match value with
                    | GlueType.Literal(GlueLiteral.Int value) ->
                        {
                            Attributes = [ FSharpAttribute.CompiledValue(FSharpLiteral.Int value) ]
                            Name = $"``%i{value}``"
                        }
                        |> FSharpUnionCase.Named
                    | _ -> failwith "Should not happen"
                )
                |> List.distinct

            ({
                Attributes =
                    [
                        FSharpAttribute.RequireQualifiedAccess
                        FSharpAttribute.EraseWithCaseRules CaseRules.None
                    ]
                Name = typeName
                Cases = literalCases @ transformFieldCases literalCases
                IsOptional = false
                TypeParameters = []
                Constants = []
            }
            : FSharpUnion)
            |> FSharpType.Union
            |> Some
        // If the union contains only literal numbers,
        // we can transform it into a standard F# enum
        else if isNumericOnly then
            let cases =
                flattenedCases
                |> List.map (fun value ->
                    match value with
                    | GlueType.Literal(GlueLiteral.Int value) ->
                        {
                            // `-1` is not an identifier
                            Name =
                                if value < 0 then
                                    $"_MINUS_{-value}"
                                else
                                    value.ToString()
                            Value = FSharpLiteral.Int value
                        }
                        : FSharpEnumCase
                    | _ -> failwith "Should not happen"
                )
                |> List.distinct

            ({ Name = typeName; Cases = cases }: FSharpEnum) |> FSharpType.Enum |> Some

        else
            // Let the caller generate an erased union (`U2`, `U3`, ...). See #55
            None

let private transformMappedTypeMembers (context: TransformContext) (mappedType: GlueMappedType) =
    let valueType = mappedType.Type |> Option.defaultValue GlueType.Unknown

    // `T[K]` is the type of the member itself
    let memberType (memberType: GlueType) =
        match valueType with
        | GlueType.IndexedAccessType _ -> memberType
        | _ -> valueType

    let property (name: string) (typ: GlueType) =
        {
            Name = name
            Documentation = []
            Type = typ
            IsStatic = false
            IsOptional = false
            Accessor = GlueAccessor.ReadWrite
            IsPrivate = false
        }
        |> GlueMember.Property

    // The keys can't be listed, any key maps to the value
    let indexer =
        [
            ({
                Parameters =
                    [
                        {
                            Name = "key"
                            IsOptional = false
                            IsSpread = false
                            Type = GlueType.Primitive GluePrimitive.String
                        }
                    ]
                Type =
                    match valueType with
                    | GlueType.IndexedAccessType _ -> GlueType.Primitive GluePrimitive.Any
                    | _ ->
                        GlueSubstitution.substitute
                            (Map.ofList
                                [
                                    mappedType.TypeParameter.Name,
                                    GlueType.Primitive GluePrimitive.String
                                ])
                            valueType
                IsReadOnly = false
            }
            : GlueIndexSignature)
            |> GlueMember.IndexSignature
        ]

    let ofLiterals (cases: GlueType list) =
        let literals =
            cases
            |> List.choose (
                function
                | GlueType.Literal literalInfo -> Some literalInfo
                | _ -> None
            )

        if literals.Length = cases.Length && not literals.IsEmpty then
            literals
            |> List.map (fun literalInfo -> property (literalInfo.ToText()) valueType)
        else
            indexer

    let ofMembers (members: GlueMember list) =
        members
        |> List.choose (
            function
            | GlueMember.Property info -> Some(property info.Name (memberType info.Type))
            | GlueMember.Method info -> Some(property info.Name (memberType info.Type))
            | GlueMember.MethodSignature info -> Some(property info.Name (memberType info.Type))
            | GlueMember.GetAccessor info -> Some(property info.Name (memberType info.Type))
            | _ -> None
        )

    match mappedType.TypeParameter.Constraint with
    // `[K in (typeof keys)[number]]`
    | Some(GlueType.IndexedAccessType {
                                          ObjectType = GlueType.ReadOnly(GlueType.TupleType glueTypes)
                                      })
    | Some(GlueType.IndexedAccessType {
                                          ObjectType = GlueType.TupleType glueTypes
                                      }) -> ofLiterals glueTypes
    | Some(GlueType.Union(GlueTypeUnion cases)) -> ofLiterals cases
    // `[K in Signals]` where `Signals` is a union of literals
    | Some(GlueType.TypeReference typeReference) ->
        context.TypeMemory
        |> List.tryPick (
            function
            | GlueType.TypeAliasDeclaration {
                                                Name = name
                                                Type = GlueType.Union(GlueTypeUnion cases)
                                            } when name = typeReference.Name ->
                Some(ofLiterals cases)
            | _ -> None
        )
        |> Option.defaultValue indexer
    | Some(GlueType.KeyOf(GlueType.Interface info)) -> ofMembers info.Members
    | Some(GlueType.KeyOf(GlueType.TypeLiteral info)) -> ofMembers info.Members
    | _ -> indexer

let private transformReadOnlyModifier (context: TransformContext) (glueType: GlueType) =

    match glueType with
    | GlueType.Array arrayType ->
        context.ExposeReadonlyArray()

        transformType context arrayType |> FSharpJSApi.ReadonlyArray |> FSharpType.JSApi

    // Ignore readonly for other types
    | _ -> transformType context glueType

/// What the arms of a type alias declaration build from
type private AliasScope =
    {
        /// The F# name of the alias, the anonymous types of its body are scoped under it
        Name: string
        Context: TransformContext
        Declaration: GlueTypeAliasDeclaration
        XmlDoc: TransformCommentResult
        /// Forced by the arms declaring type parameters, the specialized aliases follow it
        TypeParameters: Lazy<TransformDeclarationTypeParametersResult>
    }

/// `type X = ...` as an F# abbreviation of `typ`
let private aliasOf (scope: AliasScope) (typ: FSharpType) : FSharpType =
    ({
        Attributes = [ yield! scope.XmlDoc.ObsoleteAttributes ]
        XmlDoc = scope.XmlDoc.XmlDoc
        Name = scope.Name
        Type = typ
        TypeParameters = scope.TypeParameters.Value.TypeParameters
    }
    : FSharpTypeAlias)
    |> FSharpType.TypeAlias

/// `type X = { ... }` as an interface named after the alias
let private aliasInterfaceOf
    (scope: AliasScope)
    (typeParameters: FSharpTypeParameter list)
    (members: FSharpMember list)
    (inheritance: FSharpType list)
    : FSharpType
    =
    {
        XmlDoc = []
        Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
        Name = scope.Name
        OriginalName = scope.Declaration.Name
        TypeParameters = typeParameters
        Members = members
        Inheritance = inheritance
    }
    |> FSharpType.Interface

let private aliasOfUnion (scope: AliasScope) (cases: GlueType list) (unionType: GlueType) =
    let context = scope.Context

    match tryOptimizeUnionType context scope.Name cases with
    // `type Slot<T> = FacetReader<T> | StateField<T> | "doc"` is generic
    | Some(FSharpType.Union unionInfo) ->
        FSharpType.Union
            { unionInfo with
                TypeParameters = scope.TypeParameters.Value.TypeParameters
            }
    | Some typ -> typ
    | None ->
        let isNullable =
            function
            | GlueType.Primitive GluePrimitive.Null
            | GlueType.Primitive GluePrimitive.Undefined -> true
            | _ -> false

        match cases |> List.partition isNullable with
        // `type Foo = { ... } | undefined`, the scope avoids naming the anonymous type after the alias
        | _ :: _, [ single ] ->
            transformType (context.PushScope "Value") single
            |> FSharpType.Option
            |> aliasOf scope
        | _, others when others.Length > 9 ->
            let caseTypes =
                others
                |> List.mapi (fun index caseType ->
                    transformType (context.PushScope $"Case%i{index + 1}") caseType
                )
                |> List.distinct

            erasedUnion scope.Name scope.TypeParameters.Value.TypeParameters caseTypes
            |> FSharpType.Union
        | _ -> transformType context unionType |> aliasOf scope

/// `type Value = Foo[keyof Foo]` is the union of the member types
let private aliasOfIndexedAccessType (scope: AliasScope) (glueType: GlueIndexedAccessType) =
    match glueType.IndexType with
    | GlueType.KeyOf(GlueType.Interface interfaceInfo) ->
        interfaceInfo.Members
        |> List.collect (fun m ->
            match m with
            | GlueMember.Method { Type = typ }
            | GlueMember.Property { Type = typ }
            | GlueMember.GetAccessor { Type = typ }
            | GlueMember.SetAccessor { ArgumentType = typ }
            | GlueMember.CallSignature { Type = typ }
            | GlueMember.ConstructSignature { Type = typ }
            | GlueMember.MethodSignature { Type = typ }
            | GlueMember.IndexSignature { Type = typ } ->
                match typ with
                | GlueType.Union(GlueTypeUnion cases) -> cases
                | _ -> [ typ ]
        )
        |> List.distinct
        |> GlueTypeUnion
        |> GlueType.Union
        |> transformType scope.Context
        |> aliasOf scope
    | _ -> aliasOf scope FSharpType.Discard

let private aliasOfTypeReference (scope: AliasScope) (typeReference: GlueTypeReference) =
    let mappedName = mapTypeNameToFableCoreAwareName scope.Context typeReference
    let context = scope.Context.PushScope mappedName

    match typeReference.TypeArguments with
    // `type X = Promise<A & B>`: the intersection is a real interface, not `obj`
    | [ GlueType.IntersectionType members ] ->
        let typeParameters = scope.TypeParameters.Value.TypeParameters

        let makeInterfaceTyp name =
            {
                XmlDoc = []
                Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
                Name = name
                OriginalName = scope.Declaration.Name
                TypeParameters = typeParameters
                Members = TransformMembers.toFSharpMember context members
                Inheritance = []
            }

        let exposedType = makeInterfaceTyp "ReturnType"

        let typeArgument = makeInterfaceTyp (context.FullName + ".ReturnType")

        let context = context.PushScope typeReference.Name

        context.ExposeType(FSharpType.Interface exposedType)

        ({
            Attributes = [ yield! scope.XmlDoc.ObsoleteAttributes ]
            XmlDoc = scope.XmlDoc.XmlDoc
            Name = scope.Name
            Type =
                {
                    Name = mappedName
                    FullName = typeReference.FullName
                    ModulePath = typeReference.ModulePath
                    TypeArguments = [ FSharpType.Interface typeArgument ]
                    Type = FSharpType.Discard
                }
                |> FSharpType.TypeReference
            TypeParameters = typeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias
    | _ -> transformType context (GlueType.TypeReference typeReference) |> aliasOf scope

let private aliasOfUtilityType (scope: AliasScope) (utilityType: GlueUtilityType) =
    let context = scope.Context

    match utilityType with
    | GlueUtilityType.Partial interfaceInfo ->
        // `type PartialSchema<T> = Partial<Schema<T>>` declares the type parameters its members use
        let interfaceInfo =
            if interfaceInfo.TypeParameters.IsEmpty then
                { interfaceInfo with
                    TypeParameters = scope.Declaration.TypeParameters
                }
            else
                interfaceInfo

        transformInterface context interfaceInfo
        |> Interface.makePartial scope.Name
        |> FSharpType.Interface

    | GlueUtilityType.Record recordInfo ->
        transformRecord context scope.Name scope.Declaration.TypeParameters recordInfo

    | GlueUtilityType.ReturnType returnType ->
        transformType (context.PushScope "ReturnType") returnType |> aliasOf scope

    | GlueUtilityType.ThisParameterType innerType ->
        transformType context innerType |> aliasOf scope

    | GlueUtilityType.Omit members
    | GlueUtilityType.Pick members ->
        let typeParameters = scope.TypeParameters.Value.TypeParameters

        let candidate =
            ({
                Documentation = scope.Declaration.Documentation
                FullName = scope.Declaration.FullName
                Name = scope.Declaration.Name
                Members = members
                TypeParameters = scope.Declaration.TypeParameters
                HeritageClauses = []
            }
            : GlueInterface)

        let creates =
            if ParamObjectCandidate.isCandidate context.TypeMemory candidate then
                let returnType =
                    ({
                        Name = scope.Name
                        TypeParameters = typeParameters
                    }
                    : FSharpMapped)
                    |> FSharpType.Mapped

                paramObjectCreateMembers context returnType members
            else
                []

        aliasInterfaceOf
            scope
            typeParameters
            (TransformMembers.toFSharpMember context members @ creates)
            []

    | GlueUtilityType.Readonly readonlyInfo ->
        UtilityType.transformReadOnlyDeclaration context readonlyInfo (aliasOf scope)

/// `type Handler = (event: Event) => void` is a delegate
let private aliasOfFunctionType (scope: AliasScope) (functionType: GlueFunctionType) =
    let context = scope.Context

    // An F# abbreviation or delegate cannot declare the type parameters of the function itself
    let ownDefaults = ownTypeParameterDefaults functionType

    let parameters =
        functionType.Parameters
        |> List.map (GlueSubstitution.substituteParameter ownDefaults)

    let returnType = GlueSubstitution.substitute ownDefaults functionType.Type

    let declaredTypeParameters = scope.TypeParameters.Value.TypeParameters

    if isLambdaSignature parameters [] declaredTypeParameters then
        ({
            Parameters = parameters |> List.map (transformParameter context)
            ReturnType = transformCallbackReturnType (context.PushScope "ReturnType") returnType
        }
        : FSharpFunctionType)
        |> FSharpType.Function
        |> aliasOf scope
    else

        ({
            XmlDoc = scope.XmlDoc.XmlDoc
            Name = scope.Name
            TypeParameters = declaredTypeParameters
            Parameters =
                parameters |> List.map (transformParameter context) |> requiredBeforeParamArray
            // The scope keeps an anonymous return type from taking the name of the delegate
            ReturnType = transformCallbackReturnType (context.PushScope "ReturnType") returnType
        }
        : FSharpDelegate)
        |> FSharpType.Delegate

/// `type Options = { ... }` is an interface, with a `Create` when it is a plain data object
let private aliasOfTypeLiteral (scope: AliasScope) (typeLiteralInfo: GlueTypeLiteral) =
    let context = scope.Context
    let typeParameters = scope.TypeParameters.Value.TypeParameters

    let candidate =
        ({
            Documentation = scope.Declaration.Documentation
            FullName = scope.Declaration.FullName
            Name = scope.Declaration.Name
            Members = typeLiteralInfo.Members
            TypeParameters = scope.Declaration.TypeParameters
            HeritageClauses = []
        }
        : GlueInterface)

    let creates =
        if ParamObjectCandidate.isCandidate context.TypeMemory candidate then
            let returnType =
                ({
                    Name = scope.Name
                    TypeParameters = typeParameters
                }
                : FSharpMapped)
                |> FSharpType.Mapped

            paramObjectCreateMembers context returnType typeLiteralInfo.Members
        else
            []

    aliasInterfaceOf
        scope
        typeParameters
        (TransformMembers.toFSharpMember context typeLiteralInfo.Members @ creates)
        (TypeLiteral.tryFindIterableType context typeLiteralInfo.Members |> Option.toList)

/// `type Ctor = new () => X` is an interface with a `Create`, `type Ctor<T> = new () => T` an
/// erased single case union
let private aliasOfConstructorType
    (scope: AliasScope)
    (constructSignature: GlueConstructSignature)
    =
    let context = scope.Context

    match scope.Declaration.TypeParameters with
    | [] ->
        {
            XmlDoc = scope.XmlDoc.XmlDoc
            Attributes =
                [
                    yield! scope.XmlDoc.ObsoleteAttributes
                    FSharpAttribute.AllowNullLiteral
                    FSharpAttribute.Interface
                ]
            Name = scope.Name
            OriginalName = scope.Declaration.Name
            TypeParameters = []
            Members =
                TransformMembers.toFSharpMember
                    context
                    [ GlueMember.ConstructSignature constructSignature ]
            Inheritance = []
        }
        |> FSharpType.Interface

    | [ typeParameter ] ->
        ({
            Attributes = [ yield! scope.XmlDoc.ObsoleteAttributes ]
            XmlDoc = scope.XmlDoc.XmlDoc
            Name = scope.Name
            TypeParameter =
                TypeParameterTransform.transform context typeParameter |> _.FSharpTypeParameter
        }
        : FSharpSingleErasedCaseUnion)
        |> FSharpType.SingleErasedCaseUnion

    | _ ->
        context.AddWarning
            $"%s{scope.Declaration.Name} contains a ConstructorType with multiple type parameters, please open an issue at https://github.com/glutinum-org/cli/issues"

        // This is probably going to generate invalid F# code,
        // especially if the type is used as a signature type somewhere
        // But it is better to remove the TypeParameters to make it easier for the user to fix
        // by removing the TypeParameters from the type signature
        aliasOf scope FSharpType.Object

let private transformTypeAliasDeclaration
    (context: TransformContext)
    (glueTypeAliasDeclaration: GlueTypeAliasDeclaration)
    : FSharpType
    =
    let typeAliasName, context =
        sanitizeTypeNameAndPushScope glueTypeAliasDeclaration.Name context

    let scope =
        {
            Name = typeAliasName
            Context = context
            Declaration = glueTypeAliasDeclaration
            XmlDoc = transformComment glueTypeAliasDeclaration.Documentation
            TypeParameters =
                lazy
                    (transformDeclarationTypeParameters
                        context
                        glueTypeAliasDeclaration.TypeParameters)
        }

    // The declaration type parameters are forced by the arms declaring them only
    let aliasInterface (members: FSharpMember list) (inheritance: FSharpType list) =
        aliasInterfaceOf scope scope.TypeParameters.Value.TypeParameters members inheritance

    let fsharpType =
        match replaceSelfReference glueTypeAliasDeclaration.Name glueTypeAliasDeclaration.Type with
        | GlueType.Union(GlueTypeUnion cases) as unionType -> aliasOfUnion scope cases unionType

        | GlueType.KeyOf glueType ->
            match
                TypeAliasDeclaration.transformKeyOf context glueTypeAliasDeclaration.Name glueType
            with
            // `keyof StoreMutators<unknown, unknown>` of an empty registry has no key, the
            // alias is still referenced by the re-exports
            | FSharpType.Discard -> aliasOf scope FSharpType.Object
            | typ -> typ

        | GlueType.IndexedAccessType glueType -> aliasOfIndexedAccessType scope glueType

        | GlueType.Literal literalInfo ->
            TypeAliasDeclaration.transformLiteral scope.XmlDoc typeAliasName literalInfo

        | GlueType.Primitive primitiveInfo ->
            transformPrimitive primitiveInfo |> FSharpType.Primitive |> aliasOf scope

        | GlueType.TemplateLiteral -> FSharpType.Primitive FSharpPrimitive.String |> aliasOf scope

        | GlueType.TypeReference typeReference -> aliasOfTypeReference scope typeReference

        | GlueType.Array glueType ->
            transformType context (GlueType.Array glueType) |> aliasOf scope

        | GlueType.UtilityType utilityType -> aliasOfUtilityType scope utilityType

        | GlueType.FunctionType functionType -> aliasOfFunctionType scope functionType

        // The scope avoids naming an anonymous element type after the alias
        | GlueType.TupleType glueTypes ->
            transformTupleType (context.PushScope "Item") glueTypes |> aliasOf scope

        | GlueType.IntersectionType members ->
            aliasInterface (TransformMembers.toFSharpMember context members) []

        | GlueType.IntersectionOfReferences(references, members) ->
            match inheritableBases context references with
            | [] -> aliasOf scope FSharpType.Object
            | bases ->
                aliasInterface
                    (TransformMembers.toFSharpMember context members)
                    (bases |> List.map (transformType context))

        | GlueType.TypeLiteral typeLiteralInfo -> aliasOfTypeLiteral scope typeLiteralInfo

        | GlueType.Unknown -> aliasOf scope FSharpType.Object

        | GlueType.TypeParameter typeParameterInfo ->
            FSharpType.TypeParameter typeParameterInfo |> aliasOf scope

        | GlueType.MappedType mappedType ->
            aliasInterface
                (mappedType
                 |> transformMappedTypeMembers context
                 |> TransformMembers.toFSharpMember context)
                []

        | GlueType.ConstructorType constructSignature ->
            aliasOfConstructorType scope constructSignature

        | GlueType.ReadOnly glueType ->
            transformReadOnlyModifier (context.PushScope "Item") glueType |> aliasOf scope

        // We don't know how to handle these types yet, so we default to obj
        | GlueType.ClassDeclaration _
        | GlueType.FileModule _
        | GlueType.ReExport _
        | GlueType.ConditionalType _
        | GlueType.Enum _
        | GlueType.Interface _
        | GlueType.ModuleDeclaration _
        | GlueType.TypeAliasDeclaration _
        | GlueType.Discard
        | GlueType.FunctionDeclaration _
        | GlueType.ThisType _
        | GlueType.Variable _
        | GlueType.ExportDefault _
        | GlueType.NamedTupleType _
        | GlueType.OptionalType _ -> aliasOf scope FSharpType.Object

    if scope.TypeParameters.IsValueCreated then
        let typeParameters = scope.TypeParameters.Value.TypeParameters

        let defaultAliases =
            typeParameters |> List.rev |> exposeSpecializedAlias typeAliasName [] []

        defaultAliases |> List.iter context.ExposeType

        scope.TypeParameters.Value
        |> makeSealedTypeAlias typeAliasName (aliasArities typeParameters defaultAliases)
        |> Option.iter context.ExposeType

    fsharpType

module private ReExport =

    /// A constraint the transform writes the same way wherever it appears
    let rec private isExpressible (glueType: GlueType) =
        match glueType with
        | GlueType.TypeReference _
        | GlueType.Primitive _
        | GlueType.Unknown -> true
        // The transform keeps the wrapper and writes the inner type
        | GlueType.ReadOnly innerType
        | GlueType.Array innerType
        | GlueType.OptionalType innerType -> isExpressible innerType
        | GlueType.Union(GlueTypeUnion cases) -> cases |> List.forall isExpressible
        | GlueType.TupleType elements -> elements |> List.forall isExpressible
        | _ -> false

    let canForwardConstraints (typeParameters: GlueTypeParameter list) =
        typeParameters
        |> List.forall (fun typeParameter ->
            match typeParameter.Constraint with
            | None
            // Dropped by the transform
            | Some(GlueType.TypeLiteral _)
            | Some(GlueType.IntersectionType _)
            | Some(GlueType.UtilityType _)
            | Some(GlueType.MappedType _) -> true
            | Some constraintType -> isExpressible constraintType
        )

let private transformReExport
    (context: TransformContext)
    (reExport: GlueReExport)
    : FSharpType list
    =
    let declared =
        match reExport.Declaration with
        | GlueType.Interface info -> Some(info.Name, info.TypeParameters)
        // `type Fn = <T>(value: T) => T` is a generic delegate
        | GlueType.TypeAliasDeclaration({
                                            Type = GlueType.FunctionType {
                                                                             OwnTypeParameterNames = ownNames
                                                                             TypeParameters = functionTypeParameters
                                                                         }
                                        } as info) when not ownNames.IsEmpty ->
            let own =
                functionTypeParameters
                |> List.filter (fun typeParameter -> List.contains typeParameter.Name ownNames)

            Some(info.Name, info.TypeParameters @ own)
        | GlueType.TypeAliasDeclaration info -> Some(info.Name, info.TypeParameters)
        | GlueType.ClassDeclaration info -> Some(info.Name, info.TypeParameters)
        | GlueType.Enum info -> Some(info.Name, [])
        | _ -> None

    // A constraint generated from a type literal would be a different type than the one of the declaration
    let hasGeneratedConstraint =
        match declared with
        | Some(_, typeParameters) -> not (ReExport.canForwardConstraints typeParameters)
        | None -> false

    match declared with
    | None -> []
    | Some _ when hasGeneratedConstraint ->
        context.AddWarning
            $"%s{reExport.Name} is not re-exported because a type parameter constraint can't be forwarded, use the declaration in its module instead"

        []
    | Some(originalName, typeParameters) ->
        let name, context = sanitizeTypeNameAndPushScope reExport.Name context
        let typeParameters = transformDeclarationTypeParameters context typeParameters

        let reExportAlias =
            ({
                Attributes = []
                XmlDoc = []
                Name = name
                Type =
                    ({
                        Name = Naming.sanitizeTypeName originalName
                        FullName = originalName
                        ModulePath = reExport.ModulePath
                        TypeArguments =
                            typeParameters.TypeParameters
                            |> List.map (
                                function
                                | FSharpTypeParameter.FSharpTypeParameter info ->
                                    FSharpType.TypeParameter info.Name
                                | FSharpTypeParameter.FSharpType typ -> typ
                            )
                        Type = FSharpType.Discard
                    }
                    : FSharpTypeReference)
                    |> FSharpType.TypeReference
                TypeParameters = typeParameters.TypeParameters
            }
            : FSharpTypeAlias)
            |> FSharpType.TypeAlias

        reExportAlias
        :: (typeParameters.TypeParameters |> List.rev |> exposeSpecializedAlias name [] [])

let private transformModuleDeclaration
    (typeMemory: GlueType list)
    (state: TransformState)
    (reporter: Reporter)
    (typeLiteralsMemory: TypeLiteralsMemory)
    (importSource: ImportSource)
    (moduleDeclaration: GlueModuleDeclaration)
    : FSharpType list
    =
    if moduleDeclaration.Types.IsEmpty then
        []
    elif moduleDeclaration.IsGlobal then
        transform
            typeMemory
            state
            reporter
            typeLiteralsMemory
            ImportSource.Global
            true
            moduleDeclaration.Types
    else
        // If the module is a top level module we add a suffix to avoid conflicts when
        // trying to access a type from the F# module directly.
        //
        // We only do it on the top level module to try to reduce the amount of code suffixed
        // as this is not great looking
        let moduleSuffix =
            if moduleDeclaration.IsTopLevel then
                "_"
            else
                ""

        let name =
            Naming.sanitizeTypeName (
                Naming.removeSurroundingQuotes moduleDeclaration.Name + moduleSuffix
            )

        typeLiteralsMemory.EnterModule name

        let types =
            transform
                typeMemory
                state
                reporter
                typeLiteralsMemory
                importSource
                false
                moduleDeclaration.Types

        typeLiteralsMemory.LeaveModule()

        ({
            Name = name
            IsRecursive = moduleDeclaration.IsRecursive
            ImportSpecifier = None
            Types = types
        }
        : FSharpModule)
        |> FSharpType.Module
        |> List.singleton

let rec private exposeSpecializedAlias
    (name: string)
    (acc: FSharpType list)
    (tailedTypeParameters: FSharpTypeParameter list)
    (typeParameters: FSharpTypeParameter list)
    =
    match typeParameters with
    | head :: tail ->
        match head with
        | FSharpTypeParameter.FSharpType _ -> acc
        | FSharpTypeParameter.FSharpTypeParameter typeParameter ->
            match typeParameter.Default with
            | None -> acc
            | Some defaultType ->
                let orderedTail = tail |> List.rev

                // The default of a later type parameter can refer to this one (`TOut = TIn | undefined`)
                let tailedTypeParameters =
                    let substitute =
                        TypeParameter.mapFSharpType
                            [
                                {
                                    TypeParameterName = typeParameter.Name
                                    FSharpType = defaultType
                                }
                            ]

                    tailedTypeParameters
                    |> List.map (
                        function
                        | FSharpTypeParameter.FSharpType typ ->
                            FSharpTypeParameter.FSharpType(substitute typ)
                        | other -> other
                    )

                let typeAlias =
                    ({
                        Attributes = []
                        XmlDoc = []
                        Name = name
                        Type =
                            ({
                                Name = name
                                TypeParameters =
                                    orderedTail
                                    @ FSharpTypeParameter.FSharpType defaultType
                                      :: tailedTypeParameters
                            }
                            : FSharpMapped)
                            |> FSharpType.Mapped
                        TypeParameters = orderedTail
                    }
                    : FSharpTypeAlias)
                    |> FSharpType.TypeAlias

                // The type parameters are walked from the last one, its default comes first
                let newTailedTypeParameters =
                    FSharpTypeParameter.FSharpType defaultType :: tailedTypeParameters

                exposeSpecializedAlias name (acc @ [ typeAlias ]) newTailedTypeParameters tail
    | [] -> acc

let private transformClassDeclaration
    (context: TransformContext)
    (classDeclaration: GlueClassDeclaration)
    : FSharpType list
    =
    let name, context = sanitizeTypeNameAndPushScope classDeclaration.Name context

    let classDeclaration =
        let heritageClauses, members =
            withoutRedeclaredBases
                context.TypeMemory
                classDeclaration.Members
                classDeclaration.HeritageClauses

        { classDeclaration with
            HeritageClauses = heritageClauses
            Members = members
        }

    let typeParametersResult =
        transformDeclarationTypeParameters context classDeclaration.TypeParameters

    let errorInheritance, otherInheritance =
        classDeclaration.HeritageClauses
        |> List.filter (not << isSelfHeritage classDeclaration.FullName)
        |> List.partition (fun heritageClause ->
            match heritageClause with
            | GlueType.TypeReference typeReference ->
                typeReference.IsStandardLibrary && typeReference.Name = "Error"
            | _ -> false
        )

    // A class extending an `Error` subclass has to be a class too
    let rec isErrorDerived (visited: string list) (heritageClause: GlueType) =
        match heritageClause with
        | GlueType.TypeReference typeReference ->
            (typeReference.IsStandardLibrary && typeReference.Name = "Error")
            || (not (List.contains typeReference.Name visited)
                && context.TypeMemory
                   |> List.exists (
                       function
                       | GlueType.ClassDeclaration info when info.Name = typeReference.Name ->
                           info.HeritageClauses
                           |> List.exists (isErrorDerived (typeReference.Name :: visited))
                       | _ -> false
                   ))
        | _ -> false

    let hasErrorInheritance =
        not (List.isEmpty errorInheritance)
        || otherInheritance |> List.exists (isErrorDerived [])

    let inheritance =
        [
            if not (List.isEmpty errorInheritance) then
                {
                    Name = "Exception"
                    FullName = "System.Exception"
                    ModulePath = []
                    TypeArguments = []
                    IsStandardLibrary = true
                }
                |> GlueType.TypeReference
            yield! otherInheritance
        ]
        |> List.filter (
            function
            | GlueType.Discard -> false
            | heritageClause -> not (isArrayHeritage heritageClause)
        )
        |> List.map (context.ExposeTypeAlias >> transformType (context.PushScope "Extends"))
        |> List.filter isInheritableType
        |> List.distinct

    let specialiazedAlias =
        let defaultAliases =
            typeParametersResult.TypeParameters
            |> List.rev
            |> exposeSpecializedAlias name [] []

        let sealedAlias =
            typeParametersResult
            |> makeSealedTypeAlias
                name
                (aliasArities typeParametersResult.TypeParameters defaultAliases)
            |> Option.toList

        defaultAliases @ sealedAlias

    let xmlDoc = transformComment classDeclaration.Documentation

    let classDefinition =
        ({
            Attributes =
                [
                    FSharpAttribute.AllowNullLiteral

                    if hasErrorInheritance then
                        FSharpAttribute.AbstractClass
                        // A type test compiles to `instanceof`, which needs the imported class
                        yield! importAttribute classDeclaration.Name context.ImportSource
                    else
                        FSharpAttribute.Interface

                    yield! xmlDoc.ObsoleteAttributes
                ]
            XmlDoc = xmlDoc.XmlDoc
            Name = name
            OriginalName = classDeclaration.Name
            Members =
                classDeclaration.Members
                |> withoutBaseClassProperties context.TypeMemory classDeclaration.HeritageClauses
                |> CallableProperties.asMethods context.TypeMemory
                |> Conditionals.resolveMembers context.State.Conditionals
                |> TransformMembers.toFSharpMember context
            TypeParameters = typeParametersResult.TypeParameters
            Inheritance = inheritance
        }
        : FSharpInterface)
        |> FSharpType.Interface

    classDefinition :: specialiazedAlias

/// The delegate of `setData: (data: string) => void` is the `Options.setData` the map exposed
let private keysMemberTransform (scope: TransformContext) (memberName: string) =
    let _, memberScope = sanitizeMemberNameAndPushScope false memberName scope
    transformType memberScope

/// `Options.Keys` when a method constrains a type parameter by `keyof Options`
let private keysModuleOf
    (context: TransformContext)
    (interfaceInfo: GlueInterface)
    (fsharpInterface: FSharpInterface)
    : FSharpType list
    =
    if KeyOfMaps.isMapNamed context.State.KeyOfMaps interfaceInfo.FullName interfaceInfo.Name then
        // `HTMLElementEventMap` inherits most of its keys
        let members =
            ParamObjectCandidate.tryResolveMembers context.TypeMemory interfaceInfo
            |> Option.defaultValue interfaceInfo.Members

        [
            KeyOfMaps.keysModule
                (keysMemberTransform (context.PushScope fsharpInterface.Name))
                fsharpInterface.Name
                members
        ]
    else
        []

let private transformToFsharp
    (context: TransformContext)
    (glueTypes: GlueType list)
    : FSharpType list
    =
    context.SiblingTypeNames <-
        glueTypes
        |> List.choose (
            function
            | GlueType.Interface info -> Some info.Name
            | GlueType.ClassDeclaration info -> Some info.Name
            | GlueType.TypeAliasDeclaration info -> Some info.Name
            | _ -> None
        )
        |> List.map Naming.sanitizeTypeName
        |> set

    glueTypes
    |> List.collect (
        function

        | GlueType.Interface interfaceInfo when
            ParamObjectCandidate.isCandidate context.TypeMemory interfaceInfo
            ->
            let fsharpInterface = transformInterface context interfaceInfo

            // `Create` takes the inherited properties too, the interface keeps the `inherit`
            let members =
                ParamObjectCandidate.tryResolveMembers context.TypeMemory interfaceInfo
                |> Option.defaultValue interfaceInfo.Members

            let returnType =
                ({
                    Name = fsharpInterface.Name
                    TypeParameters = fsharpInterface.TypeParameters
                }
                : FSharpMapped)
                |> FSharpType.Mapped

            let creates =
                paramObjectCreateMembers (context.PushScope fsharpInterface.Name) returnType members

            FSharpType.Interface
                { fsharpInterface with
                    Members = fsharpInterface.Members @ creates
                }
            :: keysModuleOf context interfaceInfo fsharpInterface

        | GlueType.Interface interfaceInfo ->
            match tryTransformCallableInterface context interfaceInfo with
            | Some delegateType -> delegateType |> List.singleton
            | None ->
                let fsharpInterface = transformInterface context interfaceInfo

                FSharpType.Interface fsharpInterface
                :: keysModuleOf context interfaceInfo fsharpInterface

        | GlueType.Enum enumInfo -> transformEnum enumInfo |> List.singleton

        | GlueType.TypeAliasDeclaration typeAliasInfo ->
            let fsharpType = transformTypeAliasDeclaration context typeAliasInfo

            [
                fsharpType

                match fsharpType, typeAliasInfo.Type with
                | FSharpType.Interface fsharpInterface, GlueType.TypeLiteral { Members = members }
                | FSharpType.Interface fsharpInterface, GlueType.IntersectionType members when
                    KeyOfMaps.isMap context.State.KeyOfMaps typeAliasInfo.FullName
                    ->
                    KeyOfMaps.keysModule
                        (keysMemberTransform (context.PushScope fsharpInterface.Name))
                        fsharpInterface.Name
                        members
                | _ -> ()
            ]

        | GlueType.ModuleDeclaration moduleInfo ->
            transformModuleDeclaration
                context.TypeMemory
                context.State
                context._Reporter
                context.TypeLiteralsMemory
                context.ImportSource
                moduleInfo

        | GlueType.FileModule fileModule ->
            let name = Naming.sanitizeTypeName fileModule.Name
            context.TypeLiteralsMemory.EnterFileModule name

            let types =
                transform
                    context.TypeMemory
                    context.State
                    context._Reporter
                    context.TypeLiteralsMemory
                    (if fileModule.IsGlobal then
                         ImportSource.Global
                     elif not fileModule.HasRuntime then
                         ImportSource.NoRuntime
                     else
                         ImportSource.Module(
                             fileModule.ImportSpecifier,
                             fileModule.SymbolSpecifiers
                         ))
                    true
                    fileModule.Types

            context.TypeLiteralsMemory.LeaveModule()

            ({
                Name = name
                IsRecursive = false
                ImportSpecifier = Some fileModule.ImportSpecifier
                Types = types @ aggregatedExports types
            }
            : FSharpModule)
            |> FSharpType.Module
            |> List.singleton

        | GlueType.ReExport reExport -> transformReExport context reExport

        | GlueType.ClassDeclaration classInfo -> transformClassDeclaration context classInfo

        | GlueType.ExportDefault exportedType ->
            match exportedType with
            | GlueType.ClassDeclaration classInfo -> transformClassDeclaration context classInfo
            | GlueType.ModuleDeclaration moduleInfo ->
                transformModuleDeclaration
                    context.TypeMemory
                    context.State
                    context._Reporter
                    context.TypeLiteralsMemory
                    context.ImportSource
                    moduleInfo
            | _ -> FSharpType.Discard |> List.singleton

        | GlueType.ConstructorType _
        | GlueType.MappedType _
        | GlueType.FunctionType _
        | GlueType.TypeParameter _
        | GlueType.Array _
        | GlueType.TypeReference _
        | GlueType.FunctionDeclaration _
        | GlueType.IndexedAccessType _
        | GlueType.Union _
        | GlueType.Literal _
        | GlueType.Variable _
        | GlueType.Primitive _
        | GlueType.Unknown
        | GlueType.KeyOf _
        | GlueType.Discard
        | GlueType.TupleType _
        | GlueType.IntersectionType _
        | GlueType.TypeLiteral _
        | GlueType.OptionalType _
        | GlueType.NamedTupleType _
        | GlueType.TemplateLiteral
        | GlueType.UtilityType _
        | GlueType.ReadOnly _
        | GlueType.ConditionalType _
        | GlueType.IntersectionOfReferences _
        | GlueType.ThisType _ -> FSharpType.Discard |> List.singleton
    )

let private transform
    (typeMemory: GlueType list)
    (state: TransformState)
    (reporter: Reporter)
    (typeLiteralsMemory: TypeLiteralsMemory)
    (importSource: ImportSource)
    (isTopLevel: bool)
    (glueAst: GlueType list)
    : FSharpType list
    =
    // `export = e` with `declare namespace e { interface Request {} }`: the namespace is the
    // module, its types are reachable at the top level too
    let glueAst =
        let exportEqualsNames =
            glueAst
            |> List.choose (
                function
                | GlueType.ExportDefault(GlueType.Variable { Name = name }) ->
                    Some(name.Replace("export=", ""))
                | _ -> None
            )
            |> set

        let typeName (glueType: GlueType) =
            match glueType with
            | GlueType.Interface info -> Some info.Name
            | GlueType.TypeAliasDeclaration info -> Some info.Name
            | GlueType.Enum info -> Some info.Name
            | _ -> None

        let declaredNames =
            glueAst
            |> List.choose (
                function
                | GlueType.ClassDeclaration info -> Some info.Name
                | GlueType.ExportDefault(GlueType.ClassDeclaration info) -> Some info.Name
                | glueType -> typeName glueType
            )
            |> set

        let hoisted =
            glueAst
            |> List.collect (
                function
                | GlueType.ModuleDeclaration moduleInfo when
                    exportEqualsNames.Contains moduleInfo.Name
                    ->
                    let modulePath =
                        [
                            Naming.sanitizeTypeName (
                                Naming.removeSurroundingQuotes moduleInfo.Name
                                + (if moduleInfo.IsTopLevel then
                                       "_"
                                   else
                                       "")
                            )
                        ]

                    moduleInfo.Types
                    |> List.choose (fun glueType ->
                        match typeName glueType with
                        | Some name when not (declaredNames.Contains name) ->
                            Some(
                                GlueType.ReExport
                                    {
                                        Name = name
                                        Declaration = glueType
                                        ModulePath = modulePath
                                    }
                            )
                        | _ -> None
                    )
                | _ -> []
            )

        glueAst @ hoisted

    // A re-exported value is exported again under its new name
    let glueAst =
        glueAst
        |> List.map (fun glueType ->
            match glueType with
            | GlueType.ReExport {
                                    Name = name
                                    Declaration = GlueType.FunctionDeclaration info
                                } -> GlueType.FunctionDeclaration { info with Name = name }
            | GlueType.ReExport {
                                    Name = name
                                    Declaration = GlueType.Variable info
                                } -> GlueType.Variable { info with Name = name }
            | _ -> glueType
        )

    // A re-exported class only gets its constructors, its type is an alias
    let reExportedClasses =
        glueAst
        |> List.choose (fun glueType ->
            match glueType with
            | GlueType.ReExport {
                                    Name = name
                                    Declaration = GlueType.ClassDeclaration info
                                } when ReExport.canForwardConstraints info.TypeParameters ->
                Some(GlueType.ClassDeclaration { info with Name = name })
            | _ -> None
        )

    let exports, rest =
        glueAst
        |> List.partition (fun glueType ->
            match glueType with
            | GlueType.Variable _
            | GlueType.FunctionDeclaration _ -> true
            | GlueType.ExportDefault exportedType ->
                // We don't want to capture class definition here for example
                // Because we need to generate both the class bindings and the exports
                match exportedType with
                | GlueType.Variable _
                | GlueType.FunctionDeclaration _ -> true
                | _ -> false
            | _ -> false
        )

    let classes =
        let rec applyModuleDeclaration (info: GlueModuleDeclaration) : bool =
            if info.Types.IsEmpty then
                false
            else
                info.Types
                |> List.exists (fun glueType ->
                    match glueType with
                    | GlueType.ClassDeclaration _
                    | GlueType.Variable _
                    | GlueType.FunctionDeclaration _ -> true
                    | GlueType.ModuleDeclaration info -> applyModuleDeclaration info
                    | _ -> false
                )

        rest
        |> List.filter (fun glueType ->
            match glueType with
            | GlueType.ClassDeclaration _ -> true
            | GlueType.Variable _ -> true
            | GlueType.ModuleDeclaration info ->
                not info.IsGlobal && info.IsExported && applyModuleDeclaration info
            | GlueType.ExportDefault exportedType ->
                // Capture default export of classes here so we can keep
                // generate their actual bindings
                match exportedType with
                | GlueType.ClassDeclaration _ -> true
                | _ -> false
            | _ -> false
        )

    let exports =
        match importSource with
        | ImportSource.NoRuntime -> []
        | ImportSource.Global
        | ImportSource.Module _ -> exports @ classes @ reExportedClasses

    let rootTransformContext =
        TransformContext(reporter, "", typeMemory, state, typeLiteralsMemory, importSource)

    // A type test compiles to `instanceof`, which needs the class: the imported or global one
    let typeTestAttributes =
        let classDeclarations =
            glueAst
            |> List.choose (
                function
                | GlueType.ClassDeclaration info
                | GlueType.ExportDefault(GlueType.ClassDeclaration info) -> Some info
                | _ -> None
            )

        let defaultExported =
            glueAst
            |> List.choose (
                function
                | GlueType.ExportDefault(GlueType.ClassDeclaration info) -> Some info.Name
                | GlueType.ExportDefault(GlueType.Variable { Name = name }) when
                    classDeclarations |> List.exists (fun info -> info.Name = name)
                    ->
                    Some name
                | _ -> None
            )
            |> set

        let classes =
            classDeclarations
            |> List.filter (fun info -> info.IsExported || importSource = ImportSource.Global)
            |> List.map _.Name
            |> set

        let constructorVariables =
            glueAst
            |> List.choose (
                function
                | GlueType.Variable {
                                        Name = name
                                        Type = GlueType.ConstructorType _
                                    } -> Some name
                | GlueType.Variable {
                                        Name = name
                                        Type = GlueType.TypeLiteral { Members = members }
                                    } when
                    members
                    |> List.exists (
                        function
                        | GlueMember.ConstructSignature _ -> true
                        | _ -> false
                    )
                    ->
                    Some name
                | _ -> None
            )
            |> set

        fun (name: string) ->
            if not isTopLevel then
                []
            elif defaultExported.Contains name then
                importDefaultAttribute name importSource
            elif classes.Contains name || constructorVariables.Contains name then
                importAttribute name importSource
            else
                []

    let hasTypeTestAttribute (attributes: FSharpAttribute list) =
        attributes
        |> List.exists (
            function
            | FSharpAttribute.Import _
            | FSharpAttribute.ImportDefault _
            | FSharpAttribute.ImportAll _
            | FSharpAttribute.Global _ -> true
            | _ -> false
        )

    let rest =
        transformToFsharp rootTransformContext rest
        |> List.map (
            function
            | FSharpType.Interface info when not (hasTypeTestAttribute info.Attributes) ->
                match typeTestAttributes info.OriginalName with
                | [] -> FSharpType.Interface info
                | attributes ->
                    FSharpType.Interface
                        { info with
                            Attributes = info.Attributes @ attributes
                        }
            | fsharpType -> fsharpType
        )

    let exportsType =
        if List.isEmpty exports then
            None
        else
            let exportTransforms: Exports.Transforms =
                {
                    Type = transformType
                    Parameter = transformParameter
                    TypeParameters = transformTypeParameters
                    DefaultedTypeParameterOverloadsOfFunction =
                        TransformMembers.withDefaultedTypeParameterOverloadsOfFunction
                    DictionaryParameterOverloadsOfFunction =
                        TransformMembers.withDictionaryParameterOverloadsOfFunction
                }

            match
                Exports.transformExports exportTransforms rootTransformContext isTopLevel exports
            with
            | FSharpType.Interface { Members = [] } -> None
            | exportsType -> Some exportsType

    [
        // These are the exported functions, classes, etc. from the binding
        yield! Option.toList exportsType

        // "Standard" types which are a direct mapping to a TypeScript type
        yield! rest

        // Output the exposed types
        // Exposed types are that don't directly map to a TypeScript type
        // which we generate to improve the user experience. For example,
        // this is used when we have a literal type as an argument of a function / method
        yield! rootTransformContext.ToList()
    ]

type TransformResult =
    {
        FSharpAST: FSharpType list
        Warnings: ResizeArray<string>
        Errors: ResizeArray<string>
        IncludeRegExpAlias: bool
        IncludeReadonlyArrayAlias: bool
        IncludeIterableAlias: bool
    }

let apply (typeMemory: GlueType list) (glueAst: GlueType list) =
    applyWith Naming.MODULE_PLACEHOLDER typeMemory glueAst

/// What a run of the transform is given besides the declarations
type TransformOptions =
    {
        Source: ImportSource
        /// The overloads a signature gets at most from its union parameters
        MaxOverloads: int
    }

let applyWithOptions
    (options: TransformOptions)
    (typeMemory: GlueType list)
    (glueAst: GlueType list)
    =
    let reporter = Reporter()
    let typeLiteralsMemory = TypeLiteralsMemory()
    let state = TransformState.Create(typeMemory, options.MaxOverloads)

    let transformed =
        transform typeMemory state reporter typeLiteralsMemory options.Source true glueAst

    let aliases = Merge.aliasesOf transformed

    {
        FSharpAST =
            transformed
            |> Merge.applyWith aliases
            |> Abbreviations.apply
            |> FreeTypeParameters.apply
            |> DelegateExtensions.apply
            |> Merge.disambiguateOverloads aliases
        Warnings = reporter.Warnings
        Errors = reporter.Errors
        IncludeRegExpAlias = reporter.HasRegEpx
        IncludeReadonlyArrayAlias = reporter.HasReadonlyArray
        IncludeIterableAlias = reporter.HasIterable
    }

let applyWithSource (source: ImportSource) (typeMemory: GlueType list) (glueAst: GlueType list) =
    applyWithOptions
        {
            Source = source
            MaxOverloads = UnionOverloads.defaultMaxOverloads
        }
        typeMemory
        glueAst

let applyWith (importSpecifier: string) (typeMemory: GlueType list) (glueAst: GlueType list) =
    applyWithSource (ImportSource.Module(importSpecifier, Map.empty)) typeMemory glueAst
