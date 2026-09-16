module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type PluginModuleFactory =
    [<Emit("$0($1...)")>]
    abstract member Invoke: ``mod``: PluginModuleFactory.Invoke.``mod`` * ?``land``: float -> string

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Quotes =
    | [<CompiledName("'")>] _APOSTROPHE_
    | [<CompiledName("`")>] _BACKTICK_

module PluginModuleFactory =

    module Invoke =

        [<Global>]
        [<AllowNullLiteral>]
        type ``mod``
            [<ParamObject; Emit("$0")>]
            (
                typescript: string
            ) =

            member val typescript : string = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
