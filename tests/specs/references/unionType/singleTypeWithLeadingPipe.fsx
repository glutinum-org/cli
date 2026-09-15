module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type TypeLiteral =
    abstract member kind: string with get, set
    abstract member x: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type FunctionType =
    [<Emit("$0($1...)")>]
    abstract member Invoke: a: string * b: float -> unit

[<AllowNullLiteral>]
[<Interface>]
type Props =
    abstract member shape: Props.shape with get, set

module Props =

    [<Global>]
    [<AllowNullLiteral>]
    type shape
        [<ParamObject; Emit("$0")>]
        (
            kind: string,
            x: float
        ) =

        member val kind : string = nativeOnly with get, set
        member val x : float = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
