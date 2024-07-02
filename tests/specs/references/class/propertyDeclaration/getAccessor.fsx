module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Foo", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Foo () : Foo = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Foo =
    abstract member id: string with get

(***)
#r "nuget: Fable.Core"
(***)
