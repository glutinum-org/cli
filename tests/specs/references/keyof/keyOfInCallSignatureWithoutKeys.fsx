module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Empty =
    interface end

module Empty =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

type Get<'Key when 'Key :> obj> =
    delegate of key: 'Key -> obj

[<AllowNullLiteral>]
[<Interface>]
type Ctx =
    abstract member get<'Key>: key: Empty.Key<'Key> -> 'Key
    [<ParamObject; Emit("$0")>]
    static member Create (get: Get) : Ctx = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
