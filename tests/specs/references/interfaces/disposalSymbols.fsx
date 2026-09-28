module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Iterable<'T> = Collections.Generic.IEnumerable<'T>

[<AllowNullLiteral>]
[<Interface>]
type FileHandle =
    abstract member read: unit -> JS.Promise<string>
    [<Emit("$0[Symbol.dispose]($1...)")>]
    abstract member dispose: unit -> unit
    [<Emit("$0[Symbol.asyncDispose]($1...)")>]
    abstract member asyncDispose: unit -> JS.Promise<unit>

[<AllowNullLiteral>]
[<Interface>]
type Bag =
    inherit Iterable<string>
    abstract member size: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
