module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Identifier =
    abstract member __escapedIdentifier: obj with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (__escapedIdentifier: unit) : Identifier = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
