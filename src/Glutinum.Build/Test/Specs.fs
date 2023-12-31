module Build.Test.Specs

open BlackFox.CommandLine
open SimpleExec
open Build.Utils.Pnpm

let handle (args: string list) =
    let isWatch = args |> List.contains "--watch"

    let additionalArgs =
        args
        |> List.skipWhile (fun x -> x <> "--")
        |> List.skip 1
        |> String.concat " "

    Pnpm.install ()

    let avaCmd =
        CmdLine.empty
        |> CmdLine.appendRaw "npx"
        |> CmdLine.appendRaw "ava"
        |> CmdLine.appendIf isWatch additionalArgs
        |> CmdLine.toString

    let runCommand =
        if isWatch then
            "--runWatch"
        else
            "--run"

    let fableCmd =
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendIf isWatch "watch"
        |> CmdLine.appendRaw "tests"
        |> CmdLine.appendRaw "--sourceMaps"
        |> CmdLine.appendRaw runCommand
        |> CmdLine.appendRaw avaCmd
        |> CmdLine.toString

    Command.Run("dotnet", fableCmd)
