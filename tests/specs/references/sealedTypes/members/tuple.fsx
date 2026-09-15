module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Log", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Log<'R2> () : Log<'R2> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Log<'T, 'R1, 'R2> =
    abstract member info: data: 'T * float -> U2<'R1, 'R2>

type Log<'R2> =
    Log<string, float, 'R2>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
