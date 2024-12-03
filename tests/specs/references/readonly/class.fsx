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
    abstract member bar: string with get, set
    abstract member foo: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type ReadonlyFoo =
    abstract member bar: string with get
    abstract member foo: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
