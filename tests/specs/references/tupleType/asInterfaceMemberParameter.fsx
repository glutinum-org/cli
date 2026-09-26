module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Picker =
    abstract member pick: path: (string * float) -> unit
    abstract member pickMany: paths: ResizeArray<string * float> * flag: bool -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
