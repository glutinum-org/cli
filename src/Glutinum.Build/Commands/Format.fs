module Build.Commands.Format

open SimpleExec
open Spectre.Console.Cli

type FormatSettings() =
    inherit CommandSettings()

type FormatCommand() =
    inherit Command<FormatSettings>()

    override _.Execute(context, settings) =
        Command.Run("dotnet", "fantomas src tests")

        0
