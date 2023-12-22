module Build.Utils.Pnpm

open SimpleExec
open System.Text.RegularExpressions
open BlackFox.CommandLine

type Pnpm =

    static member install() = Command.Run("pnpm", "install")
