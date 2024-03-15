module Build.Tasks.Cli

open SimpleExec
open BlackFox.CommandLine

let handle (args: string list) =
    let isWatch = args |> List.contains "--watch"

    Command.Run(
        "dotnet",
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendRaw "src/Glutinum.Converter.CLI"
        |> CmdLine.appendPrefix "--outDir" "dist"
        |> CmdLine.appendRaw "--sourceMaps"
        |> CmdLine.appendRaw "--test:MSBuildCracker"
        |> CmdLine.appendIf isWatch "--watch"
        |> CmdLine.toString
    )
