namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module ReExports =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("createLogger", "re-exports")>]
        static member createLogger (?options: ReExports.logger.Options) : ReExports.logger.Logger = nativeOnly
        [<Import("isEnabled", "re-exports")>]
        static member isEnabled (level: float) : bool = nativeOnly
        [<Import("defaultLevel", "re-exports")>]
        static member inline defaultLevel: float = nativeOnly
        [<Import("configure", "re-exports")>]
        static member configure (config: DepLib.DepConfig) : unit = nativeOnly
        [<Import("Logger", "re-exports"); EmitConstructor>]
        static member Logger (?options: ReExports.logger.Options) : Logger = nativeOnly

    type Options =
        ReExports.logger.Options

    type LogLevel =
        ReExports.logger.LogLevel

    type Logger =
        ReExports.logger.Logger

    type OutputFormatter =
        ReExports.formatter.Formatter

    type Level =
        ReExports.helpers.Level

    type DepConfig =
        DepLib.DepConfig

    type Kind =
        ReExports.compiler.compiler_.Kind

    type Node =
        ReExports.compiler.compiler_.Node

    [<AllowNullLiteral>]
    [<Interface>]
    type Sink =
        abstract member write: entry: string -> unit

    module compiler =

        module compiler_ =

            [<RequireQualifiedAccess>]
            type Kind =
                | A = 1
                | B = 2

            [<AllowNullLiteral>]
            [<Interface>]
            type Node =
                abstract member kind: ReExports.compiler.compiler_.Kind with get, set

    module formatter =

        [<AllowNullLiteral>]
        [<Interface>]
        type Formatter =
            abstract member format: message: string -> string

    module helpers =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("defaultLevel", "re-exports/helpers.js")>]
            static member inline defaultLevel: float = nativeOnly
            [<Import("isEnabled", "re-exports/helpers.js")>]
            static member isEnabled (level: float) : bool = nativeOnly

        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.None)>]
        type Level =
            | debug
            | info

    module logger =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("createLogger", "re-exports/logger.js")>]
            static member createLogger (?options: ReExports.logger.Options) : ReExports.logger.Logger = nativeOnly
            [<Import("Logger", "re-exports/logger.js"); EmitConstructor>]
            static member Logger (?options: ReExports.logger.Options) : Logger = nativeOnly

        [<Global>]
        [<AllowNullLiteral>]
        type Options
            [<ParamObject; Emit("$0")>]
            (
                ?level: ReExports.logger.LogLevel,
                ?formatter: ReExports.formatter.Formatter
            ) =

            member val level : ReExports.logger.LogLevel option = nativeOnly with get, set
            member val formatter : ReExports.formatter.Formatter option = nativeOnly with get, set

        [<RequireQualifiedAccess>]
        type LogLevel =
            | Debug = 0
            | Info = 1

        [<AllowNullLiteral>]
        [<Interface>]
        type Logger =
            abstract member log: message: string -> unit

module DepLib =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("configure", "dep-lib")>]
        static member configure (config: DepLib.DepConfig) : unit = nativeOnly

    [<Global>]
    [<AllowNullLiteral>]
    type DepConfig
        [<ParamObject; Emit("$0")>]
        (
            retries: float
        ) =

        member val retries : float = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
