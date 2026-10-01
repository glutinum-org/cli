module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type excludeHooks<'Type> =
    [<EmitIndexer>]
    abstract member Item: key: string -> obj with get, set

module core_ =

    [<AllowNullLiteral>]
    [<Interface>]
    type App =
        abstract member pb: unit -> pb_.PocketBase
        abstract member onBoot: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type CoreApp =
    abstract member pb: unit -> pb_.PocketBase

module pb_ =

    [<AllowNullLiteral>]
    [<Interface>]
    type PocketBase =
        inherit excludeHooks<core_.App>
        abstract member start: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type PocketBase =
    inherit excludeHooks<pb_.PocketBase>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
