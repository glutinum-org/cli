module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("Dayjs", "module"); EmitConstructor>]
    static member Dayjs () : Dayjs = nativeOnly

[<AllowNullLiteral>]
type Dayjs =
    abstract member clone: unit -> Dayjs
    abstract member isValid: unit -> bool
    abstract member locale: unit -> string

(***)
#r "nuget: Fable.Core"
(***)
