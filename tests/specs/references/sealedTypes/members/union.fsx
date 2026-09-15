module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Log4", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Log4<'R2> () : Log4<'R2> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Log4<'T, 'R1, 'R2> =
    abstract member info: data: U2<'T, float> -> U2<'R1, 'R2>

type Log4<'R2> =
    Log4<string, float, 'R2>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
