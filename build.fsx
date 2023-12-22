#r "nuget: Fun.Build, 1.0.5"
#r "nuget: BlackFox.CommandLine, 1.0.0"

open Fun.Build
open BlackFox.CommandLine

type PipelineBuilder with

    [<CustomOperation "collapseGithubActionLogs">]
    member inline this.collapseGithubActionLogs(build: Internal.BuildPipeline) =
        let build =
            this.runBeforeEachStage (
                build,
                (fun ctx ->
                    if ctx.TryGetEnvVar("GITHUB_ACTION").IsSome then
                        if ctx.GetStageLevel() = 0 then
                            printfn $"::group::{ctx.Name}"
                )
            )

        this.runAfterEachStage (
            build,
            (fun ctx ->
                if ctx.TryGetEnvVar("GITHUB_ACTION").IsSome then
                    if ctx.GetStageLevel() = 0 then
                        printfn "::endgroup::"
            )
        )

let options =
    {|
        GithubAction =
            EnvArg.Create(
                "GITHUB_ACTION",
                description = "Run only in in github action container"
            )
        Watch = CmdArg.Create("-w", "--watch", "Watch for changes")
    |}

module Stages =

    let lint =
        stage "Lint" {
            stage "Format" {
                whenNot { envVar options.GithubAction }
                run "dotnet fantomas src"
            }

            stage "Check" {
                whenEnvVar options.GithubAction
                run "dotnet fantomas --check src"
            }
        }

    let clean = stage "Clean" { run "npx shx rm -rf ./dist" }

    let build =
        stage "Build" {
            run "dotnet fable src/Glutinum.Converter.CLI --outDir dist"
        }

    let test =
        let fableCmd (isWatch: bool) =
            let vitestCmd =
                CmdLine.empty
                |> CmdLine.appendRaw "npx"
                |> CmdLine.appendRaw "vitest"
                |> CmdLine.appendIf (not isWatch) "run"
                |> CmdLine.toString

            CmdLine.empty
            |> CmdLine.appendRaw "dotnet"
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendIf isWatch "watch"
            |> CmdLine.appendRaw "src/Glutinum.Converter"
            |> CmdLine.appendPrefix "--outDir" "fableBuild"
            |> CmdLine.appendRaw "--sourceMaps"
            |> CmdLine.appendPrefix "--run" vitestCmd
            |> CmdLine.toString

        stage "Test" {
            stage "Watch" {
                whenCmdArg options.Watch
                run (fableCmd true)
            }

            stage "Run" {
                whenNot { cmdArg options.Watch }
                run (fableCmd false)
            }
        }

pipeline "Restore dependencies" {
    description "Restore dependencies"
    collapseGithubActionLogs

    stage "Restore" {
        run "dotnet tool restore"
        run "pnpm install"
        run "dotnet husky install"
    }

    runImmediate
}

pipeline "Test" {
    collapseGithubActionLogs

    Stages.lint
    Stages.clean
    Stages.build
    Stages.test

    runIfOnlySpecified
}

pipeline "Build" {
    description "Build local package"
    collapseGithubActionLogs

    Stages.lint
    Stages.clean
    Stages.build

    runIfOnlySpecified
}

pipeline "Release" {
    description "Build and deploy to NPM"
    collapseGithubActionLogs

    Stages.lint
    Stages.clean
    Stages.build
    Stages.test

    stage "Publish" {
        run "node scripts/release-pnpm.mjs"
        run "pnpm run publish"
    }

    runIfOnlySpecified
}

tryPrintPipelineCommandHelp ()
