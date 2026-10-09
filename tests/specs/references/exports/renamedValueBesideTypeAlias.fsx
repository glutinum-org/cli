module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("RangeSet", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline RangeSet: RangeSetConstructor = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type IntervalTree =
    abstract member from: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (from: float) : IntervalTree = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type RangeSetPrototype =
    abstract member addKey: key: string -> RangeSet

[<AllowNullLiteral>]
[<Interface>]
type RangeSet =
    abstract member addKey: key: string -> RangeSet
    abstract member from: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type RangeSetConstructor =
    [<EmitConstructor>]
    abstract member Create: unit -> RangeSet

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
