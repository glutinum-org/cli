module Build.Main

open SimpleExec
open Build.Commands.Cli
open Build.Commands.Bindings
open Build.Commands.Web
open Build.Commands.Docs
open Build.Commands.Test.Specs
open Build.Commands.Test.Bindings
open Build.Commands.Test.All
open Build.Commands.Release
open Build.Commands.Lint
open Build.Commands.Format
open Spectre.Console.Cli

[<EntryPoint>]
let main args =

    if System.Environment.GetEnvironmentVariable "ACT" = null then
        Command.Run("dotnet", "husky install")

    let app = CommandApp()

    app.Configure(fun config ->
        config.Settings.ApplicationName <- "./build.sh"
        config.UseStrictParsing() |> ignore

        config
            .AddCommand<CliCommand>("cli")
            .WithDescription(
                """Build the CLI tool

You can then invoke the local version of Glutinum by running `node cli.js <args>`"""
            )
        |> ignore

        config
            .AddCommand<BindingsCommand>("bindings")
            .WithDescription(
                """Generate the Glutinum.Web and Glutinum.Node bindings from the packages pinned in package.json

`--check` fails when the committed files are out of date"""
            )
        |> ignore

        config.AddBranch(
            "test",
            fun (test: IConfigurator<CommandSettings>) ->
                test.SetDescription(
                    "Run the specs and integration tests if no subcommand is provided"
                )

                test
                    .AddCommand<SpecCommand>("specs")
                    .WithDescription("""Run tests testing isolated TypeScript syntax.""")
                |> ignore

                test
                    .AddCommand<BindingsTestCommand>("bindings")
                    .WithDescription(
                        "Run the Glutinum.Node and Glutinum.Web bindings in Node and Chromium"
                    )
                |> ignore

                test.SetDefaultCommand<AllTestCommand>()
        )
        |> ignore

        config
            .AddCommand<ReleaseCommand>("release")
            .WithDescription(
                "Pack Glutinum.Types, Glutinum.Web and Glutinum.Node, push them to nuget.org and publish the CLI to npm"
            )
        |> ignore

        config
            .AddCommand<WebCommand>("web")
            .WithDescription("Command related to the web app")
        |> ignore

        config.AddBranch(
            "docs",
            fun (docs: IConfigurator<CommandSettings>) ->
                docs.SetDescription "Build the documentation site, with the web app under /app/"

                docs
                    .AddCommand<WatchCommand>("watch")
                    .WithDescription("Serve it, rebuilding as you write")
                    .WithExample("docs watch")
                    .WithExample("docs watch --host")
                |> ignore

                docs
                    .AddCommand<BuildCommand>("build")
                    .WithDescription("Build it into docs/output")
                    .WithExample("docs build")
                |> ignore

                docs
                    .AddCommand<CheckCommand>("check")
                    .WithDescription("Build it all, write none of it, fail on anything wrong")
                    .WithExample("docs check")
                |> ignore

                docs
                    .AddCommand<CleanCommand>("clean")
                    .WithDescription("Remove what a build wrote")
                    .WithExample("docs clean")
                |> ignore

                docs
                    .AddCommand<DeployCommand>("deploy")
                    .WithDescription("Publish the last build to the gh-pages branch")
                    .WithExample("docs deploy --dry-run")
                    .WithExample("docs deploy")
                |> ignore
        )
        |> ignore

        config
            .AddCommand<LintCommand>("lint")
            .WithDescription("Run the linter on the source code")
        |> ignore

        config
            .AddCommand<FormatCommand>("format")
            .WithDescription("Format the source code")
        |> ignore

    )

    app.Run(args)
