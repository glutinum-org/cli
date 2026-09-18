module Glutinum.Converter.Program

open Glutinum.Converter
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

    glutinum <input>... [--out-file <output>]
    glutinum --all [--out-file <output>]
    glue <input>... [--out-file <output>]

    <input> can be:
      - an installed package name           (e.g. chalk, @types/vscode)
      - a path to a package directory       (e.g. ./node_modules/chalk)
      - a path to a .d.ts file              (e.g. ./node_modules/chalk/source/index.d.ts)

    Several packages are generated in the same file, each one with every
    package it depends on. A .d.ts file is generated alone.
    --all generates every package installed in the nearest node_modules.

    The types of @types/node and @types/web (the DOM) are referenced from the
    Glutinum.Node and Glutinum.Web bindings, unless asked for or --no-externals.

OPTIONS

    --out-file <output>     Destination file to write in
                            If not specified, the result will be printed to stdout
    --no-externals          Generate @types/node and @types/web with the packages
                            using them instead of referencing their bindings
    --external <package>    Reference <package> from its own binding instead of
                            generating it, `Glutinum.<Module>` is derived from its
                            name unless given as <package>=<Module>
                            Can be repeated
    --module-name <name>    Full name of the generated module, `Glutinum.Types.TypeScript`,
                            instead of `Glutinum.<Module>` derived from the package name
    --include <names>       Comma-separated declarations to keep, the others are dropped
                            Can be repeated
    --no-lib                Create the program without the TypeScript library, for a
                            package made of its files
    -h, --help              Print this help message

EXAMPLES

    glutinum chalk --out-file ./Glutinum.Chalk.fs
    glutinum vscode vscode-languageclient --out-file ./Glutinum.Vscode.fs
    glutinum leaflet --external @types/geojson --out-file ./Glutinum.Leaflet.fs
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

let private generate (options: Packages.GenerateOptions) (inputs: string list) =
    match inputs with
    | [ "--all" ] -> generatePackagesFromDisk options []
    | [ input ] when input.EndsWith ".d.ts" -> generateBindingFile input
    | inputs -> generatePackagesFromDisk options inputs

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

    | _ ->
        let inputs, outFile =
            match List.rev argv with
            | outFile :: "--out-file" :: inputs -> List.rev inputs, Some outFile
            | _ -> argv, None

        let rec takeOptions
            (args: string list)
            (options: Packages.GenerateOptions)
            (rest: string list)
            =
            match args with
            | "--external" :: value :: tail ->
                let external =
                    match value.Split('=') with
                    | [| name; moduleName |] -> name, Some moduleName
                    | _ -> value, None

                takeOptions
                    tail
                    { options with
                        Externals = options.Externals @ [ external ]
                    }
                    rest
            | "--module-name" :: value :: tail ->
                takeOptions tail { options with ModuleName = Some value } rest
            | "--no-lib" :: tail -> takeOptions tail { options with NoLib = true } rest
            | "--include" :: value :: tail ->
                takeOptions
                    tail
                    { options with
                        Include = options.Include @ (value.Split(',') |> Array.toList)
                    }
                    rest
            | arg :: tail -> takeOptions tail options (arg :: rest)
            | [] -> options, List.rev rest

        let options, inputs = takeOptions inputs Packages.defaultOptions []

        let options =
            { options with
                ExternalPackages = not (List.contains "--no-externals" inputs)
            }

        let inputs = inputs |> List.filter (fun input -> input <> "--no-externals")

        let hasUnknownOption =
            inputs |> List.exists (fun input -> input.StartsWith "-" && input <> "--all")

        let mixesFileAndPackages =
            inputs.Length > 1 && inputs |> List.exists (fun input -> input.EndsWith ".d.ts")

        if inputs.IsEmpty || hasUnknownOption then
            Log.error "Invalid arguments"
            printHelp ()
            1
        elif mixesFileAndPackages then
            Log.error "A .d.ts file is generated alone, it can't be combined with other inputs"
            1
        else
            Log.info $"""Generating binding file for %s{String.concat ", " inputs}"""
            let res = generate options inputs

            match outFile with
            | Some outFile ->
                let outFileDir = path.dirname (outFile)
                fs?mkdirSync $ (outFileDir, {| recursive = true |})
                fs.writeFileSync (outFile, res)

                let absoluteOutFile = path.join (``process``.cwd (), outFile)

                Log.info $"Bindings written to: %s{absoluteOutFile}"
            | None -> ``process``.stdout.write res |> ignore

            Log.success "Success!"

            0
