module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("withDefault", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member withDefault (?options: Options) : unit = nativeOnly
    [<Import("withoutDefault", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member withoutDefault (?options: Options) : unit = nativeOnly
    [<Import("required", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member required (options: Options option) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member step: float option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?step: float) : Options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
