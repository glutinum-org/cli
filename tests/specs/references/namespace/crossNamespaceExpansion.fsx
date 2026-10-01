module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module core_ =

    [<AllowNullLiteral>]
    [<Interface>]
    type Model =
        abstract member id: string with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (id: string) : Model = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type App =
        abstract member modelQuery: m: Model -> string
        abstract member onBoot: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type withoutBoot<'T> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type CoreApp =
    abstract member modelQuery: m: core_.Model -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
