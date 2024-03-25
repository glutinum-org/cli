module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Log =
    [<Emit("$0($1...)")>]
    abstract member Invoke: prefix: string * [<ParamArray>] args: ResizeArray<obj> [] -> obj

(***)
#r "nuget: Fable.Core"
(***)
