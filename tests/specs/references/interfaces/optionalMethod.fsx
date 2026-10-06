module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Compare =
    abstract member asMethod: (string -> unit) option with get, set
    abstract member asProperty: (string -> unit) option with get, set

[<AllowNullLiteral>]
[<Interface>]
type InParamObject =
    abstract member options: InParamObject.options with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (options: InParamObject.options) : InParamObject = nativeOnly

module InParamObject =

    [<AllowNullLiteral>]
    [<Interface>]
    type options =
        abstract member a: string with get, set
        abstract member maybe: (string -> unit) option with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (a: string, ?maybe: (string -> unit)) : options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
