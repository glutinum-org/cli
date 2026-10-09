module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("ProgressEvent", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member ProgressEvent () : ProgressEvent = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Event =
    abstract member bubbles: bool with get
    [<ParamObject; Emit("$0")>]
    static member Create (bubbles: bool) : Event = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("ProgressEvent", "REPLACE_ME_WITH_MODULE_NAME")>]
type ProgressEvent =
    abstract member __proto__: ProgressEvent.__proto__ with get, set
    abstract member loaded: float with get

module ProgressEvent =

    [<AllowNullLiteral>]
    [<Interface>]
    type __proto__ =
        abstract member bubbles: bool with get
        abstract member __proto__: obj with get, set
        abstract member loaded: float with get
        [<ParamObject; Emit("$0")>]
        static member Create (bubbles: bool, __proto__: obj, loaded: float) : __proto__ = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
