module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<ImportAll("module")>]
    static member inline chalk: ChalkInstance = nativeOnly


[<AllowNullLiteral>]
[<Interface>]
type ChalkInstance =
    interface end

(***)
#r "nuget: Fable.Core"
(***)
