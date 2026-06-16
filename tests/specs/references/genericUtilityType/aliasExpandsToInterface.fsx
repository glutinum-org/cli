module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type User =
    abstract member id: string with get, set
    abstract member name: string with get, set
    abstract member password: string with get, set

type Picked<'T, 'K when 'K :> obj> =
    Pick<'T, 'K>

[<AllowNullLiteral>]
[<Interface>]
type IdName =
    abstract member id: string with get, set
    abstract member name: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
