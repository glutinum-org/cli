module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("getSession", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member getSession (options: GetSessionOptions) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type PresentationOptions =
    abstract member title: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type GetSessionOptions =
    abstract member createIfNone: U2<bool, PresentationOptions> option with get, set
    abstract member forceNewSession: U2<bool, PresentationOptions> option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?createIfNone: U2<bool, PresentationOptions>, ?forceNewSession: U2<bool, PresentationOptions>) : GetSessionOptions = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
