module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("create", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member create (options: Options) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member attribution: U2<string, ResizeArray<string>> option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create () : Options = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (attribution: string) : Options = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (attribution: ResizeArray<string>) : Options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
