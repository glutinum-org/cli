module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("hello", "module")>]
    static member hello () : JS.Promise<JS.Uint8Array> = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
