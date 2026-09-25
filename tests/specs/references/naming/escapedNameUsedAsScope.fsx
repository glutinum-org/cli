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

    [<AllowNullLiteral>]
    [<Interface>]
    type ``x-options`` =
        abstract member debug: bool with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (debug: bool) : ``x-options`` = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
