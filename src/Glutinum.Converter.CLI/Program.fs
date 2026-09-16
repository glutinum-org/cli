module Glutinum.Converter.Program

open Glutinum.Converter.Generate
open Node
open Node.Api
open Fable.Core.JsInterop

// TODO: Create a real CLI parser
let printHelp () =
    let helpText =
        """
Generate Fable bindings from TypeScript definitions - https://github.com/glutinum-org/cl

USAGE

    glutinum <input> [--out-file <output>]
    glutinum --all [--out-file <output>]
    glue <input> [--out-file <output>]

    <input> can be:
      - an installed package name           (e.g. chalk, @types/vscode)
      - a path to a package directory       (e.g. ./node_modules/chalk)
      - a path to a .d.ts file              (e.g. ./node_modules/chalk/source/index.d.ts)

    A package is generated with every package it depends on.
    --all generates every package installed in the nearest node_modules.

OPTIONS

    --out-file <output>     Destination file to write in
                            If not specified, the result will be printed to stdout
    -h, --help              Print this help message

EXAMPLES

    glutinum chalk --out-file ./Glutinum.Chalk.fs
    glutinum --all --out-file ./Glutinum.fs
    glutinum ./node_modules/my-lib/index.d.ts
        """

    Log.log $"%s{helpText}"

let private getVersion () =
    emitJsStatement
        ()
        """
    const pkg = JSON.parse(fs.readFileSync(new URL('./../package.json', import.meta.url)));

    return pkg.version;
    """

let private generate (input: string) =
    if input = "--all" then
        generatePackages []
    elif input.EndsWith ".d.ts" then
        generateBindingFile input
    else
        generatePackages [ input ]

[<EntryPoint>]
let main (argv: string array) =
    let argv = argv |> Array.toList

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

        Log.log $"%s{version}"
        0

    | input :: "--out-file" :: outFile :: [] ->

        Log.info $"Generating binding file for %s{input}"
        let res = generate input

        let outFileDir = path.dirname (outFile)
        fs?mkdirSync $ (outFileDir, {| recursive = true |})
        fs.writeFileSync (outFile, res)

        let absoluteOutFile = path.join (``process``.cwd (), outFile)

        Log.info $"Bindings written to: %s{absoluteOutFile}"
        Log.success "Success!"

        0

    | input :: [] ->
        Log.info $"Generating binding file for %s{input}"
        let res = generate input

        ``process``.stdout.write res |> ignore

        Log.success "Success!"

        0
    | _ ->
        Log.error "Invalid arguments"
        printHelp ()
        1
