module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member alias: string option with get, set
    abstract member describe: string option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?alias: string, ?describe: string) : Options = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Parser =
    abstract member option<'O>: key: string * options: 'O -> Parser

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
