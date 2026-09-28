module Glutinum.Converter.Program

open Glutinum
open Glutinum.Converter
open Glutinum.Converter.Generate
open Fable.Core
open Fable.Core.JsInterop
open Glutinum.Commander.ParseOptions

[<Emit("import.meta.url")>]
let private importMetaUrl () : string = nativeOnly

[<Interface>]
type private PackageJson =
    abstract member version: string with get

[<Interface>]
type private CliOptions =
    abstract member outFile: string option with get
    abstract member moduleName: string option with get
    abstract member all: bool option with get
    abstract member externals: bool with get
    abstract member lib: bool with get
    abstract member external: ResizeArray<string> with get
    abstract member ``include``: ResizeArray<string> with get

let private getVersion () =
    let moduleDir =
        Glutinum.Node.path.Exports.dirname (
            Glutinum.Node.url.Exports.fileURLToPath (importMetaUrl ())
        )

    let content =
        Glutinum.Node.fs.Exports.readFileSync (
            Glutinum.Node.path.Exports.join (moduleDir, "..", "package.json"),
            Glutinum.Node.BufferEncoding.utf8
        )

    let packageJson: PackageJson = !! JS.JSON.parse content

    packageJson.version

let private helpText =
    """
    <input> can be:
      - an installed package name           (e.g. chalk, @types/vscode)
      - a path to a package directory       (e.g. ./node_modules/chalk)
      - a path to a .d.ts file              (e.g. ./node_modules/chalk/source/index.d.ts)

    Several packages are generated in the same file, each one with every
    package it depends on. A .d.ts file is generated alone.

    The types of @types/node and @types/web (the DOM) are referenced from the
    Glutinum.Node and Glutinum.Web bindings, unless asked for or --no-externals.

Examples:
    glutinum chalk --out-file ./Glutinum.Chalk.fs
    glutinum vscode vscode-languageclient --out-file ./Glutinum.Vscode.fs
    glutinum leaflet --external @types/geojson --out-file ./Glutinum.Leaflet.fs
    glutinum --all --out-file ./Glutinum.fs
    glutinum ./node_modules/my-lib/index.d.ts
"""

let private toGenerateOptions (options: CliOptions) : Packages.GenerateOptions =
    let externals =
        options.external
        |> Seq.map (fun value ->
            match value.Split('=') with
            | [| name; moduleName |] -> name, Some moduleName
            | _ -> value, None
        )
        |> Seq.toList

    let includes =
        options.``include`` |> Seq.collect (fun value -> value.Split(',')) |> Seq.toList

    { Packages.defaultOptions with
        ExternalPackages = options.externals
        Externals = externals
        ModuleName = options.moduleName
        Include = includes
        NoLib = not options.lib
    }

let private generate (options: Packages.GenerateOptions) (isAll: bool) (inputs: string list) =
    match isAll, inputs with
    | true, _ -> generatePackagesFromDisk options []
    | false, [ input ] when input.EndsWith ".d.ts" ->
        generateBindingFileWith (options.ModuleName |> Option.defaultValue "Glutinum") input
    | false, inputs -> generatePackagesFromDisk options inputs

let private write (outFile: string option) (content: string) =
    match outFile with
    | Some outFile ->
        let mkdirOptions = createEmpty<Glutinum.Node.fs.Exports.mkdirSync__.options>
        mkdirOptions.recursive <- true

        Glutinum.Node.fs.Exports.mkdirSync (
            Glutinum.Node.path.Exports.dirname outFile,
            mkdirOptions
        )
        |> ignore

        Glutinum.Node.fs.Exports.writeFileSync (outFile, content)

        Log.info $"Bindings written to: %s{Glutinum.Node.path.Exports.resolve outFile}"
    | None ->
        let stdout: Glutinum.Node.NodeJS.WriteStream =
            !!Glutinum.Node.Exports.``process``.stdout

        stdout.write (content, Glutinum.Node.BufferEncoding.utf8) |> ignore

let private run (argv: string array) =
    let program = Commander.Exports.program

    program
        .name("glutinum")
        .description(
            "Generate Fable bindings from TypeScript definitions - https://github.com/glutinum-org/cli"
        )
        .version(getVersion ())
    |> ignore

    program
        .argument("[inputs...]", "packages, a package directory, or a .d.ts file to generate")
        .option("--out-file <path>", "destination file to write in, stdout otherwise")
        .option("--all", "generate every package installed in the nearest node_modules")
        .option(
            "--module-name <name>",
            "full name of the generated module, instead of `Glutinum.<Module>`"
        )
        .option("--no-externals", "generate @types/node and @types/web instead of referencing them")
        .option("--no-lib", "create the program without the TypeScript library")
    |> ignore

    program
        .option(
            "--external <package>",
            "reference <package> from its own binding, as <package> or <package>=<Module>",
            (fun value (previous: ResizeArray<string>) ->
                previous.Add value
                previous
            ),
            ResizeArray()
        )
        .option(
            "--include <names>",
            "comma separated declarations to keep, the others are dropped",
            (fun value (previous: ResizeArray<string>) ->
                previous.Add value
                previous
            ),
            ResizeArray()
        )
        .addHelpText(Commander.AddHelpTextPosition.after, helpText)
        .showHelpAfterError()
    |> ignore

    let parseOptions = Commander.ParseOptions.Create from.user

    program.parse (ResizeArray argv, parseOptions) |> ignore

    let options = program.opts<CliOptions>()
    let inputs = program.args |> Seq.toList
    let isAll = options.all |> Option.defaultValue false

    let mixesFileAndPackages =
        inputs.Length > 1 && inputs |> List.exists (fun input -> input.EndsWith ".d.ts")

    if inputs.IsEmpty && not isAll then
        Log.error "Give at least one input, or --all"
        program.outputHelp (Commander.HelpContext.Create(error = true))
        1
    elif mixesFileAndPackages then
        Log.error "A .d.ts file is generated alone, it can't be combined with other inputs"
        1
    else
        if isAll then
            Log.info "Generating binding file for every installed package"
        else
            Log.info $"""Generating binding file for %s{String.concat ", " inputs}"""

        try
            generate (toGenerateOptions options) isAll inputs |> write options.outFile

            Log.success "Success!"

            0
        with ex ->
            Log.error ex.Message
            1

[<EntryPoint>]
let main (argv: string array) =
    let exitCode = run argv
    // Fable discards the value returned by the entry point
    Glutinum.Node.Exports.``process``.exitCode <- Some !^(float exitCode)
    exitCode
