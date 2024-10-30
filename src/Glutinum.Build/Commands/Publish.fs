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
open Build.Utils.Dotnet
open Semver

[<RequireQualifiedAccess>]
type Project =
    | GlutinumCli
    | GlutinumWeb
    | GlutinumTypes
    | All

type ProjectTypeConverter() =
    inherit TypeConverter()

    override _.ConvertFrom(_: ITypeDescriptorContext, _, value: obj) =
        match value with
        | :? string as text ->
            match text with
            | "Glutinum.Converter.CLI"
            | "Glutinum.CLI"
            | "cli" -> Project.GlutinumCli
            | "Glutinum.Web"
            | "web" -> Project.GlutinumWeb
            | "Glutinum.Types"
            | "types" -> Project.GlutinumTypes
            | "all"
            | "All" -> Project.All
            | _ -> raise <| InvalidOperationException("Invalid project name")

        | _ -> raise <| InvalidOperationException("Invalid project name")

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

    [<CommandOption("--version")>]
    member val Version = None with get, set

    [<CommandOption("--project")>]
    [<TypeConverter(typeof<ProjectTypeConverter>)>]
    [<Description("""Name of the project to release

Possible values:
- Glutinum.Converter.CLI | Glutinum.CLI | cli
- Glutinum.Web | web
- Glutinum.Types | types
- All

If not specified, all projects will be released.
    """)>]
    member val Project = Project.All with get, set

    [<CommandOption("--allow-dirty")>]
    [<Description("Allow to run in a dirty repository (having not commit changes in your reporitory)")>]
    member val AllowDirty: bool = false with get, set

    [<CommandOption("--allow-branch <VALUES>")>]
    [<Description("List of branches that are allowed to be used to generate the changelog. Default is 'main'")>]
    member val AllowBranch: string array = [| "main" |] with get, set

    [<CommandOption("--dry-run")>]
    [<Description("Run the command without publishing the package. Allows to check the generated CHANGELOG.md (don't forget to revert the changes)")>]
    member val IsDryRun = false with get, set

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

let private getReleaseContext
    (repository: Repository)
    (settings: PublishSettings)
    (changelogPath: string)
    =
    let changelogContent =
        File.ReadAllText(changelogPath).Replace("\r\n", "\n").Split('\n')

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
                match settings.Project with
                | Project.GlutinumCli -> List.contains "cli" tags
                | Project.GlutinumWeb -> List.contains "web" tags
                | Project.GlutinumTypes -> List.contains "Glutinum.Types" tags
                | Project.All ->
                    failwith
                        "Can't get release context for the \"All\" projects case"
            | None -> false
        )

    let lastChangelogVersion = Changelog.tryFindLastVersion changelogPath

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
        | Ok version -> version.Version
        | Error Changelog.NoVersionFound -> SemVersion(0, 0, 0)
        | Error error -> error.ToText() |> failwith

    let newVersion =
        match settings.Version with
        | Some version -> SemVersion.Parse(version, SemVersionStyles.Strict)
        | None ->
            if shouldBumpMajor then
                refVersion
                    .WithMajor(refVersion.Major + 1)
                    .WithMinor(0)
                    .WithPatch(0)
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

let private updateChangelog
    (releaseContext: ReleaseContext)
    (changelogPath: string)
    =
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

    File.WriteAllText(changelogPath, newChangelogContent)

let private releaseWebOnly
    (repository: Repository)
    (context: CommandContext)
    (settings: PublishSettings)
    =
    // Force project to be GlutinumWeb
    settings.Project <- Project.GlutinumWeb

    let releaseContext =
        getReleaseContext repository settings Workspace.``CHANGELOG.md``

    updatePreludeVersion (releaseContext.NewVersion.ToString())

    Web.WebCommand().Execute(context, Web.WebSettings()) |> ignore

    // If we are not in a CI environment, we can publish the web app
    // Otherwise, we want to let the CI handle it
    if Environment.GetEnvironmentVariable("CI") = null then
        Command.Run("npx", "gh-pages -d ./src/Glutinum.Web/dist/")

        // Reset the changes made to the Prelude file, as we only needed it for the publish
        Command.Run("git", "checkout HEAD -- src/Glutinum.Converter/Prelude.fs")

