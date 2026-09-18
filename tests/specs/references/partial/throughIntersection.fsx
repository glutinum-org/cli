module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Options<'T> =
    abstract member value: 'T with get, set
    abstract member label: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Base<'T> =
    abstract member opts: Base.opts<'T> with get, set
    abstract member rest: Base.rest<'T> with get, set

[<AllowNullLiteral>]
[<Interface>]
type Config =
    abstract member opts: Config.opts with get, set
    abstract member rest: Config.rest with get, set
    abstract member extra: float with get, set

module Base =

    [<AllowNullLiteral>]
    [<Interface>]
    type opts<'T> =
        abstract member value: 'T option with get, set
        abstract member label: string option with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type rest<'T> =
        abstract member value: 'T with get, set

module Config =

    [<AllowNullLiteral>]
    [<Interface>]
    type opts =
        abstract member value: string option with get, set
        abstract member label: string option with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type rest =
        abstract member value: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
