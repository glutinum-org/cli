module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("MyClass", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member MyClass () : MyClass = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type MyClass =
    abstract member contains: otherBoundsOrLatLng: U2<MyUnion, string> -> bool

type MyUnion =
    U2<MyClass, string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
