module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Arguments<'T> =
    abstract member ``_``: ResizeArray<string> with get, set
    [<EmitIndexer>]
    abstract member Item: argName: string -> obj with get, set

[<AllowNullLiteral>]
[<Interface>]
type Parser<'T> =
    abstract member parseSync: unit -> Arguments<'T>
    abstract member parsePlain: unit -> Arguments<'T>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
