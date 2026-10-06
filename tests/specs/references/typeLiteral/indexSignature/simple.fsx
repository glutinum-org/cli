module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type OptionsInline =
    abstract member users: OptionsInline.users with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (users: OptionsInline.users) : OptionsInline = nativeOnly

module OptionsInline =

    [<AllowNullLiteral>]
    [<Interface>]
    type users =
        [<EmitIndexer>]
        abstract member Item: key: string -> string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
