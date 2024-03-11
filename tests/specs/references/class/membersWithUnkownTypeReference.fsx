module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("Dayjs", "module"); EmitConstructor>]
    static member Dayjs () : Dayjs = nativeOnly

[<AllowNullLiteral>]
type Dayjs =
    abstract member locale: preset: U2<string, obj> -> Dayjs

(***)
#r "nuget: Fable.Core"
(***)
