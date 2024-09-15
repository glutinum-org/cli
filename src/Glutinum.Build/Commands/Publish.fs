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
open Build.Workspace
open LibGit2Sharp
open EasyBuild.CommitParser
open System.Linq
open EasyBuild.CommitParser.Types
open Thoth.Json.Newtonsoft
open EasyBuild.CommitParser.Types
open Semver

let cwd = Environment.CurrentDirectory

/// Updates the internal version of the project to the last version in the changelog
/// This is used to display the version on the Web app
let private updatePreludeVersion (newVersion: string) =
    let preludePath = Workspace.src.``Glutinum.Converter``.``Prelude.fs``
    let preludeContent = File.ReadAllText(preludePath)

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

    File.WriteAllText(preludePath, newPreludeContent)

let private capitalizeFirstLetter (text: string) =
    (string text.[0]).ToUpper() + text.[1..]

type PublishSettings() =
    inherit CommandSettings()

    [<CommandOption("--web-only")>]
    [<Description("Publish the web app only

This is useful when there is a bug related to the Web app only
for which we need to publish a new version separately from the CLI tool")>]
    member val IsWebOnly: bool = false with get, set

    [<CommandOption("--major")>]
    member val BumpMajor = false with get, set

    [<CommandOption("--minor")>]
    member val BumpMinor = false with get, set

    [<CommandOption("--patch")>]
    member val BumpPatch = false with get, set

type ReleaseContext =
    {
        NewVersion: SemVersion
        ReleaseCommits:
            {|
                OriginalCommit: Commit
                SemanticCommit: CommitMessage
            |} seq
        ChangelogContent: string[]
        LastCommitSha: string
    }

let private getReleaseContext (settings: PublishSettings) =
    // TODO: Replace libgit2sharp with using CLI directly
    // libgit2sharp seems all nice at first, but I find the API to be a bit cumbersome
    // when manipulating the repository for (commit, stage, etc.)
    // It also doesn't support SSH
    use repository = new Repository(Workspace.``.``)

    if repository.Head.FriendlyName <> "main" then
        failwith "You must be on the main branch to publish"

    if repository.RetrieveStatus().IsDirty then
        failwith "You must commit your changes before publishing"

    let changelogContent =
        File
            .ReadAllText(Workspace.``CHANGELOG.md``)
            .Replace("\r\n", "\n")
            .Split('\n')

    let changelogConfigSection =
        changelogContent
        |> Array.skipWhile (fun line -> "<!-- EasyBuild: START -->" <> line)
        |> Array.takeWhile (fun line -> "<!-- EasyBuild: END -->" <> line)

    let lastReleasedCommit =
        let regex = Regex("^<!-- last_commit_released:\s(?'hash'\w*) -->$")

        changelogConfigSection
        |> Array.tryPick (fun line ->
            let m = regex.Match(line)

            if m.Success then
                Some m.Groups.["hash"].Value
            else
                None
        )

    let commitFilter = CommitFilter()

    // If we found a last released commit, use it as the starting point
    // Otherwise, not providing a starting point seems to get all commits
    if lastReleasedCommit.IsSome then
        commitFilter.ExcludeReachableFrom <- lastReleasedCommit.Value

    let commits = repository.Commits.QueryBy(commitFilter).ToList()

    let commitParserConfig =
        Workspace.``commit-linter.json``
        |> File.ReadAllText
        |> Decode.unsafeFromString CommitParserConfig.decoder

    let releaseCommits =
        commits
        // Parse the commit message
        |> Seq.choose (fun commit ->
            match
                Parser.tryParseCommitMessage commitParserConfig commit.Message,
                commit
            with
            | Ok semanticCommit, commit ->
                Some
                    {|
                        OriginalCommit = commit
                        SemanticCommit = semanticCommit
                    |}
            | Error _, _ -> None
        )
        // Only include commits that are feat or fix
        |> Seq.filter (fun commits ->
            match commits.SemanticCommit.Type with
            | "feat"
            | "fix" -> true
            | _ -> false
        )
        // Only keep the commits related to the tool we are releasing
        // Each tool should have its own generation but I am waiting for EasyBuild.ChangelogGen to be ready
        // for implementing this correctly.
        // For now, this is good enough because we only have a single changelog
        |> Seq.filter (fun commits ->
            match commits.SemanticCommit.Tags with
            | Some tags ->
                if settings.IsWebOnly then
                    List.contains "converter" tags || List.contains "web" tags
                else
                    List.contains "converter" tags || List.contains "cli" tags
            | None -> false
        )

    let lastChangelogVersion =
        Changelog.tryGetLastVersion Workspace.``CHANGELOG.md``

    let shouldBumpMajor =
        settings.BumpMajor
        || releaseCommits
           |> Seq.exists (fun commit -> commit.SemanticCommit.BreakingChange)

    let shouldBumpMinor =
        settings.BumpMinor
        || releaseCommits
           |> Seq.exists (fun commit -> commit.SemanticCommit.Type = "feat")

    let shouldBumpPatch =
        settings.BumpPatch
        || releaseCommits
           |> Seq.exists (fun commit -> commit.SemanticCommit.Type = "fix")

    let refVersion =
        match lastChangelogVersion with
        | Some version -> version.Version
        | None -> SemVersion(0, 0, 0)

    let newVersion =
        if shouldBumpMajor then
            refVersion.WithMajor(refVersion.Major + 1).WithMinor(0).WithPatch(0)
        elif shouldBumpMinor then
            refVersion.WithMinor(refVersion.Minor + 1).WithPatch(0)
        elif shouldBumpPatch then
            refVersion.WithPatch(refVersion.Patch + 1)
        else if
            // On CI, we allow to publish without a version bump
            // It happens when we just released a new stable version, the changelog is already up-to-date
            Environment.GetEnvironmentVariable("CI") <> null
            || settings.IsWebOnly
        then
            refVersion
        else
            failwith "No version bump required"

    if settings.IsWebOnly then
        {
            NewVersion = newVersion.WithPrerelease("preview")
            ReleaseCommits = releaseCommits
            ChangelogContent = changelogContent
            LastCommitSha = commits[0].Sha
        }

    else
        {
            NewVersion = newVersion
            ReleaseCommits = releaseCommits
            ChangelogContent = changelogContent
            LastCommitSha = commits[0].Sha
        }

let private tryFindAdditionalChangelogContent (text: string) =
    let lines = text.Replace("\r\n", "\n").Split('\n') |> Seq.toList

    let rec apply
        (acc: string list)
        (lines: string list)
        (isInsideChangelogBlock: bool)
        =
        match lines with
        | [] -> acc
        | line :: rest ->
            if isInsideChangelogBlock then
                if line = "=== changelog ===" then
                    apply acc rest false
                else
                    apply (acc @ [ line ]) rest true
            else if line = "=== changelog ===" then
                apply acc rest true
            else
                apply acc rest false

    apply [] lines false

let private updateChangelog (releaseContext: ReleaseContext) =
    let newVersionLines = ResizeArray<string>()

    let appendLine (line: string) = newVersionLines.Add(line)

    let newLine () = newVersionLines.Add("")

    appendLine ($"## {releaseContext.NewVersion}")
    newLine ()

    releaseContext.ReleaseCommits
    |> Seq.groupBy (fun commit -> commit.SemanticCommit.Type)
    |> Seq.iter (fun (commitType, commits) ->
        match commitType with
        | "feat" -> appendLine "### 🚀 Features"
        | "fix" -> appendLine "### 🐞 Bug Fixes"
        | _ -> ()

        newLine ()

        for commit in commits do
            let githubCommitUrl sha =
                $"https://github.com/glutinum-org/cli/commit/%s{sha}"

            let shortSha = commit.OriginalCommit.Sha.Substring(0, 7)

            let commitUrl = githubCommitUrl commit.OriginalCommit.Sha

            let description =
                capitalizeFirstLetter commit.SemanticCommit.Description

            $"* %s{description} ([%s{shortSha}](%s{commitUrl}))" |> appendLine

            let additionalChangelogContent =
                tryFindAdditionalChangelogContent commit.OriginalCommit.Message
                // Indent the additional lines to be under item bullet point
                |> List.map (fun line -> $"    %s{line}")
                // Trim empty lines
                |> List.map (fun line ->
                    if String.IsNullOrWhiteSpace line then
                        ""
                    else
                        line
                )

            if not additionalChangelogContent.IsEmpty then
                appendLine ""
                additionalChangelogContent |> List.iter appendLine
                appendLine ""

        newLine ()
    )

    newLine ()

    // TODO: Add contributors list
    // TODO: Add breaking changes list

    let rec removeConsecutiveEmptyLines
        (previousLineWasBlank: bool)
        (result: string list)
        (lines: string list)
        =
        match lines with
        | [] -> result
        | line :: rest ->
            // printfn $"%A{String.IsNullOrWhiteSpace(line)}"
            if previousLineWasBlank && String.IsNullOrWhiteSpace(line) then
                removeConsecutiveEmptyLines true result rest
            else
                removeConsecutiveEmptyLines
                    (String.IsNullOrWhiteSpace(line))
                    (result @ [ line ])
                    rest

    let newChangelogContent =
        [
            // Add title and description of the original changelog
            yield!
                releaseContext.ChangelogContent
                |> Seq.takeWhile (fun line ->
                    "<!-- EasyBuild: START -->" <> line
                )

            // Ad EasyBuild metadata
            "<!-- EasyBuild: START -->"
            $"<!-- last_commit_released: {releaseContext.LastCommitSha} -->"
            "<!-- EasyBuild: END -->"
            ""

            // New version
            yield! newVersionLines

            // Add the rest of the changelog
            yield!
                releaseContext.ChangelogContent
                |> Seq.skipWhile (fun line -> not (line.StartsWith("##")))
        ]
        |> removeConsecutiveEmptyLines false []
        |> String.concat "\n"

    File.WriteAllText(Workspace.``CHANGELOG.md``, newChangelogContent)

type PublishCommand() =
    inherit Command<PublishSettings>()

    override _.Execute(context, settings) =
        Test.Specs.SpecCommand().Execute(context, Test.Specs.SpecSettings())
        |> ignore

        let releaseContext = getReleaseContext settings
        updatePreludeVersion (releaseContext.NewVersion.ToString())

        if settings.IsWebOnly then
            Web.WebCommand().Execute(context, Web.WebSettings()) |> ignore

            // If we are not in a CI environment, we can publish the web app
            // Otherwise, we want to let the CI handle it
            if Environment.GetEnvironmentVariable("CI") = null then
                Command.Run("npx", "gh-pages -d ./src/Glutinum.Web/dist/")

                // Reset the changes to the Prelude file, as we only needed it for the publish
                Command.Run(
                    "git",
                    "checkout HEAD -- src/Glutinum.Converter/Prelude.fs"
                )

            0
        else if Seq.isEmpty releaseContext.ReleaseCommits then
            printfn $"No new commits to release, skipping..."
            0
        else

            if (Directory.Exists "dist") then
                Directory.Delete("dist", true)

            updateChangelog releaseContext

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

            let fableModuleGitignore =
                FileInfo(VirtualWorkspace.dist.fable_modules.``.gitignore``)

            // We need to delete the fable_modules/.gitignore file so NPM pack
            // includes the fable_modules directory in the tarball
            if fableModuleGitignore.Exists then
                fableModuleGitignore.Delete()

            let packageJsonContent =
                File.ReadAllText(Workspace.``package.json``)

            // Update package.json with the new version
            let updatedPackageJsonContent =
                Npm.replaceVersion
                    packageJsonContent
                    (releaseContext.NewVersion.ToString())

            File.WriteAllText(
                Workspace.``package.json``,
                updatedPackageJsonContent
            )

            Pnpm.publish (noGitChecks = true, access = Publish.Access.Public)

            // Web app is going to be published via CI
            // Web.WebCommand().Execute(context, Web.WebSettings()) |> ignore
            // Command.Run("npx", "gh-pages -d ./src/Glutinum.Web/dist/")

            // Because we messed with Fable output, prefer to clean up
            // So Fable will start from scratch next time
            let fableModuleDir =
                DirectoryInfo(VirtualWorkspace.dist.fable_modules.``.``)

            if fableModuleDir.Exists then
                fableModuleDir.Delete(true)

            Command.Run("git", "add .")

            Command.Run(
                "git",
                CmdLine.empty
                |> CmdLine.appendRaw "commit"
                |> CmdLine.appendPrefix
                    "-m"
                    $"chore: release %s{releaseContext.NewVersion.ToString()}"
                |> CmdLine.toString
            )

            Command.Run("git", "push")

            0
