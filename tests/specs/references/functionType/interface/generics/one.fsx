module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject<'T> =
    abstract member upper: (string option -> 'T) with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (upper: (string option -> 'T)) : MyObject<'T> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
