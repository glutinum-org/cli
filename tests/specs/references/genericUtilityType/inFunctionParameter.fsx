module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (x: Exports.f__.x) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type User =
    abstract member id: string with get, set
    abstract member name: string with get, set

type Picked<'T, 'K when 'K :> obj> =
    Pick<'T, 'K>

module Exports =

    module f__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type x =
            abstract member id: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (id: string) : x = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
