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

type PublishSettings() =
    inherit CommandSettings()

type PublishCommand() =
    inherit Command<PublishSettings>()

    override _.Execute(context, settings_) =

        0
