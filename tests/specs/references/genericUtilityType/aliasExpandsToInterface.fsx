module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type User =
    abstract member id: string with get, set
    abstract member name: string with get, set
    abstract member password: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (id: string, name: string, password: string) : User = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Picked<'T, 'K when 'K :> obj> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type IdName =
    abstract member id: string with get, set
    abstract member name: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (id: string, name: string) : IdName = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
