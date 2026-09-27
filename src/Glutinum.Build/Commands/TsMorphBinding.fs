module Build.Commands.TsMorphBinding

open System.IO
open System.Text.RegularExpressions
open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli

let private bindingFile = "src/Glutinum.Converter/TsMorph.fs"

/// The surface of `@ts-morph/bootstrap` the converter drives ts-morph through
let private included =
    "createProjectSync,createProject,Project,ProjectOptions,FileSystemHost,InMemoryFileSystemHost"

/// The `ts` compiler enums `CompilerOptions` names without qualification once expanded
let private tsEnums =
    [
        "ImportsNotUsedAsValues"
        "JsxEmit"
        "ModuleKind"
        "ModuleResolutionKind"
        "ModuleDetectionKind"
        "NewLineKind"
        "MapLike"
        "ScriptTarget"
    ]

let private declarationFile () =
    let struct (output, _) =
        Command
            .ReadAsync(
                "node",
                CmdLine.empty
                |> CmdLine.appendRaw "-p"
                |> CmdLine.appendRaw
                    "\"require.resolve('@ts-morph/bootstrap/lib/ts-morph-bootstrap.d.ts', { paths: [process.cwd()] })\""
                |> CmdLine.toString
            )
            .GetAwaiter()
            .GetResult()

    output.Trim()

type TsMorphBindingCommand() =
    inherit Command<EmptyCommandSettings>()

    override _.Execute(context, settings) =
        Command.Run(
            "node",
            CmdLine.empty
            |> CmdLine.appendRaw "--stack-size=8000"
            |> CmdLine.appendRaw "cli.js"
            |> CmdLine.appendRaw (declarationFile ())
            |> CmdLine.appendPrefix "--module-name" "TsMorph"
            |> CmdLine.appendPrefix "--include" included
            |> CmdLine.appendPrefix "--out-file" bindingFile
            |> CmdLine.toString
        )

        let binding =
            // The converter refers to the `ts` namespace as `Ts`, from the `TypeScript` binding
            Regex.Replace(File.ReadAllText bindingFile, @"\bts_\b", "Ts")
            |> _.Replace("REPLACE_ME_WITH_MODULE_NAME", "@ts-morph/bootstrap")
            |> _.Replace("module rec TsMorph\n", "module rec TsMorph\n\n#nowarn \"44\"\n")
            |> fun content ->
                // The expanded `CompilerOptions` names the `ts` enums without qualification
                (content, tsEnums)
                ||> List.fold (fun content name ->
                    Regex.Replace(content, @"(?<![.\w])" + name + @"\b", "Ts." + name)
                )

        // `RuntimeDirEntry` comes from `@ts-morph/common`, referenced but not declared
        let binding =
            let lines = binding.Replace("\r\n", "\n").Split('\n') |> Array.toList
            let lastOpen = lines |> List.findIndexBack (fun l -> l.StartsWith "open ")

            let injected =
                [
                    "open TypeScript"
                    ""
                    "/// A `readDirSync` entry, from `@ts-morph/common`"
                    "type RuntimeDirEntry ="
                    "    abstract member name: string with get, set"
                    "    abstract member isFile: bool with get, set"
                    "    abstract member isDirectory: bool with get, set"
                    "    abstract member isSymlink: bool with get, set"
                ]

            List.concat [ lines[..lastOpen]; injected; lines[lastOpen + 1 ..] ]
            |> String.concat "\n"

        File.WriteAllText(bindingFile, binding)

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fantomas"
            |> CmdLine.appendRaw bindingFile
            |> CmdLine.toString
        )

        0
