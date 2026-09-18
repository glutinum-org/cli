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
let private appOutDir = "../../docs/static/app"

let private viteBuild (watch: bool) =
    CmdLine.empty
    |> CmdLine.appendRaw "vite build"
    |> CmdLine.appendIf watch "--watch"
    |> CmdLine.appendPrefix "--base" "/app/"
    |> CmdLine.appendPrefix "--outDir" appOutDir
    |> CmdLine.toString

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

    Command.Run("npx", viteBuild false, workingDirectory = "src/Glutinum.Web")

/// The app is rebuilt into the static files of the site as its sources change, the site
/// picks the new files up
let private watchWebApp () =
    Pnpm.install ()

    Command.Run("npx", "fcm", workingDirectory = "src/Glutinum.Web")

    [
        Command.RunAsync(
            "npx",
            CmdLine.empty
            |> CmdLine.appendRaw "nodemon"
            |> CmdLine.appendPrefix "-e" "module.scss"
            |> CmdLine.appendPrefix "--watch" "**/*.module.scss"
            |> CmdLine.appendPrefix "--exec" "npx fcm"
            |> CmdLine.toString,
            workingDirectory = "src/Glutinum.Web"
        )
        |> Async.AwaitTask

        Command.RunAsync(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendRaw "--noCache"
            |> CmdLine.appendRaw "--watch"
            |> CmdLine.appendRaw "--run"
            |> CmdLine.appendRaw ("npx " + viteBuild true)
            |> CmdLine.toString,
            workingDirectory = "src/Glutinum.Web"
        )
        |> Async.AwaitTask
    ]

/// The API reference reads the Release assemblies of the packages published from here
let private buildPackages () =
    for project in
        [
            "src/Glutinum.Types/Glutinum.Types.fsproj"
            "bindings/Glutinum.Web/Glutinum.Web.fsproj"
        ] do
        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "build"
            |> CmdLine.appendRaw project
            |> CmdLine.appendPrefix "-c" "Release"
            |> CmdLine.toString
        )

let private site (command: string) =
    Command.RunAsync(
        "dotnet",
        CmdLine.empty
        |> CmdLine.appendRaw "run"
        |> CmdLine.appendPrefix "--project" "docs"
        |> CmdLine.appendRaw "--"
        |> CmdLine.appendRaw command
        |> CmdLine.toString
    )
    |> Async.AwaitTask

type DocsCommand() =
    inherit Command<DocsSettings>()

    override _.Execute(context, settings) =
        buildPackages ()

        if settings.IsWatch then
            watchWebApp () @ [ site "watch" ]
            |> Async.Parallel
            |> Async.RunSynchronously
            |> ignore
        else
            buildWebApp ()
            site "build" |> Async.RunSynchronously

        0
