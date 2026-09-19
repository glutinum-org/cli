module Build.Commands.Docs

open System.IO
open SimpleExec
open BlackFox.CommandLine
open Build.Utils.Pnpm
open Spectre.Console.Cli
open System.ComponentModel

/// <summary>What every one of these takes, and what it hands to the site.</summary>
/// <remarks>Anything written after <c>--</c> is handed to the site as it stands.</remarks>
type DocsSettings() =
    inherit CommandSettings()

    [<CommandOption("-p|--port <PORT>")>]
    [<Description("The port to serve on.")>]
    member val Port = 0 with get, set

    [<CommandOption("--strict")>]
    [<Description("Treat the site's warnings as errors.")>]
    member val Strict = false with get, set

    [<CommandOption("--verbose")>]
    [<Description("Log what the build is doing.")>]
    member val Verbose = false with get, set

    /// <summary>What the site is given, beyond the name of its command.</summary>
    abstract Arguments: CmdLine -> CmdLine

    default this.Arguments line =
        line
        |> CmdLine.appendPrefixIf (this.Port > 0) "--port" (string this.Port)
        |> CmdLine.appendIf this.Strict "--strict"
        |> CmdLine.appendIf this.Verbose "--verbose"

/// <summary>A build that can be published under a version prefix.</summary>
type VersionedSettings() =
    inherit DocsSettings()

    [<CommandOption("--version <VERSION>")>]
    [<Description("Build under a version prefix, for a site that serves several.")>]
    member val Version = "" with get, set

    override this.Arguments line =
        base.Arguments line
        |> CmdLine.appendPrefixIfNotNullOrEmpty "--version" this.Version

type CleanSettings() =
    inherit DocsSettings()

    [<CommandOption("--global")>]
    [<Description("Empty the shared cache of downloaded tools too.")>]
    member val Global = false with get, set

    override this.Arguments line =
        base.Arguments line |> CmdLine.appendIf this.Global "--global"

type DeploySettings() =
    inherit VersionedSettings()

    [<CommandOption("--dry-run")>]
    [<Description("Say what would be published, publish nothing.")>]
    member val DryRun = false with get, set

    override this.Arguments line =
        base.Arguments line |> CmdLine.appendIf this.DryRun "--dry-run"

type WatchSettings() =
    inherit DocsSettings()

    [<CommandOption("--host [HOST]")>]
    [<Description("Listen on an address other than localhost. On its own, every interface.")>]
    member val Host = FlagValue<string>() with get, set

    [<CommandOption("--no-restart")>]
    [<Description("Serve without rebuilding the site when its own code changes.")>]
    member val NoRestart = false with get, set

    override this.Arguments line =
        base.Arguments line
        |> CmdLine.appendIf this.Host.IsSet "--host"
        |> CmdLine.appendIf (this.Host.IsSet && not (isNull this.Host.Value)) this.Host.Value

/// The web app is published under `/app/` of the documentation site
let private appOutDir = "../../docs/static/app"

let private appStaticDir = "docs/static/app"

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

    if Directory.Exists appStaticDir then
        Directory.Delete(appStaticDir, true)

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
            "bindings/Glutinum.Node/Glutinum.Node.fsproj"
        ] do
        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "build"
            |> CmdLine.appendRaw project
            |> CmdLine.appendPrefix "-c" "Release"
            |> CmdLine.toString
        )

/// <summary>Runs the site with a command of its own, and what the flags asked for.</summary>
let private site
    (command: string)
    (watch: bool)
    (settings: DocsSettings)
    (context: CommandContext)
    =
    let before =
        if watch then
            // Without this, dotnet watch hot-reloads the running site in place instead of restarting it.
            [ "watch"; "--no-hot-reload" ]
        else
            [ "run" ]

    let arguments =
        before
        |> List.fold (fun line argument -> CmdLine.appendRaw argument line) CmdLine.empty
        |> CmdLine.appendPrefix "--project" "docs"
        |> CmdLine.appendRaw "--"
        |> CmdLine.appendRaw command
        |> settings.Arguments
        |> CmdLine.appendSeq context.Remaining.Raw
        |> CmdLine.toString

    Command.RunAsync("dotnet", arguments) |> Async.AwaitTask

type BuildCommand() =
    inherit Command<VersionedSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        buildPackages ()
        buildWebApp ()
        site "build" false settings context |> Async.RunSynchronously
        0

type CheckCommand() =
    inherit Command<DocsSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        buildPackages ()
        buildWebApp ()
        site "check" false settings context |> Async.RunSynchronously
        0

type CleanCommand() =
    inherit Command<CleanSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        site "clean" false settings context |> Async.RunSynchronously

        if Directory.Exists appStaticDir then
            Directory.Delete(appStaticDir, true)

        0

type DeployCommand() =
    inherit Command<DeploySettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        site "gh-pages" false settings context |> Async.RunSynchronously
        0

type WatchCommand() =
    inherit Command<WatchSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, settings) =
        buildPackages ()

        watchWebApp () @ [ site "watch" (not settings.NoRestart) settings context ]
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore

        0
