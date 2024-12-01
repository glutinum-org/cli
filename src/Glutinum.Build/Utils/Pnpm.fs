module Build.Utils.Pnpm

open SimpleExec
open System.Text.RegularExpressions
open BlackFox.CommandLine

module Publish =

    [<RequireQualifiedAccess>]
    type Access =
        | Public
        | Restricted

        member this.Text =
            match this with
            | Public -> "public"
            | Restricted -> "restricted"

type Pnpm =

    static member install() = Command.Run("pnpm", "install")

    static member publish(?projectDir: string, ?noGitChecks: bool, ?access: Publish.Access) =
        let noGitChecks = defaultArg noGitChecks false

        Command.Run(
            "pnpm",
            CmdLine.empty
            |> CmdLine.appendRaw "publish"
            |> CmdLine.appendIf noGitChecks "--no-git-checks"
            |> CmdLine.appendPrefixIf access.IsSome "--access" access.Value.Text
            |> CmdLine.toString,
            ?workingDirectory = projectDir
        )
