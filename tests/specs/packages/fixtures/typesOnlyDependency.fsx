namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module TypesOnlyUser =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("send", "types-only-user")>]
        static member send (dispatcher: TypesOnlyDep.Dispatcher) : unit = nativeOnly

module TypesOnlyDep =

    [<AllowNullLiteral>]
    [<Interface>]
    type Dispatcher =
        abstract member close: unit -> unit

    [<AllowNullLiteral>]
    [<Interface>]
    type DispatcherOptions =
        abstract member keepAlive: bool option with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (?keepAlive: bool) : DispatcherOptions = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
