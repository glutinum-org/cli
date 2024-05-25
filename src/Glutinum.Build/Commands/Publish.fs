module Build.Commands.Publish

open System
open System.IO
open SimpleExec
open BlackFox.CommandLine
open Build.Utils
open Build.Commands
open Build.Utils.Pnpm
open System.Text.RegularExpressions
open Spectre.Console.Cli
open System.ComponentModel

let cwd = Environment.CurrentDirectory

/// Updates the internal version of the project to the last version in the changelog
/// This is used to display the version on the Web app
let private updatePreludeVersion (newVersion: string) =
    let prelude = Path.Combine(cwd, "src", "Glutinum.Converter", "Prelude.fs")
    let preludeContent = File.ReadAllText(prelude)

    let newPreludeContent =
        Regex.Replace(
            preludeContent,
            $@"^(?'indentation'\s*)let VERSION = ""(?'version'.*?)""",
            (fun (m: Match) ->
                m.Groups.["indentation"].Value
                + $"let VERSION = \"{newVersion}\""
            ),
            RegexOptions.Multiline
        )

    File.WriteAllText(prelude, newPreludeContent)

let private publishWebApp (context) =
    // Compile the web app
    Web.WebCommand().Execute(context, Web.WebSettings()) |> ignore

    Command.Run("npx", "gh-pages -d ./src/Glutinum.Web/dist/")

type PublishSettings() =
    inherit CommandSettings()

    [<CommandOption("--web-only")>]
    [<Description("Publish the web app only

This is useful when there is a bug related to the Web app only
for which we need to publish a new version separately from the CLI tool")>]
    member val IsWebOnly: bool = false with get, set

type PublishCommand() =
    inherit Command<PublishSettings>()

    override _.Execute(context, settings) =
        if (Directory.Exists "dist") then
            Directory.Delete("dist", true)

        let packageJsonPath = Path.Combine(cwd, "package.json")
        let packageJsonContent = File.ReadAllText(packageJsonPath)
        let changelogPath = Path.Combine(cwd, "CHANGELOG.md")

        let lastChangelogVersion =
            Changelog.getLastVersion changelogPath
            |> fun v -> v.Version.ToString()

        updatePreludeVersion lastChangelogVersion

        // We allow to force the publishing of the web app only
        // The Web app doesn't have a CHANGELOG.md file, so in case there is a bug
        // to fix, we can force the publishing of the web app only
        if settings.IsWebOnly then
            publishWebApp context

        else if Npm.needPublishing packageJsonContent lastChangelogVersion then
            Command.Run(
                "dotnet",
                CmdLine.empty
                |> CmdLine.appendRaw "fable"
                |> CmdLine.appendRaw "src/Glutinum.Converter.CLI"
                |> CmdLine.appendPrefix "--outDir" "dist"
                |> CmdLine.appendRaw "--sourceMaps"
                |> CmdLine.appendRaw "--test:MSBuildCracker"
                |> CmdLine.toString
            )

            // Update package.json with the new version
            let updatedPackageJsonContent =
                Npm.replaceVersion packageJsonContent lastChangelogVersion

            File.WriteAllText(packageJsonPath, updatedPackageJsonContent)

            Pnpm.publish (noGitChecks = true, access = Publish.Access.Public)

            publishWebApp context
        else
            printfn $"Already up-to-date, skipping..."

        0
