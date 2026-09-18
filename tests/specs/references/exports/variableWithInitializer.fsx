module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline ts
        with get () : ts_.Exports =
            nativeOnly

module ts_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.versionMajorMinor")>]
        abstract member versionMajorMinor: string
        [<Emit("$0.retries")>]
        abstract member retries: float
        [<Emit("$0.enabled")>]
        abstract member enabled: bool
        [<Emit("$0.version")>]
        abstract member version: string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
