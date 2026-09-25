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

    [<AllowNullLiteral>]
    [<Interface>]
    type ``mod`` =
        abstract member typescript: string with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (typescript: string) : ``mod`` = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
