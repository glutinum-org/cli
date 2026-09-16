module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Names =
    abstract member ``type``: Names.``type`` with get, set
    abstract member ``1st``: Names.``1st`` with get, set
    abstract member ``x-options``: Names.``x-options`` with get, set

module Names =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type ``type`` =
        | a
        | b

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type ``1st`` =
        | c
        | d

    [<Global>]
    [<AllowNullLiteral>]
    type ``x-options``
        [<ParamObject; Emit("$0")>]
        (
            debug: bool
        ) =

        member val debug : bool = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
