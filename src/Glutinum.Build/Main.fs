module Build.Main

open SimpleExec
open Build.Commands.Cli
open Build.Commands.Publish
open Build.Commands.Web
open Build.Commands.Test.Specs
open Build.Commands.Lint
open Build.Commands.Format
open Spectre.Console.Cli

[<EntryPoint>]
let main args =

    Command.Run("dotnet", "husky install")

    let app = CommandApp()

    app.Configure(fun config ->
        config.Settings.ApplicationName <- "./build.sh"

        config
            .AddCommand<CliCommand>("cli")
            .WithDescription(
                """Build the CLI tool

You can then invoke the local version of Glutinum by running `node cli.js <args>`"""
            )
        |> ignore

        config.AddBranch(
            "test",
            fun (test: IConfigurator<SpecSettings>) ->
                test.SetDescription(
                    "Run the specs and integration tests if no subcommand is provided"
                )

                test
                    .AddCommand<SpecCommand>("specs")
                    .WithDescription(
                        """Run tests testing isolated TypeScript syntax."""
                    )
                |> ignore
        )
        |> ignore

        config
            .AddCommand<WebCommand>("web")
            .WithDescription("Command related to the web app")
        |> ignore

        config
            .AddCommand<LintCommand>("lint")
            .WithDescription("Run the linter on the source code")
        |> ignore

        config
            .AddCommand<FormatCommand>("format")
            .WithDescription("Format the source code")
        |> ignore

        config
            .AddCommand<PublishCommand>("publish")
            .WithDescription(
                """Publish the different packages to NuGet and NPM based on the CHANGELOG.md files

If the last version in the CHANGELOG.md is different from the version in the packages,
the package will be published
        """
            )
        |> ignore

    )

    app.Run(args)
