module Build.Commands.Release

open System
open System.IO
open System.Text.RegularExpressions
open SimpleExec
open BlackFox.CommandLine
open Spectre.Console.Cli
open Build.Utils.Pnpm
open Build.Workspace
open EasyBuild.Tools.DotNet
open EasyBuild.Tools.Fable

// npm tags a published version `latest` unless a dist-tag is given, even for a pre-release
let private npmTag () =
    let packageJson = File.ReadAllText(Path.Combine(root, "package.json"))

    let version =
        Regex.Match(packageJson, "\"version\"\\s*:\\s*\"(?<v>[^\"]+)\"").Groups.["v"].Value

    let prerelease = Regex.Match(version, "-(?<id>[0-9A-Za-z]+)")

    if prerelease.Success then
        Some prerelease.Groups.["id"].Value
    else
        None

// A package is pushed after the ones it references, so a restore never sees a missing dependency
let private packages =
    [
        Workspace.src.``Glutinum.Types``.``.``
        Workspace.bindings.``Glutinum.Web``.``.``
        Workspace.bindings.``Glutinum.Node``.``.``
    ]

type ReleaseSettings() =
    inherit CommandSettings()

type ReleaseCommand() =
    inherit Command<ReleaseSettings>()
    interface ICommandLimiter<CommandSettings>

    override _.Execute(_, _) =
        let apiKey = Environment.GetEnvironmentVariable "NUGET_KEY"

        if String.IsNullOrWhiteSpace apiKey then
            printfn "NUGET_KEY is not set"
            1
        else
            if Directory.Exists VirtualWorkspace.nupkgs.``.`` then
                Directory.Delete(VirtualWorkspace.nupkgs.``.``, true)

            for package in packages do
                let nupkg = DotNet.pack package

                DotNet.nugetPush (
                    nupkg,
                    apiKey = apiKey,
                    source = "https://api.nuget.org/v3/index.json",
                    skipDuplicate = true
                )

            if Directory.Exists VirtualWorkspace.dist.``.`` then
                Directory.Delete(VirtualWorkspace.dist.``.``, true)

            Fable.build (
                Workspace.src.``Glutinum.Converter.CLI``.``.``,
                noGitignore = true,
                noCache = true,
                outDir = VirtualWorkspace.dist.``.``
            )

            Pnpm.publish (noGitChecks = true, access = Publish.Access.Public, ?tag = npmTag ())

            0
