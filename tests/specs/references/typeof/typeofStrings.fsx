module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System


[<AllowNullLiteral>]
[<Interface>]
type Primitives =
    abstract member a: bool with get, set
    abstract member button: bool with get, set

(***)
#r "nuget: Fable.Core"
(***)
