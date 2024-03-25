module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("Dayjs", "module"); EmitConstructor>]
    static member Dayjs () : Dayjs = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Dayjs =
    abstract member clone: unit -> Dayjs
    abstract member isValid: unit -> bool
    abstract member locale: unit -> string

(***)
#r "nuget: Fable.Core"
(***)
