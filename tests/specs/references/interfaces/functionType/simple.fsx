module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type AlertStatic =
    abstract member alert: AlertStatic.alert with get, set

module AlertStatic =

    type alert =
        delegate of title: string * ?message: string -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
