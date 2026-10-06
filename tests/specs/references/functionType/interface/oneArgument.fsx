module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member upper: (string -> string) with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (upper: (string -> string)) : MyObject = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
