module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline a
        with get () : a_.Exports =
            nativeOnly

module a_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.b")>]
        abstract member b: b.Exports with get

    module b =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Emit("$0.c")>]
            abstract member c: c.Exports with get

        module c =

            [<AbstractClass>]
            [<Erase>]
            type Exports =
                [<Emit("$0.run($1...)")>]
                abstract member run: value: string -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
