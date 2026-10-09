module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Internals<'T> =
    abstract member input: 'T with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (input: 'T) : Internals<'T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Schema<'Input> =
    abstract member parse: input: 'Input -> 'Input

[<AllowNullLiteral>]
[<Interface>]
type BaseString<'T> =
    inherit Schema<obj>
    abstract member _zod: 'T with get, set

[<AllowNullLiteral>]
[<Interface>]
type MiniString<'Input> =
    inherit BaseString<Internals<'Input>>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
