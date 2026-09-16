module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("show", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member show (?options: Exports.show.options) : unit = nativeOnly

module Exports =

    module show =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            private () =

            [<ParamObject; Emit("$0")>]
            new (value: bool, ?delay: float) =
                options()

            [<ParamObject; Emit("$0")>]
            new (value: string, ?delay: float) =
                options()

            member val value : U2<bool, string> = nativeOnly with get, set
            member val delay : float option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
