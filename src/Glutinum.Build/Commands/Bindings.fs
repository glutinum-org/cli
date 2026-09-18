module Build.Commands.Bindings

open System.IO
open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open System.ComponentModel
open Build.Utils.Pnpm

/// The bindings published from this repository, generated from the packages pinned in `bindings/package.json`
let bindings =
    [
        "@types/web", "Glutinum.Web/Glutinum.Web.fs", []
        "@types/node", "Glutinum.Node/Glutinum.Node.fs", []
        // The ES library types Fable.Core has no counterpart for, see `esLibraryTypes`
        "./es",
        "../src/Glutinum.Types/TypeScript.fs",
        [ "--module-name"; "Glutinum.Types.TypeScript"; "--no-lib" ]
    ]

/// The declarations of `lib.es*.d.ts` kept in `Glutinum.Types`: what a binding refers to and
/// Fable.Core does not define. `Array`, `Date`, `Promise`, `Map` and the typed arrays are Fable.Core's.
let esLibraryTypes =
    [
        "Date"
        "DateConstructor"
        // The static side of the globals, their instances are `float`, `string` and `obj`
        "value:Number"
        "NumberConstructor"
        "value:String"
        "StringConstructor"
        "value:Object"
        "ObjectConstructor"
        "value:Symbol"
        "SymbolConstructor"
        // `JSON` and `Math` are complete in Fable.Core
        "Reflect"
        "PropertyDescriptor"
        "TypedPropertyDescriptor"
        "ArrayLike"
        "ConcatArray"
        "ReadonlyArray"
        "ReadonlyMap"
        "ReadonlySet"
        "PromiseLike"
        "TemplateStringsArray"
        "Iterator"
        "IteratorResult"
        "IteratorYieldResult"
        "IteratorReturnResult"
        "IterableIterator"
        "Generator"
        "ArrayBufferLike"
        "ArrayBufferTypes"
        "SharedArrayBuffer"
        "SharedArrayBufferConstructor"
        "ErrorOptions"
        "PropertyKey"
        "PropertyDescriptorMap"
        "ProxyHandler"
        "ProxyConstructor"
        "BooleanConstructor"
    ]

/// `bindings/es` is a package made of copies of the `lib.es*.d.ts` files of the pinned TypeScript,
/// the originals are the library of every program and are never generated
let private writeEsPackage () =
    let source = Path.Combine("bindings", "node_modules", "typescript", "lib")
    let target = Path.Combine("bindings", "es")

    if Directory.Exists target then
        Directory.Delete(target, true)

    Directory.CreateDirectory target |> ignore

    let files =
        Directory.GetFiles(source, "lib.es*.d.ts")
        |> Array.map Path.GetFileName
        |> Array.filter (fun name -> not (name.Contains ".full."))
        |> Array.sort

    // The `reference lib` directives would load the originals next to the copies
    for name in files do
        File.ReadAllLines(Path.Combine(source, name))
        |> Array.filter (fun line -> not (line.Contains "<reference lib="))
        |> fun lines -> File.WriteAllLines(Path.Combine(target, name), lines)

    let references =
        files
        |> Array.map (fun name -> $"/// <reference path=\"{name}\" />")
        |> String.concat "\n"

    File.WriteAllText(Path.Combine(target, "index.d.ts"), references + "\n")

    File.WriteAllText(
        Path.Combine(target, "package.json"),
        """{ "name": "es", "version": "0.0.0", "types": "index.d.ts" }
"""
    )

type BindingsSettings() =
    inherit CommandSettings()

    [<CommandOption("--check")>]
    [<Description("Fail when the generated files differ from the committed ones")>]
    member val IsCheck: bool = false with get, set

type BindingsCommand() =
    inherit Command<BindingsSettings>()

    override _.Execute(context, settings) =
        Pnpm.install ()

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendRaw "src/Glutinum.Converter.CLI"
            |> CmdLine.appendPrefix "--outDir" "dist"
            |> CmdLine.toString
        )

        // From `bindings`, so the declaration packages stay out of the repository's own `node_modules`
        writeEsPackage ()

        for (package, file, options) in bindings do
            Command.Run(
                "node",
                CmdLine.empty
                |> CmdLine.appendRaw "--stack-size=8000"
                |> CmdLine.appendRaw "../cli.js"
                |> CmdLine.appendRaw package
                |> (fun command ->
                    if package = "./es" then
                        command
                        |> CmdLine.appendPrefix "--include" (String.concat "," esLibraryTypes)
                    else
                        command
                )
                |> CmdLine.appendRaw (String.concat " " options)
                |> CmdLine.appendPrefix "--out-file" file
                |> CmdLine.toString,
                workingDirectory = "bindings"
            )

        if settings.IsCheck then
            let files = bindings |> List.map (fun (_, file, _) -> "bindings/" + file)

            try
                Command.Run(
                    "git",
                    CmdLine.empty
                    |> CmdLine.appendRaw "diff"
                    |> CmdLine.appendRaw "--exit-code"
                    |> CmdLine.appendRaw "--stat"
                    |> CmdLine.appendRaw "--"
                    |> CmdLine.appendRaw (String.concat " " files)
                    |> CmdLine.toString
                )

                0
            with :? ExitCodeException ->
                printfn
                    "The bindings are out of date, run `./build.sh bindings` and commit the result"

                1
        else
            0
