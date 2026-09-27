module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member a: string with get, set
    abstract member b: float with get, set

type Same =
    Options

[<AllowNullLiteral>]
[<Interface>]
type Strings =
    abstract member a: string with get, set
    abstract member b: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
