module Build.Tasks.Test.Integration

open BlackFox.CommandLine
open SimpleExec
open Build.Utils.Pnpm
open Build.Tasks

// For now, we don't have any integration tests
// I wanted to do it for dayjs, but looking at the repository,
// it seems that the library is dead
// So I don't think it's worth it to spend time on that specific library

let handle (args: string list) =
    let isWatch = args |> List.contains "--watch"

    // We always need to generate the specs test files
    Pnpm.install ()

    // Make sure we have the latest build of the CLI
    Cli.handle []

    // Generate bindings for libraries
    // Command.Run(
    //     "node",
    //     CmdLine.empty
    //     |> CmdLine.appendRaw "cli.js"
    //     |> CmdLine.appendRaw "node_modules/dayjs/index.d.ts"
    //     |> CmdLine.appendPrefix
    //         "--out-file"
    //         "tests/integrations/glues/Glutinum.Dayjs.fs"
    //     |> CmdLine.toString
    // )

    // Adapt the imports of the bindings (Glutinum doesn't automatically infer the module name yet)

    let additionalArgs =
        if isWatch then
            let candidates = args |> List.skipWhile (fun x -> x <> "--")

            if List.isEmpty candidates then
                null
            else
                candidates |> List.skip 1 |> String.concat " "
        else
            "run"

    let vitestCmd =
        CmdLine.empty
        |> CmdLine.appendRaw "npx"
        |> CmdLine.appendRaw "vitest"
        |> CmdLine.appendRaw additionalArgs
        |> CmdLine.toString

    let fableCmd =
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendIf isWatch "watch"
        |> CmdLine.appendPrefix "--outDir" "fableBuild"
        |> CmdLine.appendRaw "--sourceMaps"
        // Avoid strange logs because both Fable and Vitest rewrite the console
        |> CmdLine.appendRaw "--verbose"
        |> CmdLine.appendRaw "--run"
        |> CmdLine.appendRaw vitestCmd
        |> CmdLine.toString

    Command.Run("dotnet", fableCmd, workingDirectory = "tests/integrations")
