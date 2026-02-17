module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Log2", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Log2<'R2> () : Log2<'R2> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Log2<'R2> =
    abstract member info: data: U2<string, float> -> U2<float, 'R2>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
