module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type AlertStatic =
    abstract member alert: AlertStatic.alert with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (alert: AlertStatic.alert) : AlertStatic = nativeOnly

module AlertStatic =

    type alert =
        delegate of title: string * ?message: string -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
