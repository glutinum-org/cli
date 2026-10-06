module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Animal =
    abstract member name: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (name: string) : Animal = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
