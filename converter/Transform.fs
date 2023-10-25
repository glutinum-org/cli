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

let rec private transformType (glueType: GlueType) : FSharpType =
    match glueType with
    | GlueType.Primitive primitiveInfo ->
        match primitiveInfo with
        | GluePrimitive.String -> FSharpType.Primitive FSharpPrimitive.String
        | GluePrimitive.Int -> FSharpType.Primitive FSharpPrimitive.Int
        | GluePrimitive.Float -> FSharpType.Primitive FSharpPrimitive.Float
        | GluePrimitive.Bool -> FSharpType.Primitive FSharpPrimitive.Bool
        | GluePrimitive.Unit -> FSharpType.Primitive FSharpPrimitive.Unit
        | GluePrimitive.Number -> FSharpType.Primitive FSharpPrimitive.Number
        | GluePrimitive.Any -> FSharpType.Primitive FSharpPrimitive.Null
    | GlueType.Union cases ->
        {
            Attributes = []
            Name = $"U{cases.Length}"
            Cases =
                cases
                |> List.map (fun caseType ->
                    {
                        Attributes = []
                        Name = caseType.Name
                    }
                )
        }
        |> FSharpType.Union

    | GlueType.IndexedAccessType _
    | GlueType.Literal _
    | GlueType.Interface _
    | GlueType.Enum _
    | GlueType.TypeAliasDeclaration _
    | GlueType.Variable _
    | GlueType.KeyOf _
    | GlueType.Discard -> FSharpType.Discard

/// <summary></summary>
/// <param name="exports"></param>
/// <returns></returns>
let private transformExports (exports: GlueType list) : FSharpType =
    let members =
        exports
        |> List.map (
            function
            | GlueType.Variable info ->
                {
                    Attributes = [ FSharpAttribute.Import(info.Name, "module") ]
                    Name = info.Name
                    Parameters = []
                    Type = transformType info.Type
                    IsOptional = false
                    IsStatic = true
                    Accessor = None
                    Accessibility = FSharpAccessiblity.Public
                }

            | glueType ->
                failwithf "Could not generate exportMembers for: %A" glueType
        )

    {
        Attributes = [ FSharpAttribute.Erase ]
        Name = "Exports"
        Members = members
    }
    |> FSharpType.Interface

let private transformParameter (parameter: GlueParameter) : FSharpParameter =
    {
        Name = parameter.Name
        IsOptional = parameter.IsOptional
        Type = transformType parameter.Type
    }

let private transformAccessor (accessor: GlueAccessor) : FSharpAccessor =
    match accessor with
    | GlueAccessor.ReadOnly -> FSharpAccessor.ReadOnly
    | GlueAccessor.WriteOnly -> FSharpAccessor.WriteOnly
    | GlueAccessor.ReadWrite -> FSharpAccessor.ReadWrite

