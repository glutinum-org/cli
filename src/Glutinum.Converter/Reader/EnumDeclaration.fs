module Glutinum.Converter.Reader.EnumDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.Utils
open TypeScript
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

let private readEnumMembers
    (checker: Ts.TypeChecker)
    (state:
        {|
            NextCaseIndex: int
            Members: GlueEnumMember list
        |})
    (enumMember: Ts.EnumMember)
    =

    let caseValue =
        match enumMember.initializer with
        | None ->
            match checker.getConstantValue (!^enumMember) with
            | Some(U2.Case1 str) -> GlueLiteral.String str
            | Some(U2.Case2 num) ->
                if Constructors.Number.isSafeInteger num then
                    GlueLiteral.Int(int num)
                else
                    GlueLiteral.Float num
            | None -> GlueLiteral.Int(state.NextCaseIndex)
        | Some initializer ->
            match tryReadLiteral initializer with
            | Some glueLiteral ->
                match glueLiteral with
                | GlueLiteral.String _
                | GlueLiteral.Int _
                | GlueLiteral.Float _ as value -> value
                | GlueLiteral.Bool _ ->
                    failwith (
                        generateReaderError
                            "enum member"
                            "Boolean literals are not supported in enums"
                            initializer
                    )

            | None ->
                failwith (
                    generateReaderError
                        "enum member"
                        "readEnumCases: Unsupported enum initializer"
                        initializer
                )

    let name = unbox<Ts.Identifier> enumMember.name

    let newCase =
        {
            Name = name.getText ()
            Value = caseValue
        }
        : GlueEnumMember

    {|
        NextCaseIndex =
            match caseValue with
            // Use the current case index as a reference for the next case
            // In TypeScript, you can have the following enum:
            // enum E { A, B = 4, C }
            // Meaning that C is 5
            | GlueLiteral.Int i -> i + 1
            // TODO: Mixed enums is not supported in F#, should we fail, ignore
            // or generate a comment in the generated code?
            | _ -> state.NextCaseIndex + 1
        Members = state.Members @ [ newCase ]
    |}

let readEnumDeclaration
    (reader: TypeScriptReader)
    (enumDeclaration: Ts.EnumDeclaration)
    : GlueEnum
    =
    let initialState =
        {|
            NextCaseIndex = 0
            Members = []
        |}

    let readEnumResults =
        enumDeclaration.members
        |> List.ofSeq
        |> List.fold (readEnumMembers reader.checker) initialState

    {
        Name = enumDeclaration.name.getText ()
        Members = readEnumResults.Members
    }
