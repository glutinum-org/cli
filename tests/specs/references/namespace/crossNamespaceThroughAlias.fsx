module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module hook_ =

    [<AllowNullLiteral>]
    [<Interface>]
    type Event =
        abstract member next: unit -> unit

module fs_ =

    type _alias =
        hook_.Event

    [<AllowNullLiteral>]
    [<Interface>]
    type DeleteEvent =
        inherit hook_.Event
        abstract member key: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
