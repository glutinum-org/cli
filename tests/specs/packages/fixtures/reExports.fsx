module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("createLogger", "re-exports")>]
    static member createLogger (?options: logger.Options) : logger.Logger = nativeOnly
    [<Import("isEnabled", "re-exports")>]
    static member isEnabled (level: float) : bool = nativeOnly
    [<Import("defaultLevel", "re-exports")>]
    static member inline defaultLevel: float = nativeOnly
    [<Import("configure", "re-exports")>]
    static member configure (config: DepLib.DepConfig) : unit = nativeOnly
    [<Import("Logger", "re-exports"); EmitConstructor>]
    static member Logger (?options: logger.Options) : Logger = nativeOnly

type Options =
    logger.Options

type LogLevel =
    logger.LogLevel

type Logger =
    logger.Logger

type OutputFormatter =
    formatter.Formatter

type Level =
    helpers.Level

type DepConfig =
    DepLib.DepConfig

[<AllowNullLiteral>]
[<Interface>]
type Sink =
    abstract member write: entry: string -> unit

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
        static member createLogger (?options: logger.Options) : logger.Logger = nativeOnly
        [<Import("Logger", "re-exports/logger.js"); EmitConstructor>]
        static member Logger (?options: logger.Options) : Logger = nativeOnly

    [<Global>]
    [<AllowNullLiteral>]
    type Options
        [<ParamObject; Emit("$0")>]
        (
            ?level: logger.LogLevel,
            ?formatter: formatter.Formatter
        ) =

        member val level : logger.LogLevel option = nativeOnly with get, set
        member val formatter : formatter.Formatter option = nativeOnly with get, set

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
