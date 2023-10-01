module Tests.Enums

open Fable.Core
open Mocha
open Glutinum.FastGlob
open Glutinum.Ava
open Glutinum.Converter.Program
open Node.Api
open Fable.Core.JsInterop

type Node.Fs.IExports with

    [<Emit("$0.readdirSync($1, $2)")>]
    member __.readdirSync(filename: string, options: obj) : string[] = jsNative

module Fs =

    let readFile filePath =
        JS.Constructors.Promise.Create(fun resolve reject ->
            fs.readFile (
                filePath,
                fun err data ->
                    if err.IsSome then
                        reject (err)
                    else
                        resolve (data.ToString())
            )
        )

let macroTestSpec (t: ExecutionContext<obj>) (specPath: string) =
    promise {
        let filepath = $"{__SOURCE_DIRECTORY__}/specs/{specPath}.d.ts"
        let res = transform filepath
        let! expected = Fs.readFile ($"{__SOURCE_DIRECTORY__}/specs/{specPath}.fs")

        t.deepEqual.Invoke(res, expected) |> ignore
    }

[<ImportDefault("ava")>]
let test: obj = jsNative

// This generates a test for each file .d.ts file in the specs directory
// The .d.ts files needs to have a matching .fs file with the expected output
fs.readdirSync ($"{__SOURCE_DIRECTORY__}/specs", {| recursive = true |})
|> Array.filter (fun x -> x.EndsWith(".d.ts"))
|> Array.map (fun x -> x.Replace(".d.ts", ""))
|> Array.map (fun specPath ->
    test $ ($"{specPath}", macroTestSpec, $"{specPath}")
)
|> ignore
