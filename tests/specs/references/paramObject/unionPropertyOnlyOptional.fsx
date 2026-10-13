module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (options: AllOptional) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type AllOptional =
    abstract member value: U2<bool, string> option with get, set
    abstract member delay: float option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?delay: float) : AllOptional = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (value: bool, ?delay: float) : AllOptional = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (value: string, ?delay: float) : AllOptional = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
