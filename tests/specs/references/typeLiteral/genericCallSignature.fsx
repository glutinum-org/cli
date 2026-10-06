module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Static =
    abstract member memoize: Static.memoize with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (memoize: Static.memoize) : Static = nativeOnly

module Static =

    [<AllowNullLiteral>]
    [<Interface>]
    type memoize =
        [<Emit("$0($1...)")>]
        abstract member Invoke<'T>: func: 'T -> 'T
        abstract member Cache: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
