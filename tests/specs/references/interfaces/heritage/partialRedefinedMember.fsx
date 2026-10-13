module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("get", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member get () : Redefines = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Base =
    abstract member value: U2<string, float> with get, set
    abstract member other: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: string, other: float) : Base = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (value: float, other: float) : Base = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Redefines =
    abstract member value: string with get, set
    abstract member other: float option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: string, ?other: float) : Redefines = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
