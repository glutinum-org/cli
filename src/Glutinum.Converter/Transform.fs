module rec Glutinum.Converter.Transform

open Fable.Core
open System
open Node
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.FSharpAST
open Node.Api
open Fable.Core.JS
open Glutinum.Converter.GlueAST

let private transformLiteral (glueLiteral: GlueLiteral) : FSharpLiteral =
    match glueLiteral with
    | GlueLiteral.String value -> FSharpLiteral.String value
    | GlueLiteral.Int value -> FSharpLiteral.Int value
    | GlueLiteral.Float value -> FSharpLiteral.Float value
    | GlueLiteral.Bool value -> FSharpLiteral.Bool value

let private transformPrimitive
    (gluePrimitive: GluePrimitive)
    : FSharpPrimitive
    =
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

let rec private transformType (glueType: GlueType) : FSharpType =
    match glueType with
    | GlueType.Primitive primitiveInfo ->
        transformPrimitive primitiveInfo |> FSharpType.Primitive
    | GlueType.Union(GlueTypeUnion cases) ->
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
            FSharpType.Option(transformType others.Head)
        else
            {
                Attributes = []
                Name = $"U{others.Length}"
                Cases =
                    others
                    |> List.map (fun caseType ->
                        {
                            Attributes = []
                            Name = caseType.Name
                        }
                    )
                IsOptional = isOptional
            }
            |> FSharpType.Union

    | GlueType.TypeReference typeReference ->
        ({
            Name = typeReference.Name
            FullName = typeReference.FullName
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.Array glueType ->
        transformType glueType |> FSharpType.ResizeArray

    | GlueType.ClassDeclaration classDeclaration ->
        ({
            Name = classDeclaration.Name
            FullName = classDeclaration.Name
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.TypeParameter name -> FSharpType.TypeParameter name
    | GlueType.ModuleDeclaration _
    | GlueType.IndexedAccessType _
    | GlueType.Literal _
    | GlueType.Interface _
    | GlueType.Enum _
    | GlueType.TypeAliasDeclaration _
    | GlueType.Variable _
    | GlueType.KeyOf _
    | GlueType.Discard
    | GlueType.FunctionType _
    | GlueType.Partial _
    | GlueType.FunctionDeclaration _ ->
        printfn "Could not transform type: %A" glueType
        FSharpType.Discard

/// <summary></summary>
/// <param name="exports"></param>
/// <returns></returns>
let private transformExports
    (isTopLevel: bool)
    (exports: GlueType list)
    : FSharpType
    =
    let members =
        exports
        |> List.collect (
            function
            | GlueType.Variable info ->
                {
                    Attributes = [ FSharpAttribute.Import(info.Name, "module") ]
                    Name = Naming.escapeName info.Name
                    Parameters = []
                    Type = transformType info.Type
                    IsOptional = false
                    IsStatic = true
                    Accessor = None
                    Accessibility = FSharpAccessiblity.Public
                }
                |> FSharpMember.Property
                |> List.singleton

            | GlueType.FunctionDeclaration info ->
                {
                    Attributes = [ FSharpAttribute.Import(info.Name, "module") ]
                    Name = Naming.escapeName info.Name
                    Parameters = info.Parameters |> List.map transformParameter
                    Type = transformType info.Type
                    IsOptional = false
                    IsStatic = true
                    Accessor = None
                    Accessibility = FSharpAccessiblity.Public
                }
                |> FSharpMember.Method
                |> List.singleton

            | GlueType.ClassDeclaration info ->
                // TODO: Handle constructor overloads

                info.Constructors
                |> List.map (fun (GlueConstructor parameters) ->
                    {
                        Attributes =
                            [
                                if isTopLevel then
                                    FSharpAttribute.Import(info.Name, "module")
                                    FSharpAttribute.EmitConstructor
                                else
                                    FSharpAttribute.EmitMacroConstructor
                                        info.Name
                            ]
                        Name = Naming.escapeName info.Name
                        Parameters = parameters |> List.map transformParameter
                        Type =
                            ({
                                Name = Naming.escapeName info.Name
                                Declarations = []
                            })
                            |> FSharpType.Mapped
                        IsOptional = false
                        IsStatic = true
                        Accessor = None
                        Accessibility = FSharpAccessiblity.Public
                    }
                    |> FSharpMember.Method
                )

            | GlueType.ModuleDeclaration moduleDeclaration ->
                moduleDeclaration.Types
                |> List.choose (fun typ ->
                    match typ with
                    | GlueType.ClassDeclaration info ->
                        {
                            Attributes = [ FSharpAttribute.ImportAll "module" ]
                            // "_" suffix is added to avoid name collision if
                            // there are some functions with the same name as
                            // the name of the module
                            // TODO: Only add the "_" suffix if there is a name collision
                            Name = moduleDeclaration.Name + "_"
                            Parameters = []
                            Type =
                                ({
                                    Name = $"{moduleDeclaration.Name}.Exports"
                                    Declarations = []
                                })
                                |> FSharpType.Mapped
                            IsOptional = false
                            IsStatic = true
                            Accessor = FSharpAccessor.ReadOnly |> Some
                            Accessibility = FSharpAccessiblity.Public
                        }
                        |> FSharpMember.Property
                        |> Some
                    | _ -> None
                )

            | glueType ->
                failwithf "Could not generate exportMembers for: %A" glueType
        )

    {
        Attributes = [ FSharpAttribute.Erase ]
        Name = "Exports"
        Members = members
        TypeParameters = []
    }
    |> FSharpType.Interface

let private transformParameter (parameter: GlueParameter) : FSharpParameter =
    {
        Name = Naming.escapeName parameter.Name
        IsOptional = parameter.IsOptional
        Type = transformType parameter.Type
    }

let private transformAccessor (accessor: GlueAccessor) : FSharpAccessor =
    match accessor with
    | GlueAccessor.ReadOnly -> FSharpAccessor.ReadOnly
    | GlueAccessor.WriteOnly -> FSharpAccessor.WriteOnly
    | GlueAccessor.ReadWrite -> FSharpAccessor.ReadWrite

let private transformMembers (members: GlueMember list) : FSharpMember list =
    members
    |> List.map (
        function
        | GlueMember.Method methodInfo ->
            {
                Attributes = []
                Name = Naming.escapeName methodInfo.Name
                Parameters =
                    methodInfo.Parameters |> List.map transformParameter
                Type = transformType methodInfo.Type
                IsOptional = methodInfo.IsOptional
                IsStatic = methodInfo.IsStatic
                Accessor = None
                Accessibility = FSharpAccessiblity.Public
            }
            |> FSharpMember.Method

        | GlueMember.CallSignature callSignatureInfo ->
            {
                Attributes = [ FSharpAttribute.EmitSelfInvoke ]
                Name = "Invoke"
                Parameters =
                    callSignatureInfo.Parameters |> List.map transformParameter
                Type = transformType callSignatureInfo.Type
                IsOptional = false
                IsStatic = false
                Accessor = None
                Accessibility = FSharpAccessiblity.Public
            }
            |> FSharpMember.Method

        | GlueMember.Property propertyInfo ->
            {
                Attributes = []
                Name = Naming.escapeName propertyInfo.Name
                Parameters = []
                Type = transformType propertyInfo.Type
                IsOptional = false
                IsStatic = propertyInfo.IsStatic
                Accessor = transformAccessor propertyInfo.Accessor |> Some
                Accessibility = FSharpAccessiblity.Public
            }
            |> FSharpMember.Property

        | GlueMember.IndexSignature indexSignature ->
            {
                Attributes = [ FSharpAttribute.EmitIndexer ]
                Name = "Item"
                Parameters =
                    indexSignature.Parameters |> List.map transformParameter
                Type = transformType indexSignature.Type
                IsOptional = false
                IsStatic = false
                Accessor = Some FSharpAccessor.ReadWrite
                Accessibility = FSharpAccessiblity.Public
            }
            |> FSharpMember.Property
    )

let private transformInterface (info: GlueInterface) : FSharpInterface =
    {
        Attributes = [ FSharpAttribute.AllowNullLiteral ]
        Name = Naming.escapeName info.Name
        Members = transformMembers info.Members
        TypeParameters = transformTypeParameters info.TypeParameters
    }

let private transformEnum (glueEnum: GlueEnum) : FSharpType =
    let (integralValues, stringValues) =
        glueEnum.Members
        // Remove values enums values that are not supported by F#/Fable
        |> List.filter (fun m ->
            match m.Value with
            | GlueLiteral.Int _
            | GlueLiteral.String _ -> true
            | _ -> false
        )
        |> List.partition (fun m ->
            match m.Value with
            | GlueLiteral.Int _ -> true
            | _ -> false
        )

    match integralValues, stringValues with
    | [], [] -> failwith $"""Empty enum: {glueEnum.Name}"""
    | integralValues, [] ->
        let transformMembers (glueMember: GlueEnumMember) : FSharpEnumCase =
            {
                Name = Naming.escapeName glueMember.Name
                Value = transformLiteral glueMember.Value
            }

        {
            Name = Naming.escapeName glueEnum.Name
            Cases = integralValues |> List.map transformMembers |> List.distinct
        }
        |> FSharpType.Enum

    | [], stringValues ->
        let transformMembers (glueMember: GlueEnumMember) : FSharpUnionCase =
            let caseValue =
                match glueMember.Value with
                | GlueLiteral.String value -> value
                | _ -> failwith "Should not happen"

            let caseName = Naming.escapeName glueMember.Name

            // |> String.removeSingleQuote
            // |> String.removeDoubleQuote
            // |> String.capitalizeFirstLetter

            let differentName =
                Naming.nameNotEqualsDefaultFableValue caseName caseValue

            {
                Attributes =
                    [
                        if differentName then
                            FSharpAttribute.CompiledName(caseValue)
                    ]
                Name = Naming.escapeName caseName
            }

        {
            Attributes =
                [
                    FSharpAttribute.RequireQualifiedAccess
                    FSharpAttribute.StringEnum CaseRules.None
                ]
            Name = Naming.escapeName glueEnum.Name
            Cases = stringValues |> List.map transformMembers |> List.distinct
            IsOptional = false
        }
        |> FSharpType.Union
    | _ ->
        failwith
            $"""Mix enums are not supported in F#

Errored enum: {glueEnum.Name}
"""

module TypeAliasDeclaration =

    let transformKeyOf (aliasName: string) (glueType: GlueType) : FSharpType =
        let cases =
            match glueType with
            | GlueType.Interface interfaceInfo ->
                interfaceInfo.Members
                |> List.choose (fun m ->
                    match m with
                    | GlueMember.Method { Name = caseName }
                    | GlueMember.Property { Name = caseName } ->
                        let caseValue =
                            caseName
                            |> String.removeSingleQuote
                            |> String.removeDoubleQuote

                        let differentName =
                            Naming.nameNotEqualsDefaultFableValue
                                caseName
                                caseValue

                        {
                            Attributes =
                                [
                                    if differentName then
                                        FSharpAttribute.CompiledName(caseName)
                                ]
                            Name = Naming.escapeName caseValue
                        }
                        : FSharpUnionCase
                        |> Some
                    // Doesn't make sense to have a case for call signature
                    | GlueMember.CallSignature _
                    // Doesn't make sense to have a case for index signature
                    // because index signature is used because we don't know the name
                    // of the properties and so it is used only to describe the
                    // shape of the object
                    | GlueMember.IndexSignature _ -> None
                )
            | _ -> []

        ({
            Attributes =
                [
                    FSharpAttribute.RequireQualifiedAccess
                    FSharpAttribute.StringEnum CaseRules.None
                ]
            Name = Naming.escapeName aliasName
            Cases = cases
            IsOptional = false
        }
        : FSharpUnion)
        |> FSharpType.Union

let private transformTypeParameters
    (typeParameters: GlueTypeParameter list)
    : FSharpTypeParameter list
    =
    typeParameters
    |> List.map (fun typeParameter ->
        {
            Name = Naming.escapeName typeParameter.Name
            Constraint = typeParameter.Constraint |> Option.map transformType
            Default = typeParameter.Default |> Option.map transformType
        }
    )


let private transformTypeAliasDeclaration
    (glueTypeAliasDeclaration: GlueTypeAliasDeclaration)
    : FSharpType
    =

    // TODO: Make the transformation more robust
    match glueTypeAliasDeclaration.Type with
    | GlueType.Union(GlueTypeUnion cases) as unionType ->

        // Unions can have nested unions, so we need to flatten them
        // TODO: Is there cases where we don't want to flatten?
        // U2<U2<int, string>, bool>
        let rec flattenCases (cases: GlueType list) : GlueType list =
            cases
            |> List.collect (
                function
                // We are inside an union, and have access to the literal types
                | GlueType.Literal _ as literal -> [ literal ]
                | GlueType.Union(GlueTypeUnion cases) -> flattenCases cases
                | GlueType.TypeAliasDeclaration aliasCases ->
                    match aliasCases.Type with
                    | GlueType.Union(GlueTypeUnion cases) -> flattenCases cases
                    | _ -> failwith "Should not happen"
                // Can't find cases so we return an empty list to discard the type
                // Should we do something if we fall in this state?
                // I think the code below will be able to recover by generating
                // an erased enum, but I don't know if there cases where we could
                // be bitting ourselves in the foot
                | _ -> []
            )

        let flattenedCases = flattenCases cases

        let isStringOnly =
            // If the list is empty, it means that there was no candidates
            // for string literals
            not flattenedCases.IsEmpty
            && flattenedCases
               |> List.forall (
                   function
                   | GlueType.Literal(GlueLiteral.String _) -> true
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

        // If the union contains only literal strings,
        // we can transform it into a StringEnum
        if isStringOnly then
            let cases =
                flattenedCases
                |> List.map (fun value ->
                    match value with
                    | GlueType.Literal(GlueLiteral.String value) ->
                        let caseName =
                            value
                            |> String.removeSingleQuote
                            |> String.removeDoubleQuote
                        // |> String.capitalizeFirstLetter

                        {
                            Attributes = []
                            Name = Naming.escapeName caseName
                        }
                        : FSharpUnionCase
                    | _ -> failwith "Should not happen"
                )
                |> List.distinct

            ({
                Attributes =
                    [
                        FSharpAttribute.RequireQualifiedAccess
                        FSharpAttribute.StringEnum CaseRules.None
                    ]
                Name = Naming.escapeName glueTypeAliasDeclaration.Name
                Cases = cases
                IsOptional = false
            }
            : FSharpUnion)
            |> FSharpType.Union
        // If the union contains only literal numbers,
        // we can transform it into a standard F# enum
        else if isNumericOnly then
            let cases =
                flattenedCases
                |> List.map (fun value ->
                    match value with
                    | GlueType.Literal(GlueLiteral.Int value) ->
                        {
                            Name = value.ToString()
                            Value = FSharpLiteral.Int value
                        }
                        : FSharpEnumCase
                    | _ -> failwith "Should not happen"
                )
                |> List.distinct

            ({
                Name = Naming.escapeName glueTypeAliasDeclaration.Name
                Cases = cases
            }
            : FSharpEnum)
            |> FSharpType.Enum
        // Otherwise, we want to generate an erased Enum
        // Either by using U2, U3, etc. or by creating custom
        // Erased enum cases for improving the user experience
        else
            ({
                Name = Naming.escapeName glueTypeAliasDeclaration.Name
                Type = transformType unionType
                TypeParameters =
                    transformTypeParameters
                        glueTypeAliasDeclaration.TypeParameters
            }
            : FSharpTypeAlias)
            |> FSharpType.TypeAlias

    | GlueType.KeyOf glueType ->
        TypeAliasDeclaration.transformKeyOf
            glueTypeAliasDeclaration.Name
            glueType

    | GlueType.IndexedAccessType glueType ->
        let typ =
            match glueType with
            | GlueType.KeyOf glueType ->
                match glueType with
                | GlueType.Interface interfaceInfo ->
                    interfaceInfo.Members
                    // Flatten all the types
                    |> List.collect (fun m ->
                        match m with
                        | GlueMember.Method { Type = typ }
                        | GlueMember.Property { Type = typ }
                        | GlueMember.CallSignature { Type = typ }
                        | GlueMember.IndexSignature { Type = typ } ->
                            match typ with
                            | GlueType.Union(GlueTypeUnion cases) -> cases
                            | _ -> [ typ ]
                    )
                    // Remove duplicates
                    |> List.distinct
                    // Wrap inside of an union, so it can be transformed as U2, U3, etc.
                    |> GlueTypeUnion
                    |> GlueType.Union
                    |> transformType

                | _ -> FSharpType.Discard
            | _ -> FSharpType.Discard

        ({
            Name = Naming.escapeName glueTypeAliasDeclaration.Name
            Type = typ
            TypeParameters =
                transformTypeParameters glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    | GlueType.Primitive primitiveInfo ->
        ({
            Name = Naming.escapeName glueTypeAliasDeclaration.Name
            Type = transformPrimitive primitiveInfo |> FSharpType.Primitive
            TypeParameters =
                transformTypeParameters glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    | GlueType.TypeReference typeReference ->
        ({
            Name = Naming.escapeName glueTypeAliasDeclaration.Name
            Type = transformType (GlueType.TypeReference typeReference)
            TypeParameters =
                transformTypeParameters glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    | GlueType.Array glueType ->
        ({
            Name = Naming.escapeName glueTypeAliasDeclaration.Name
            Type = transformType (GlueType.Array glueType)
            TypeParameters =
                transformTypeParameters glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    | GlueType.Partial interfaceInfo ->
        let originalInterface = transformInterface interfaceInfo

        // Adapt the original interface to make it partial
        let partialInterface =
            { originalInterface with
                // Use the alias name instead of the original interface name
                Name = Naming.escapeName glueTypeAliasDeclaration.Name
                // Transform all the members to optional
                Members =
                    originalInterface.Members
                    |> List.map (fun m ->
                        match m with
                        | FSharpMember.Method method ->
                            { method with IsOptional = true }
                            |> FSharpMember.Method
                        | FSharpMember.Property property ->
                            { property with IsOptional = true }
                            |> FSharpMember.Property
                    )
            }

        FSharpType.Interface partialInterface

    | GlueType.FunctionType functionType ->
        {
            Attributes = [ FSharpAttribute.AllowNullLiteral ]
            Name = Naming.escapeName glueTypeAliasDeclaration.Name
            TypeParameters =
                transformTypeParameters glueTypeAliasDeclaration.TypeParameters
            Members =
                {
                    Attributes = [ FSharpAttribute.EmitSelfInvoke ]
                    Name = "Invoke"
                    Parameters =
                        functionType.Parameters |> List.map transformParameter
                    Type = transformType functionType.Type
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessiblity.Public
                }
                |> FSharpMember.Method
                |> List.singleton
        }
        |> FSharpType.Interface

    | _ -> FSharpType.Discard

let private transformModuleDeclaration
    (moduleDeclaration: GlueModuleDeclaration)
    : FSharpType
    =
    ({
        Name = Naming.escapeName moduleDeclaration.Name
        IsRecursive = moduleDeclaration.IsRecursive
        Types = transform false moduleDeclaration.Types
    }
    : FSharpModule)
    |> FSharpType.Module

let private transformClassDeclaration
    (classDeclaration: GlueClassDeclaration)
    : FSharpType
    =
    ({
        Attributes = [ FSharpAttribute.AllowNullLiteral ]
        Name = Naming.escapeName classDeclaration.Name
        Members = transformMembers classDeclaration.Members
        TypeParameters = transformTypeParameters classDeclaration.TypeParameters
    }
    : FSharpInterface)
    |> FSharpType.Interface

let rec private transformToFsharp (glueTypes: GlueType list) : FSharpType list =
    glueTypes
    |> List.map (
        function
        | GlueType.Interface interfaceInfo ->
            FSharpType.Interface(transformInterface interfaceInfo)

        | GlueType.Enum enumInfo -> transformEnum enumInfo

        | GlueType.TypeAliasDeclaration typeAliasInfo ->
            transformTypeAliasDeclaration typeAliasInfo

        | GlueType.ModuleDeclaration moduleInfo ->
            transformModuleDeclaration moduleInfo

        | GlueType.ClassDeclaration classInfo ->
            transformClassDeclaration classInfo

        | GlueType.TypeParameter _
        | GlueType.FunctionType _
        | GlueType.Partial _
        | GlueType.Array _
        | GlueType.TypeReference _
        | GlueType.FunctionDeclaration _
        | GlueType.IndexedAccessType _
        | GlueType.Union _
        | GlueType.Literal _
        | GlueType.Variable _
        | GlueType.Primitive _
        | GlueType.KeyOf _
        | GlueType.Discard -> FSharpType.Discard
    )

let transform (isTopLevel: bool) (glueAst: GlueType list) : FSharpType list =
    let exports, rest =
        glueAst
        |> List.partition (fun glueType ->
            match glueType with
            | GlueType.Variable _
            | GlueType.FunctionDeclaration _ -> true
            | _ -> false
        )

    let classes =
        rest
        |> List.filter (fun glueType ->
            match glueType with
            | GlueType.ClassDeclaration _ -> true
            | GlueType.ModuleDeclaration info ->
                info.Types
                |> List.exists (fun glueType ->
                    match glueType with
                    | GlueType.ClassDeclaration _ -> true
                    | _ -> false
                )
            | _ -> false
        )

    // let oneLevelDeeperClasses =
    //     rest
    //     |> List.choose (fun glueType ->
    //         match glueType with
    //         | GlueType.ModuleDeclaration _ -> true
    //         | _ -> false
    //     )

    let exports = exports @ classes

    [
        if not (List.isEmpty exports) then
            transformExports isTopLevel exports

        yield! transformToFsharp rest
    ]
