module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<ImportAll("module")>]
    static member inline supportsColor: Exports.supportsColor = nativeOnly


type ColorInfo =
    string

module Exports =

    [<Global>]
    [<AllowNullLiteral>]
    type supportsColor
        [<ParamObject; Emit("$0")>]
        (
            stdout: ColorInfo,
            stderr: ColorInfo
        ) =

        member val stdout : ColorInfo = nativeOnly with get, set
        member val stderr : ColorInfo = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
(***)
