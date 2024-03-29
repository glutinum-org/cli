module Build.Web

open System
open System.IO
open SimpleExec
open BlackFox.CommandLine
open Build.Utils
open Build.Utils.Pnpm
open SimpleExec

let handle (args: string list) =
    let isWatch = args |> List.contains "--watch"

    Pnpm.install ()

    Command.Run("npx", "fcm", workingDirectory = "src/Glutinum.Web")

    if isWatch then
        Async.Parallel
            [
                Command.RunAsync(
                    "npx",
                    CmdLine.empty
                    |> CmdLine.appendRaw "nodemon"
                    |> CmdLine.appendPrefix "-e" "module.scss"
                    |> CmdLine.appendPrefix "--watch" "**/*.module.scss"
                    |> CmdLine.appendPrefix "--exec" "npx fcm"
                    |> CmdLine.toString,
                    workingDirectory = "src/Glutinum.Web"
                )
                |> Async.AwaitTask

                Command.RunAsync(
                    "dotnet",
                    CmdLine.empty
                    |> CmdLine.appendRaw "fable"
                    |> CmdLine.appendRaw "--noCache"
                    |> CmdLine.appendRaw "--sourceMaps"
                    |> CmdLine.appendRaw "--verbose"
                    |> CmdLine.appendRaw "--watch"
                    |> CmdLine.appendRaw "--test:MSBuildCracker"
                    |> CmdLine.appendRaw "--run"
                    |> CmdLine.appendRaw "npx vite"
                    |> CmdLine.toString,
                    workingDirectory = "src/Glutinum.Web"
                )
                |> Async.AwaitTask

            ]
        |> Async.RunSynchronously
        |> ignore

    else

        if (Directory.Exists("src/Glutinum.Web/dist")) then
            Directory.Delete("src/Glutinum.Web/dist", true)

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendRaw "--noCache"
            |> CmdLine.appendRaw "--verbose"
            |> CmdLine.appendRaw "--test:MSBuildCracker"
            |> CmdLine.toString,
            workingDirectory = "src/Glutinum.Web"
        )

        Command.Run("npx", "vite build", workingDirectory = "src/Glutinum.Web")
