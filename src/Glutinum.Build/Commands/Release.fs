module Build.Commands.Release

open System
open System.IO
open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open Build.Utils.Pnpm

// A package is pushed after the ones it references, so a restore never sees a missing dependency
let private packages =
    [ "src/Glutinum.Types"; "bindings/Glutinum.Web"; "bindings/Glutinum.Node" ]

type ReleaseSettings() =
    inherit CommandSettings()

type ReleaseCommand() =
    inherit Command<ReleaseSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(_, _) =
        let apiKey = Environment.GetEnvironmentVariable "NUGET_KEY"

        if String.IsNullOrWhiteSpace apiKey then
            printfn "NUGET_KEY is not set"
            1
        else
            if Directory.Exists "nupkgs" then
                Directory.Delete("nupkgs", true)

            for package in packages do
                Command.Run(
                    "dotnet",
                    CmdLine.empty
                    |> CmdLine.appendRaw "pack"
                    |> CmdLine.appendRaw package
                    |> CmdLine.appendPrefix "-c" "Release"
                    |> CmdLine.appendPrefix "-o" "nupkgs"
                    |> CmdLine.toString
                )

                let name = Path.GetFileName package

                for nupkg in Directory.GetFiles("nupkgs", $"{name}.*.nupkg") do
                    Command.Run(
                        "dotnet",
                        CmdLine.empty
                        |> CmdLine.appendRaw "nuget"
                        |> CmdLine.appendRaw "push"
                        |> CmdLine.appendRaw nupkg
                        |> CmdLine.appendPrefix "--api-key" apiKey
                        |> CmdLine.appendPrefix "--source" "https://api.nuget.org/v3/index.json"
                        |> CmdLine.appendRaw "--skip-duplicate"
                        |> CmdLine.toString
                    )

            // The CLI on npm
            Command.Run(
                "dotnet",
                CmdLine.empty
                |> CmdLine.appendRaw "fable"
                |> CmdLine.appendRaw "src/Glutinum.Converter.CLI"
                |> CmdLine.appendPrefix "--outDir" "dist"
                |> CmdLine.toString
            )

            Pnpm.publish (noGitChecks = true, access = Publish.Access.Public)

            0
