module Build.Commands.Test.All

open Spectre.Console.Cli
open Build.Commands.Test.Specs
open Build.Commands.Test.Bindings
open Build.Commands.Test.Converter

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
            runConverterTests ()
            runBindingsTests ()
            0
