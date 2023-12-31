module Build.Main

open SimpleExec
open BlackFox.CommandLine

// This is a basic help message, as the CLI parser is not a "real" CLI parser
// For now, it is enough as this is just a dev tool
let printHelp () =
    let helpText =
        """
Usage: dotnet run <command> [<args>]

Available commands:
    test                            Run the main tests suite

        Options:
            --watch                 Watch for changes and re-run the tests
                                    You can pass additional arguments to 'ava'
                                    by using '--' followed by the arguments
                                    For example: -- --match="**class**"

    cli                             Build the CLI tool
        Options:
            --watch                 Watch for changes and re-build the CLI tool

    lint                            Run the linter on the source code

    format                          Format the source code

    publish                         Publish the different packages to NuGet and NPM
                                    based on the CHANGELOG.md files
                                    If the last version in the CHANGELOG.md is
                                    different from the version in the packages,
                                    the package will be published
        """

    printfn $"%s{helpText}"

module Cli =

    let handle (args: string list) =
        let isWatch = args |> List.contains "--watch"

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendRaw "src/Glutinum.Converter.CLI"
            |> CmdLine.appendPrefix "--outDir" "dist"
            |> CmdLine.appendRaw "--sourceMaps"
            |> CmdLine.appendIf isWatch "--watch"
            |> CmdLine.toString
        )

[<EntryPoint>]
let main argv =
    let argv = argv |> Array.map (fun x -> x.ToLower()) |> Array.toList

    Command.Run("dotnet", "husky install")

    match argv with
    | "test" :: args -> Test.Specs.handle args
    | "publish" :: args -> Publish.handle args
    | "lint" :: _ -> Command.Run("dotnet", "fantomas --check src")
    | "format" :: _ -> Command.Run("dotnet", "fantomas src")
    | "cli" :: args -> Cli.handle args
    | "help" :: _
    | "--help" :: _
    | _ -> printHelp ()

    0
