module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Test =
    [<EmitConstructor>]
    abstract member Create: [<ParamArray>] args: ResizeArray<obj> [] -> obj

(***)
#r "nuget: Fable.Core"
(***)
