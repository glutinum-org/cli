module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("pipeline", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member pipeline<'B> (destination: 'B) : JS.Promise<obj> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Destination<'T> =
    abstract member run: unit -> JS.Promise<'T>

[<AllowNullLiteral>]
[<Interface>]
type Result<'S> =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
