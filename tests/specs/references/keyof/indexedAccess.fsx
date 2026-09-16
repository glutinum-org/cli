module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("get", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member get<'T, 'K when 'K :> obj> (config: 'T, key: 'K) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Config =
    abstract member options: Config.options with get, set

module Config =

    [<Global>]
    [<AllowNullLiteral>]
    type options
        [<ParamObject; Emit("$0")>]
        (
            strict: bool
        ) =

        member val strict : bool = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
