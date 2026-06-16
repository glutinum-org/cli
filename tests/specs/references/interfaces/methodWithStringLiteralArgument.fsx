module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Page =
    [<Emit("$0.on('console',$1...)")>]
    abstract member on_console: listener: (unit -> obj) -> Page
    [<Emit("$0.on('ready')")>]
    abstract member on_ready: unit -> Page

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
