module Tests.Enums

open Fable.Core
open Mocha
open Glutinum.FastGlob
open Glutinum.Ava
open Glutinum.Converter.Program
open Node.Api
open Fable.Core.JS

let readFile filePath =
    Constructors.Promise.Create(fun resolve reject ->
        fs.readFile(filePath, fun err data ->
            if err.IsSome then
                reject(err)
            else
                resolve(data.ToString())
        )
    )

test.test("literal string enums are converted into DU decorated with StringEnum", fun t ->
    promise {
        let filepath = __SOURCE_DIRECTORY__ + "/specs/enums/literalStringEnum.d.ts"
        let res = transform filepath
        let! expected = readFile(__SOURCE_DIRECTORY__ + "/specs/enums/literalStringEnum.fs")

        t.deepEqual.Invoke(res, expected) |> ignore
    }
)


test.test("literal numeric enums are converted into F# enums", fun t ->
    promise {
        let filepath = __SOURCE_DIRECTORY__ + "/specs/enums/literalNumericEnum.d.ts"
        let res = transform filepath
        let! expected = readFile(__SOURCE_DIRECTORY__ + "/specs/enums/literalNumericEnum.fs")

        t.deepEqual.Invoke(res, expected) |> ignore
    }
)

test.test("named enums are converted into DU decarated with StringEnum and cases with CompiledName", fun t ->
    promise {
        let filepath = __SOURCE_DIRECTORY__ + "/specs/enums/namedStringEnum.d.ts"
        let res = transform filepath
        let! expected = readFile(__SOURCE_DIRECTORY__ + "/specs/enums/namedStringEnum.fs")

        t.deepEqual.Invoke(res, expected) |> ignore
    }
)

test.test("named int enums are converted into F# enums", fun t ->
    promise {
        let filepath = __SOURCE_DIRECTORY__ + "/specs/enums/namedIntEnum.d.ts"
        let res = transform filepath
        let! expected = readFile(__SOURCE_DIRECTORY__ + "/specs/enums/namedIntEnum.fs")

        t.deepEqual.Invoke(res, expected) |> ignore
    }
)

test.test("named int enums with initial rank are converted info F# enums which respect that rank", fun t ->
    promise {
        let filepath = __SOURCE_DIRECTORY__ + "/specs/enums/namedIntEnumWithInitialRank.d.ts"
        let res = transform filepath
        let! expected = readFile(__SOURCE_DIRECTORY__ + "/specs/enums/namedIntEnumWithInitialRank.fs")

        t.deepEqual.Invoke(res, expected) |> ignore
    }
)
