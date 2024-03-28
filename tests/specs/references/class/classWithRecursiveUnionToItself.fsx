module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type MyClass =
    abstract member contains: otherBoundsOrLatLng: U2<MyUnion, string> -> bool

type MyUnion =
    U2<MyClass, string>

(***)
#r "nuget: Fable.Core"
(***)
