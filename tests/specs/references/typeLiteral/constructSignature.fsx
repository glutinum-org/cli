module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type Test =
    [<EmitConstructor>]
    abstract member Create: [<ParamArray>] args: ResizeArray<obj> [] -> obj

(***)
#r "nuget: Fable.Core"
(***)
