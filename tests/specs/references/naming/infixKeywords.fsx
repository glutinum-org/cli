module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type PluginModuleFactory =
    delegate of ``mod``: PluginModuleFactory.``mod`` * ?``land``: float -> string

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Quotes =
    | [<CompiledName("'")>] _APOSTROPHE_
    | [<CompiledName("`")>] _BACKTICK_

module PluginModuleFactory =

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
