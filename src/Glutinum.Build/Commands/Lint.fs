module Build.Commands.Lint

open SimpleExec
open Spectre.Console.Cli

type LintSettings() =
    inherit CommandSettings()

type LintCommand() =
    inherit Command<LintSettings>()

    override _.Execute(context, settings) =
        Command.Run("dotnet", "fantomas --check src tests")

        0
