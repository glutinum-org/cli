module Changelog

open System.IO

let tryGetLastVersion (changelogPath: string) =
    let content = File.ReadAllText changelogPath

    match ChangelogParser.parse content with
    | Ok changelog ->
        changelog.Versions |> List.tryFind (fun v -> v.Title <> "Unreleased")

    | Error msg ->
        failwithf
            $"""Failed to parse changelog:
File: %s{changelogPath}
Error:
%s{msg}"""

let getLastVersion (changelogPath: string) =
    tryGetLastVersion changelogPath
    |> Option.defaultWith (fun () ->
        failwithf
            $"""Failed to find version in changelog:
File: %s{changelogPath}"""
    )
