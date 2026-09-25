module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Callback<'Param, 'AtomType> =
    delegate of event: Callback.event<'Param, 'AtomType> -> unit

module Callback =

    [<AllowNullLiteral>]
    [<Interface>]
    type event<'Param, 'AtomType> =
        abstract member ``type``: Callback.event.``type`` with get, set
        abstract member param: 'Param with get, set
        abstract member atom: 'AtomType with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (``type``: Callback.event.``type``, param: 'Param, atom: 'AtomType) : event<'Param, 'AtomType> = nativeOnly

    module event =

        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.None)>]
        type ``type`` =
            | CREATE
            | REMOVE

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