let private releaseGlutinumCli
    (repository: Repository)
    (settings: PublishSettings)
    =

    let releaseContext =
        getReleaseContext repository settings Workspace.``CHANGELOG.md``

    updatePreludeVersion (releaseContext.NewVersion.ToString())

    if (Directory.Exists VirtualWorkspace.dist.``.``) then
        Directory.Delete(VirtualWorkspace.dist.``.``, true)

    updateChangelog releaseContext Workspace.``CHANGELOG.md``

    let packageJsonContent = File.ReadAllText(Workspace.``package.json``)

    // Update package.json with the new version
    let updatedPackageJsonContent =
        Npm.replaceVersion
            packageJsonContent
            (releaseContext.NewVersion.ToString())

    File.WriteAllText(Workspace.``package.json``, updatedPackageJsonContent)

    if settings.IsDryRun then
        printfn $"Dry run completed for project %A{settings.Project}"

        printfn
            "Please revert the changes after inspecting the generated CHANGELOG.md, package.json and Prelude.fs"

    else
        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendRaw "fable"
            |> CmdLine.appendRaw "src/Glutinum.Converter.CLI"
            |> CmdLine.appendPrefix "--outDir" VirtualWorkspace.dist.``.``
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

let private releaseGlutinumTypes
    (repository: Repository)
    (settings: PublishSettings)
    =

    let changelogPath = Workspace.src.``Glutinum.Types``.``CHANGELOG.md``
    let releaseContext = getReleaseContext repository settings changelogPath

    if Seq.isEmpty releaseContext.ReleaseCommits then
        printfn $"No commits to release for project %A{settings.Project}"

    else
        updateChangelog releaseContext changelogPath

        if settings.IsDryRun then
            printfn $"Dry run completed for project %A{settings.Project}"

            printfn
                "Please revert the changes after inspecting the generated CHANGELOG.md"
        else

            let projectFileInfo =
                FileInfo
                    Workspace.src.``Glutinum.Types``.``Glutinum.Types.fsproj``

            let binDir = projectFileInfo.DirectoryName + "/bin" |> DirectoryInfo

            if binDir.Exists then
                binDir.Delete(true)

            let nupkgPath = Dotnet.pack projectFileInfo.DirectoryName

            let nugetKey = Environment.GetEnvironmentVariable("NUGET_KEY")

            if isNull nugetKey then
                failwith "NUGET_KEY environment variable is not set"

            Nuget.push (
                nupkgPath,
                Environment.GetEnvironmentVariable("NUGET_KEY")
            )

type PublishCommand() =
    inherit Command<PublishSettings>()

    override _.Execute(context, settings) =
        // TODO: Replace libgit2sharp with using CLI directly
        // libgit2sharp seems all nice at first, but I find the API to be a bit cumbersome
        // when manipulating the repository for (commit, stage, etc.)
        // It also doesn't support SSH
        use repository = new Repository(Workspace.``.``)

        if
            not (
                Array.contains repository.Head.FriendlyName settings.AllowBranch
            )
        then
            failwith
                $"Branch '{repository.Head.FriendlyName}' is not allowed to make a release"

        if repository.RetrieveStatus().IsDirty && not settings.AllowDirty then
            failwith "You must commit your changes before publishing"

        // Test.Specs.SpecCommand().Execute(context, Test.Specs.SpecSettings())
        // |> ignore

        // let releaseContext = getReleaseContext repository settings
        // updatePreludeVersion (releaseContext.NewVersion.ToString())

        if settings.IsWebOnly then
            releaseWebOnly repository context settings
        else
            match settings.Project with
            | Project.GlutinumCli -> releaseGlutinumCli repository settings
            | Project.GlutinumWeb -> releaseWebOnly repository context settings
            | Project.GlutinumTypes -> releaseGlutinumTypes repository settings
            | Project.All ->
                releaseGlutinumCli repository settings
                releaseWebOnly repository context settings
                releaseGlutinumTypes repository settings

        0
