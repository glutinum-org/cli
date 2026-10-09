module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Dayjs", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Dayjs () : Dayjs = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("Dayjs", "REPLACE_ME_WITH_MODULE_NAME")>]
type Dayjs =
    abstract member locale: preset: string -> Dayjs
    abstract member locale: preset: obj -> Dayjs
    abstract member locale: preset: U2<string, obj> -> Dayjs

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
