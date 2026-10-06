module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Mixed<'T, 'U> =
    abstract member value: 'T with get, set
    abstract member other: 'U with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: 'T, other: 'U) : Mixed<'T, 'U> = nativeOnly

type Mixed<'T> =
    Mixed<'T, float>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
