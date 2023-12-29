module Glutinum.Converter.Program

open Glutinum.Converter.Generate
open Node
open Fable.Core.JsInterop

// TODO: Create a real CLI parser
let printHelp () =
    let helpText =
        """
Generate Fable bindings from TypeScript definitions - https://github.com/glutinum-org/cl

USAGE

    glutinum <input> [--out-file <output>]
    glue <input> [--out-file <output>]

OPTIONS

    --out-file <output>     Destination file to write in
                            If not specified, the result will be printed to stdout
    -h, --help              Print this help message

EXAMPLES

    glutinum ./node_modules/my-lib/index.d.ts --out-file ./Glutinum.MyLib.fs
    glutinum ./node_modules/my-lib/index.d.ts
        """

    printfn $"%s{helpText}"

let private getVersion () =
    emitJsStatement
        ()
        """
    const pkg = JSON.parse(fs.readFileSync(new URL('./../package.json', import.meta.url)));

    return pkg.version;
    """

[<EntryPoint>]
let main (argv: string array) =
    let argv = argv |> Array.map (fun x -> x.ToLower()) |> Array.toList

    // Naive CLI parser
    // Order of matching is important !!!
    match argv with
    | []
    | "-h" :: _
    | "--help" :: _
    | "help" :: _ ->
        printHelp ()
        0

    | "--version" :: [] ->
        let version = getVersion ()

        printfn $"%s{version}"
        0

    | input :: "--out-file" :: outFile :: [] ->

        printfn "Generating binding file for %s" input
        let res = generateBindingFile input

        let outFileDir = path.dirname (outFile)
        fs?mkdirSync $ (outFileDir, {| recursive = true |})
        fs.writeFileSync (outFile, res)

        0

    | input :: [] ->
        printfn "Generating binding file for %s" input
        let res = generateBindingFile input

        printfn "Generation result:\n%s" res

        0
    | _ ->
        printfn "Invalid arguments"
        printHelp ()
        1
