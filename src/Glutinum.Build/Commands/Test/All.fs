module Build.Commands.Test.All

open Spectre.Console.Cli
open Build.Commands.Test.Specs
open Build.Commands.Test.Bindings

type AllTestSettings() =
    inherit CommandSettings()

type AllTestCommand() =
    inherit Command<AllTestSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(context, _) =
        let exitCode = SpecCommand().Execute(context, SpecSettings())

        if exitCode <> 0 then
            exitCode
        else
            runBindingsTests ()
            0
