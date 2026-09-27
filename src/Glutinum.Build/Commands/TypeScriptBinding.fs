module Build.Commands.TypeScriptBinding

open System.IO
open System.Text.RegularExpressions
open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli

/// The TypeScript bundled with ts-morph is the compiler the converter runs on, pnpm keeps
/// `@ts-morph/common` out of the top-level `node_modules`
let private declarationFile () =
    let struct (output, _) =
        Command
            .ReadAsync(
                "node",
                CmdLine.empty
                |> CmdLine.appendRaw "-p"
                |> CmdLine.appendRaw
                    "\"require.resolve('@ts-morph/common/lib/typescript.d.ts', { paths: [require('path').dirname(require.resolve('@ts-morph/bootstrap'))] })\""
                |> CmdLine.toString
            )
            .GetAwaiter()
            .GetResult()

    output.Trim()

let private bindingFile = "src/Glutinum.Converter/TypeScript.fs"

type TypeScriptBindingCommand() =
    inherit Command<EmptyCommandSettings>()

    override _.Execute(context, settings) =
        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendRaw "src/Glutinum.Converter.CLI"
            |> CmdLine.appendPrefix "--outDir" "dist"
            |> CmdLine.toString
        )

        Command.Run(
            "node",
            CmdLine.empty
            |> CmdLine.appendRaw "--stack-size=8000"
            |> CmdLine.appendRaw "cli.js"
            |> CmdLine.appendRaw (declarationFile ())
            |> CmdLine.appendPrefix "--module-name" "TypeScript"
            |> CmdLine.appendPrefix "--out-file" bindingFile
            |> CmdLine.toString
        )

        // The converter refers to the `ts` namespace as `Ts`, its `ts` value is imported from `@ts-morph/bootstrap`
        let binding =
            Regex.Replace(File.ReadAllText bindingFile, @"\bts_\b", "Ts")
            |> _.Replace("REPLACE_ME_WITH_MODULE_NAME", "typescript")
            // The declarations marked `@deprecated` are referenced by the others
            |> _.Replace("module rec TypeScript\n", "module rec TypeScript\n\n#nowarn \"44\"\n")

        File.WriteAllText(bindingFile, binding)

        0
