#r "nuget: FsToolkit.ErrorHandling, 4.15.1"

open System.Text.RegularExpressions
open System
open System.IO
open FsToolkit.ErrorHandling

type Type =
    | Feat
    | Fix
    | CI
    | Chore
    | Docs
    | Test
    | Style
    | Refactor

    static member fromText(text: string) =
        match text with
        | "feat" -> Feat
        | "fix" -> Fix
        | "ci" -> CI
        | "chore" -> Chore
        | "docs" -> Docs
        | "test" -> Test
        | "style" -> Style
        | "refactor" -> Refactor
        | _ -> failwith $"Invalid scope: {text}"

type CommitMessage =
    {
        Type: Type
        Scope: string option
        Description: string
        BreakingChange: bool
    }

let private validateCommitMessage (commitMsg: string) =
    let commitRegex =
        Regex(
            "^(?<type>feat|fix|ci|chore|docs|test|style|refactor)(\(?<scope>.+?\))?(?<breakingChange>!)?: (?<description>.{1,})$"
        )

    let m = commitRegex.Match(commitMsg)

    if m.Success then
        let scope =
            if m.Groups.["scope"].Success then
                Some m.Groups.["scope"].Value
            else
                None

        {
            Type = Type.fromText m.Groups.["type"].Value
            Scope = scope
            Description = m.Groups.["description"].Value
            BreakingChange = m.Groups.["breakingChange"].Success
        }
        |> Ok
    else
        Error
            $"Invalid commit message format.

Expected a commit message with the following format: <type>[optional scope]: <description>

Where <type> is one of the following:
- feat: A new feature
- fix: A bug fix
- ci: Changes to CI/CD configuration
- chore: Changes to the build process or external dependencies
- docs: Documentation changes
- test: Adding or updating tests
- style: Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
- refactor: A code change that neither fixes a bug nor adds a feature

Example:
-------------------------
feat: add new feature
-------------------------"

let private validateSecondLine (line: string) =
    if String.IsNullOrWhiteSpace line then
        Ok()
    else
        Error
            "Invalid commit message format.

Expected an empty line after the commit message.

Example:
-------------------------
feat: add new feature

-------------------------"

let private validateTagLine (tagLine: string) (commitMessage: CommitMessage) =
    match commitMessage.Type with
    | Feat
    | Fix
    | Test
    | Style
    | Refactor
    | Docs ->
        let tagLineRegex = Regex("^(\[converter\]|\[web\])+$")

        if tagLineRegex.IsMatch(tagLine) then
            Ok()
        else
            Error
                "Invalid commit message format.

Expected a tag line with either [converter] or [web] (or both).

Example:
-------------------------
feat: add new feature

[converter]
-------------------------"

    // Tag line is not required for the following scopes
    | CI
    | Chore -> Ok()

let private validate commit secondLine tagLine =
    let result =
        result {
            let! commitMessage = validateCommitMessage commit
            do! validateSecondLine secondLine
            do! validateTagLine tagLine commitMessage
        }

    match result with
    | Ok _ -> exit 0
    | Error error ->
        printfn $"%s{error}"
        exit 1

let private args =
    Environment.GetCommandLineArgs()
    |> Array.toList
    // Skip fsi.dll and script name
    |> List.skip 2

match args with
| commitMsgFile :: [] ->
    let commitMsgLines = File.ReadAllLines(commitMsgFile) |> Array.toList

    match commitMsgLines with
    // Standard case
    | commit :: secondLine :: tagLine :: _ -> validate commit secondLine tagLine
    // Give a chance to be a valid commit message if the type allows it
    | commit :: [] -> validate commit "" ""

    | _ ->
        printfn
            "Invalid commit message format, expected a commit message with a tag line"

        exit 1

| _ ->
    printfn
        "Invalid arguments, expected a single argument with the path to the commit message file"

    exit 1
