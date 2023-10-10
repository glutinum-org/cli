module Tests.Enums

open Fable.Core
open Mocha
open Glutinum.FastGlob
open Glutinum.Ava
open Glutinum.Converter.Program
open Node.Api
open Fable.Core.JsInterop
open System

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

let private removeHeader (textContent : string) =
    textContent.Replace("\r\n", "\n").Split('\n')
    // Skip start of header
    |> Array.skip 1
    // Skip until first line with (***)
    |> Array.skipWhile (fun x -> x.StartsWith("(***)") |> not)
    // Skip end of header (***)
    |> Array.skip 1
    // Skip until we find a non-empty line
    |> Array.skipWhile String.IsNullOrEmpty
    |> String.concat "\n"

let macroTestSpec (t: ExecutionContext<obj>) (specPath: string) =
    promise {
        let filepath = $"{__SOURCE_DIRECTORY__}/specs/{specPath}.d.ts"
        let res = transform filepath
        let! expectedContent =
            $"{__SOURCE_DIRECTORY__}/specs/{specPath}.fsx"
            |> Fs.readFile
        let expected = removeHeader expectedContent

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
