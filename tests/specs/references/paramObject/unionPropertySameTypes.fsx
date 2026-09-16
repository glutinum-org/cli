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

[<Global>]
[<AllowNullLiteral>]
type GetSessionOptions
    [<ParamObject; Emit("$0")>]
    (
        ?createIfNone: U2<bool, PresentationOptions>,
        ?forceNewSession: U2<bool, PresentationOptions>
    ) =

    member val createIfNone : U2<bool, PresentationOptions> option = nativeOnly with get, set
    member val forceNewSession : U2<bool, PresentationOptions> option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
