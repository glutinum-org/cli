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
        "@types/web", "Glutinum.Web/Glutinum.Web.fs"
        "@types/node", "Glutinum.Node/Glutinum.Node.fs"
    ]

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
        for (package, file) in bindings do
            Command.Run(
                "node",
                CmdLine.empty
                |> CmdLine.appendRaw "--stack-size=8000"
                |> CmdLine.appendRaw "../cli.js"
                |> CmdLine.appendRaw package
                |> CmdLine.appendPrefix "--out-file" file
                |> CmdLine.toString,
                workingDirectory = "bindings"
            )

        if settings.IsCheck then
            let files = bindings |> List.map (fun (_, file) -> "bindings/" + file)

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
