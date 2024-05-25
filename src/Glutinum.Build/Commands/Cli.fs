module Build.Commands.Cli

open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open System.ComponentModel

type CliSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    [<Description("Watch for changes and re-build the CLI tool")>]
    member val IsWatch: bool = false with get, set

type CliCommand() =
    inherit Command<CliSettings>()

    override _.Execute(context, settings) =
        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendRaw "src/Glutinum.Converter.CLI"
            |> CmdLine.appendPrefix "--outDir" "dist"
            |> CmdLine.appendRaw "--sourceMaps"
            |> CmdLine.appendRaw "--test:MSBuildCracker"
            |> CmdLine.appendIf settings.IsWatch "--watch"
            |> CmdLine.toString
        )

        0
