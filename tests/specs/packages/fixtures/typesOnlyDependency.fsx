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

    [<Global>]
    [<AllowNullLiteral>]
    type DispatcherOptions
        [<ParamObject; Emit("$0")>]
        (
            ?keepAlive: bool
        ) =

        member val keepAlive : bool option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
