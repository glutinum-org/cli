module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline createInstance: Exports.createInstance__.Type = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Instance =
    abstract member parse: unit -> unit

module Exports =

    module createInstance__ =

        type Type =
            delegate of ?args: U2<ResizeArray<string>, string> * ?cwd: string -> Instance

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
