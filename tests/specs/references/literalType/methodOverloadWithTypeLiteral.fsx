module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type TelemetryLogger =
    abstract member logError: ?data: TelemetryLogger.logError.data -> unit
    abstract member logError: error: Exception * ?data: TelemetryLogger.logError.data_1 -> unit

module TelemetryLogger =

    module logError =

        [<Global>]
        [<AllowNullLiteral>]
        type data
            [<ParamObject; Emit("$0")>]
            (
                ?encoding: string
            ) =

            member val encoding : string option = nativeOnly with get

        [<Global>]
        [<AllowNullLiteral>]
        type data_1
            [<ParamObject; Emit("$0")>]
            (
                ?permissions: ResizeArray<string>
            ) =

            member val permissions : ResizeArray<string> option = nativeOnly with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
