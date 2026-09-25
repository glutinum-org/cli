module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ContextVariableMap =
    abstract member metric: string with get, set

module ContextVariableMap =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"metric\"")>]
        static member inline metric: Key<string> = nativeOnly

type Get<'Key when 'Key :> obj> =
    delegate of key: 'Key -> obj

[<AllowNullLiteral>]
[<Interface>]
type Ctx =
    abstract member get<'Key>: key: ContextVariableMap.Key<'Key> -> 'Key

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
