module Build.Commands.CommanderBinding

open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli

let private bindingFile = "src/Glutinum.Converter.CLI/Commander.fs"

type CommanderBindingCommand() =
    inherit Command<EmptyCommandSettings>()

    override _.Execute(context, settings) =
        Command.Run(
            "node",
            CmdLine.empty
            |> CmdLine.appendRaw "cli.js"
            |> CmdLine.appendRaw "commander"
            |> CmdLine.appendPrefix "--out-file" bindingFile
            |> CmdLine.toString
        )

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fantomas"
            |> CmdLine.appendRaw bindingFile
            |> CmdLine.toString
        )

        0
