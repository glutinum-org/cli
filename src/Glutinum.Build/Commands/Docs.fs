module Build.Commands.Docs

open System.IO
open SimpleExec
open BlackFox.CommandLine
open Build.Utils.Pnpm
open Spectre.Console.Cli
open System.ComponentModel

type DocsSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    [<Description("Serve the documentation and rebuild it on change")>]
    member val IsWatch: bool = false with get, set

/// The web app is published under `/app/` of the documentation site
let private buildWebApp () =
    Pnpm.install ()

    Command.Run("npx", "fcm", workingDirectory = "src/Glutinum.Web")

    if Directory.Exists "docs/static/app" then
        Directory.Delete("docs/static/app", true)

    Command.Run(
        "dotnet",
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendRaw "--noCache"
        |> CmdLine.toString,
        workingDirectory = "src/Glutinum.Web"
    )

    Command.Run(
        "npx",
        CmdLine.empty
        |> CmdLine.appendRaw "vite build"
        |> CmdLine.appendPrefix "--base" "/app/"
        |> CmdLine.appendPrefix "--outDir" "../../docs/static/app"
        |> CmdLine.toString,
        workingDirectory = "src/Glutinum.Web"
    )

type DocsCommand() =
    inherit Command<DocsSettings>()

    override _.Execute(context, settings) =
        buildWebApp ()

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "run"
            |> CmdLine.appendPrefix "--project" "docs"
            |> CmdLine.appendRaw "--"
            |> CmdLine.appendRaw (
                if settings.IsWatch then
                    "watch"
                else
                    "build"
            )
            |> CmdLine.toString
        )

        0
