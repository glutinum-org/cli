module Glutinum.Converter.Transformer.Enum

open Fable.Core
open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Transformer.Utils

let transformEnum (glueEnum: GlueEnum) : FSharpType =
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
    | [], [] ->
        {
            XmlDoc = []
            Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
            Name = Naming.sanitizeTypeName glueEnum.Name
            OriginalName = glueEnum.Name
            TypeParameters = []
            Members = []
            Inheritance = []
        }
        |> FSharpType.Interface
    | integralValues, [] ->
        let transformMembers (glueMember: GlueEnumMember) : FSharpEnumCase =
            {
                Name = Naming.sanitizeTypeName glueMember.Name
                Value = transformLiteral glueMember.Value
            }

        {
            Name = Naming.sanitizeTypeName glueEnum.Name
            Cases = integralValues |> List.map transformMembers |> List.distinct
        }
        |> FSharpType.Enum

    | [], stringValues ->
        let transformMembers (glueMember: GlueEnumMember) : FSharpUnionCase =
            let caseValue =
                match glueMember.Value with
                | GlueLiteral.String value -> value
                | _ -> failwith "Should not happen"

            let caseName = Naming.sanitizeTypeName glueMember.Name

            {
                Attributes =
                    [
                        if caseName <> caseValue then
                            caseValue
                            |> Naming.removeSurroundingQuotes
                            |> FSharpAttribute.CompiledName
                    ]
                Name = caseName
            }
            |> FSharpUnionCase.Named

        {
            Attributes =
                [
                    FSharpAttribute.RequireQualifiedAccess
                    FSharpAttribute.StringEnum CaseRules.None
                ]
            Name = Naming.sanitizeTypeName glueEnum.Name
            Cases = stringValues |> List.map transformMembers |> List.distinct
            IsOptional = false
            TypeParameters = []
            Constants = []
        }
        |> FSharpType.Union
    // `enum Mixed { A = "a", B = 2 }`: an erased union of the value kinds, the members are constants
    | _ ->
        let name = Naming.sanitizeTypeName glueEnum.Name

        let constants =
            glueEnum.Members
            |> List.choose (fun glueMember ->
                let constant case value =
                    Some
                        {
                            Name = Naming.sanitizeTypeName glueMember.Name
                            Case = case
                            Value = value
                        }

                match glueMember.Value with
                | GlueLiteral.String value ->
                    constant "String" (Naming.removeSurroundingQuotes value |> sprintf "%A")
                | GlueLiteral.Int value -> constant "Number" $"{value}.0"
                | GlueLiteral.Float value -> constant "Number" (string value)
                | GlueLiteral.Bool _
                | GlueLiteral.Null -> None
            )

        {
            Attributes = [ FSharpAttribute.RequireQualifiedAccess; FSharpAttribute.Erase ]
            Name = name
            Cases =
                [
                    if constants |> List.exists (fun constant -> constant.Case = "String") then
                        FSharpUnionCase.Field("String", FSharpType.Primitive FSharpPrimitive.String)
                    if constants |> List.exists (fun constant -> constant.Case = "Number") then
                        FSharpUnionCase.Field("Number", FSharpType.Primitive FSharpPrimitive.Float)
                ]
            IsOptional = false
            TypeParameters = []
            Constants = constants
        }
        |> FSharpType.Union
