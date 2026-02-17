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
type Log<'R2> =
    abstract member info: data: string * float -> U2<float, 'R2>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
