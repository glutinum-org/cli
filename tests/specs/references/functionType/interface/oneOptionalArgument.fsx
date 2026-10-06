module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member upper: (string option -> string) with get, set
    abstract member lower: (string option -> string) with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (upper: (string option -> string), lower: (string option -> string)) : MyObject = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
