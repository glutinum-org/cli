module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("log", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member log (data: Thenable<string>) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Thenable<'T> =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
