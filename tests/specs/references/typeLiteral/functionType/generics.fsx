module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Callback<'Param, 'AtomType> =
    [<Emit("$0($1...)")>]
    abstract member Invoke: event: Callback.event<'Param, 'AtomType> -> unit

module Callback =

    [<Global>]
    [<AllowNullLiteral>]
    type event<'Param, 'AtomType>
        [<ParamObject; Emit("$0")>]
        (
            ``type``: Callback.event.``type``,
            param: 'Param,
            atom: 'AtomType
        ) =

        member val ``type`` : Callback.event.``type`` = nativeOnly with get, set
        member val param : 'Param = nativeOnly with get, set
        member val atom : 'AtomType = nativeOnly with get, set

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
