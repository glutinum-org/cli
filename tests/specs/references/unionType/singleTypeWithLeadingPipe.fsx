module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type TypeLiteral =
    abstract member kind: string with get, set
    abstract member x: float with get, set

type FunctionType =
    delegate of a: string * b: float -> unit

[<AllowNullLiteral>]
[<Interface>]
type Props =
    abstract member shape: Props.shape with get, set

module Props =

    [<AllowNullLiteral>]
    [<Interface>]
    type shape =
        abstract member kind: string with get, set
        abstract member x: float with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (kind: string, x: float) : shape = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
