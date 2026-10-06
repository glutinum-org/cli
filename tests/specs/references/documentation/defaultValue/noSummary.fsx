module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Logger =
    /// <summary>
    /// <c>[timestamp]</c>
    /// </summary>
    abstract member prefix: string option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?prefix: string) : Logger = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
