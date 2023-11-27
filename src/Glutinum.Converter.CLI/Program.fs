module Glutinum.Converter.Program

open Glutinum.Converter.Generate

[<EntryPoint>]
let main (argv : string array) =
    let filePath = argv.[0]
    let res = generateBindingFile filePath

    printfn "Generation result:\n%s" res

    0

// print res

// log(printer.ToString())

// let res = main "tests/specs/functions/topLevelDeclaredBasicFunction.d.ts"
