module Build.Publish

open System
open System.IO
open SimpleExec
open BlackFox.CommandLine
open Build.Utils
open Build.Utils.Pnpm


let private publishNpm (projectDir: string) =
    let packageJsonPath = Path.Combine(projectDir, "package.json")
    let packageJsonContent = File.ReadAllText(packageJsonPath)
    let changelogPath = Path.Combine(projectDir, "CHANGELOG.md")

    let lastChangelogVersion =
        Changelog.getLastVersion changelogPath |> fun v -> v.Version.ToString()

    printfn $"Publishing: %s{projectDir}"

    if Npm.needPublishing packageJsonContent lastChangelogVersion then
        let updatedPackageJsonContent =
            Npm.replaceVersion packageJsonContent lastChangelogVersion

        File.WriteAllText(packageJsonPath, updatedPackageJsonContent)
        Pnpm.publish ()
        printfn $"Published!"
    else
        printfn $"Already up-to-date, skipping..."

let handle (_args: string list) =
    Test.Specs.handle []

    if (Directory.Exists "dist") then
        Directory.Delete("dist", true)

    Command.Run("dotnet", "fantomas src")

    Command.Run(
        "dotnet",
        CmdLine.empty
        |> CmdLine.appendRaw "fable"
        |> CmdLine.appendRaw "src/Glutinum.Converter.CLI"
        |> CmdLine.appendPrefix "--outDir" "dist"
        |> CmdLine.appendRaw "--sourceMaps"
        |> CmdLine.toString
    )

    publishNpm Environment.CurrentDirectory
