module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("useInView", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member useInView (options: Options) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member required: U2<float, string> with get, set
    abstract member trackVisibility: U2<bool, string> option with get, set
    abstract member delay: float option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (required: float, ?delay: float) : Options = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (required: float, trackVisibility: bool, ?delay: float) : Options = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (required: float, trackVisibility: string, ?delay: float) : Options = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (required: string, ?delay: float) : Options = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (required: string, trackVisibility: bool, ?delay: float) : Options = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (required: string, trackVisibility: string, ?delay: float) : Options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
