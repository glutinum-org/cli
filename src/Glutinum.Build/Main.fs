module Build.Main

open SimpleExec
open BlackFox.CommandLine
open Build.Tasks

// This is a basic help message, as the CLI parser is not a "real" CLI parser
// For now, it is enough as this is just a dev tool
let printHelp () =
    let helpText =
        """
Available commands:
    test                            Run the specs and integration tests if no subcommand is provided

        Subcommands:

            specs                   Run tests testing isolated TypeScript syntax.

                Options:
                    --generate-only Only generate the tests files based on the `references` folder
                                    This is the preferred way to generate the tests files of
                                    you want to use the Test Explorer UI from your IDE.

                                    You need to combine this options with `--watch` if you want
                                    Fable to watch for changes and re-generate the files.

                                    IMPORTANT: When adding or removing a file from the `references` folder,
                                    you need to re-run this command. (Will be improved in the future)

                    --watch         Watch for changes and re-run the tests
                                    You can pass additional arguments to 'vitest' by using '--' followed by the arguments
                                    For example:
                                        ./build.sh test specs --watch -- --ui
                                        ./build.sh test specs --watch -- -t date

    web                             Command related to the web app

        Options:
            --watch                 Watch for changes and re-build the web app

    cli                             Build the CLI tool
                                    You can then invoke the local version of Glutinum
                                    by running `node cli.js <args>`
        Options:
            --watch                 Watch for changes and re-build the CLI tool

    lint                            Run the linter on the source code

    format                          Format the source code

    publish                         Publish the different packages to NuGet and NPM
                                    based on the CHANGELOG.md files
                                    If the last version in the CHANGELOG.md is
                                    different from the version in the packages,
                                    the package will be published

        Options:
            --web-only              Publish the web app only
                                    This is useful when there is a bug related to the
                                    Web app only for which we need to publish a new version
                                    separately from the CLI tool
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
    | "test" :: args ->
        match args with
        | "specs" :: args -> Test.Specs.handle args
        // | "integration" :: args -> Test.Integration.handle args
        | _ -> Test.Specs.handle args
    | "web" :: args -> Web.handle args
    | "publish" :: args -> Publish.handle args
    | "lint" :: _ -> Command.Run("dotnet", "fantomas --check src tests")
    | "format" :: _ -> Command.Run("dotnet", "fantomas src tests")
    | "cli" :: args -> Cli.handle args
    | "help" :: _
    | "--help" :: _
    | _ -> printHelp ()

    0
