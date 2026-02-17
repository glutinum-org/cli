module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Log3", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Log3<'R2> () : Log3<'R2> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Thenable<'T> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Log3<'R2> =
    abstract member info: data: U2<Thenable<string>, float> -> U2<float, 'R2>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