let private transformInterface (info: GlueInterface) : FSharpInterface =
    let members =
        info.Members
        |> List.map (
            function
            | GlueMember.Method methodInfo ->
                {
                    Attributes = []
                    Name = methodInfo.Name
                    Parameters =
                        methodInfo.Parameters |> List.map transformParameter
                    Type = transformType methodInfo.Type
                    IsOptional = methodInfo.IsOptional
                    IsStatic = methodInfo.IsStatic
                    Accessor = None
                    Accessibility = FSharpAccessiblity.Public
                }


            | GlueMember.CallSignature callSignatureInfo ->
                {
                    Attributes = [ FSharpAttribute.EmitSelfInvoke ]
                    Name = "Invoke"
                    Parameters =
                        callSignatureInfo.Parameters
                        |> List.map transformParameter
                    Type = transformType callSignatureInfo.Type
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessiblity.Public
                }

            | GlueMember.Property propertyInfo ->
                {
                    Attributes = []
                    Name = propertyInfo.Name
                    Parameters = []
                    Type = transformType propertyInfo.Type
                    IsOptional = false
                    IsStatic = propertyInfo.IsStatic
                    Accessor = transformAccessor propertyInfo.Accessor |> Some
                    Accessibility = FSharpAccessiblity.Public
                }
        )

    {
        Attributes = [ FSharpAttribute.AllowNullLiteral ]
        Name = info.Name
        Members = members
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
                Name = glueMember.Name
                Value = transformLiteral glueMember.Value
            }

        {
            Name = glueEnum.Name
            Cases = integralValues |> List.map transformMembers
        }
        |> FSharpType.Enum

    | [], stringValues ->
        let transformMembers (glueMember: GlueEnumMember) : FSharpUnionCase =
            let caseValue =
                match glueMember.Value with
                | GlueLiteral.String value -> value
                | _ -> failwith "Should not happen"

            let caseName =
                glueMember.Name
                |> String.removeSingleQuote
                |> String.removeDoubleQuote
                |> String.capitalizeFirstLetter

            let differentName =
                Naming.nameNotEqualsDefaultFableValue caseName caseValue

            {
                Attributes =
                    [
                        if differentName then
                            FSharpAttribute.CompiledName(caseValue)
                    ]
                Name = caseName
            }

        {
            Attributes =
                [
                    FSharpAttribute.RequireQualifiedAccess
                    FSharpAttribute.StringEnum
                ]
            Name = glueEnum.Name
            Cases = stringValues |> List.map transformMembers
        }
        |> FSharpType.Union
    | _ ->
        failwith
            $"""Mix enum are not supported in F#

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
                            Naming.nameNotEqualsDefaultFableValue caseName caseValue

                        {
                            Attributes =
                                [
                                    if differentName then
                                        FSharpAttribute.CompiledName(caseName)
                                ]
                            Name = caseValue
                        }
                        : FSharpUnionCase
                        |> Some
                    // Doesn't make sense to have a case for call signature
                    | GlueMember.CallSignature _ -> None
                )
            | _ ->
                []

        ({
            Attributes =
                [
                    FSharpAttribute.RequireQualifiedAccess
                    FSharpAttribute.StringEnum
                ]
            Name = aliasName
            Cases = cases
        }
        : FSharpUnion)
        |> FSharpType.Union

let private transformTypeAliasDeclaration
    (glueTypeAliasDeclaration: GlueTypeAliasDeclaration)
    : FSharpType
    =

    // TODO: Make the transformation more robust
    match glueTypeAliasDeclaration.Types with
    | GlueType.Union cases :: [] ->
        // Unions can have nested unions, so we need to flatten them
        // TODO: Is there cases where we don't want to flatten?
        // U2<U2<int, string>, bool>
        let rec flattenCases (cases: GlueType list) : GlueType list =
            cases
            |> List.collect (
                function
                | GlueType.Union cases -> flattenCases cases
                | glueType -> [ glueType ]
            )

        let cases = flattenCases cases

        let isStringOnly =
            cases
            |> List.forall (
                function
                | GlueType.Literal(GlueLiteral.String _) -> true
                | _ -> false
            )

        let isNumericOnly =
            cases
            |> List.forall (
                function
                | GlueType.Literal(GlueLiteral.Int _) -> true
                | _ -> false
            )

        // If the union contains only literal strings,
        // we can transform it into a StringEnum
        if isStringOnly then
            let cases =
                cases
                |> List.map (fun value ->
                    match value with
                    | GlueType.Literal(GlueLiteral.String value) ->
                        let caseName =
                            value
                            |> String.removeSingleQuote
                            |> String.removeDoubleQuote
                            |> String.capitalizeFirstLetter

                        {
                            Attributes = []
                            Name = caseName
                        }
                        : FSharpUnionCase
                    | _ -> failwith "Should not happen"
                )

            ({
                Attributes =
                    [
                        FSharpAttribute.RequireQualifiedAccess
                        FSharpAttribute.StringEnum
                    ]
                Name = glueTypeAliasDeclaration.Name
                Cases = cases
            }
            : FSharpUnion)
            |> FSharpType.Union
        // If the union contains only literal numbers,
        // we can transform it into a standard F# enum
        else if isNumericOnly then
            let cases =
                cases
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

            ({
                Name = glueTypeAliasDeclaration.Name
                Cases = cases
            }
            : FSharpEnum)
            |> FSharpType.Enum
        // Otherwise, we want to generate an erased Enum
        // Either by using U2, U3, etc. or by creating custom
        // Erased enum cases for improving the user experience
        else
            // TODO
            FSharpType.Discard

    | GlueType.KeyOf glueType :: [] ->
        TypeAliasDeclaration.transformKeyOf glueTypeAliasDeclaration.Name glueType

    | GlueType.IndexedAccessType glueType :: [] ->
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
                        | GlueMember.CallSignature { Type = typ } ->
                            match typ with
                            | GlueType.Union cases -> cases
                            | _ -> [ typ ]
                    )
                    // Remove duplicates
                    |> List.distinct
                    // Wrap inside of an union, so it can be transformed as U2, U3, etc.
                    |> GlueType.Union
                    |> transformType

                | _ -> FSharpType.Discard
            | _ -> FSharpType.Discard

        ({
            Name = glueTypeAliasDeclaration.Name
            Type = typ
        }
        : FSharpTypeAlias)
        |> FSharpType.Alias

    | _ -> FSharpType.Discard

let rec private transformToFsharp (glueTypes: GlueType list) : FSharpType list =
    glueTypes
    |> List.map (
        function
        | GlueType.Interface interfaceInfo ->
            FSharpType.Interface(transformInterface interfaceInfo)

        | GlueType.Enum enumInfo -> transformEnum enumInfo

        | GlueType.TypeAliasDeclaration typeAliasInfo ->
            transformTypeAliasDeclaration typeAliasInfo

        | GlueType.IndexedAccessType _
        | GlueType.Union _
        | GlueType.Literal _
        | GlueType.Variable _
        | GlueType.Primitive _
        | GlueType.KeyOf _
        | GlueType.Discard ->
            FSharpType.Discard
    )

let transform (glueAst: GlueType list) : FSharpType list =
    let exports, rest =
        glueAst
        |> List.partition (fun glueType ->
            match glueType with
            | GlueType.Variable _ -> true
            | _ -> false
        )

    [
        if not (List.isEmpty exports) then
            transformExports exports

        yield! transformToFsharp rest
    ]
