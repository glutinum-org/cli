module Build.Utils.Pnpm

open SimpleExec
open System.Text.RegularExpressions
open BlackFox.CommandLine

type Pnpm =

    static member install() = Command.Run("pnpm", "install")

    static member publish(?projectDir: string) =
        Command.Run("pnpm", "publish", ?workingDirectory = projectDir)
