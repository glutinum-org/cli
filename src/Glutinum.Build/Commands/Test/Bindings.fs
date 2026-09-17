module Build.Commands.Test.Bindings

open BlackFox.CommandLine
open SimpleExec
open Build.Utils.Pnpm
open Spectre.Console.Cli
open System.ComponentModel

type BindingsTestSettings() =
    inherit CommandSettings()

    [<CommandOption("--headed")>]
    [<Description("Open the browser window of the web tests")>]
    member val IsHeaded: bool = false with get, set

let private fable (workingDirectory: string) (args: string list) =
    Command.Run(
        "dotnet",
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendRaw (String.concat " " args)
        |> CmdLine.toString,
        workingDirectory = workingDirectory
    )

/// The Node binding runs in Node, the Web binding runs in Chromium through a page built with Vite
let runBindingsTests () =
    Pnpm.install ()

    fable "tests/Glutinum.Node.Tests" [ "--runScript" ]

    fable "tests/Glutinum.Web.Tests/page" [ "--outDir"; "build" ]
    Command.Run("npx", "vite build", workingDirectory = "tests/Glutinum.Web.Tests/page")
    fable "tests/Glutinum.Web.Tests" [ "--runScript" ]

type BindingsTestCommand() =
    inherit Command<BindingsTestSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(_, _) =
        runBindingsTests ()
        0
