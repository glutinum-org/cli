module Build.Test.Specs

open BlackFox.CommandLine
open SimpleExec

let handle (args: string list) =
    let isWatch = args |> List.contains "--watch"

    let vitestCmd =
        CmdLine.empty
        |> CmdLine.appendRaw "npx"
        |> CmdLine.appendRaw "vitest"
        |> CmdLine.appendIf (not isWatch) "run"
        |> CmdLine.toString

    let fableCmd =
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendIf isWatch "watch"
        |> CmdLine.appendRaw "src/Glutinum.Converter"
        |> CmdLine.appendPrefix "--outDir" "fableBuild"
        |> CmdLine.appendRaw "--sourceMaps"
        |> CmdLine.appendRaw "--run"
        |> CmdLine.appendRaw vitestCmd
        |> CmdLine.toString

    Command.Run("dotnet", fableCmd)
