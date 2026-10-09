module Build.Commands.Test.Converter

open BlackFox.CommandLine
open SimpleExec
open Build.Utils.Pnpm
open Spectre.Console.Cli

type ConverterTestSettings() =
    inherit CommandSettings()

let runConverterTests () =
    Pnpm.install ()

    Command.Run(
        "dotnet",
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendRaw "--runScript"
        |> CmdLine.toString,
        workingDirectory = "tests/Glutinum.Converter.Tests"
    )

type ConverterTestCommand() =
    inherit Command<ConverterTestSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(_, _) =
        runConverterTests ()
        0
