module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (x: Exports.f.x) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type User =
    abstract member id: string with get, set
    abstract member name: string with get, set

type Picked<'T, 'K when 'K :> obj> =
    Pick<'T, 'K>

module Exports =

    module f =

        [<Global>]
        [<AllowNullLiteral>]
        type x
            [<ParamObject; Emit("$0")>]
            (
                id: string
            ) =

            member val id : string = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
