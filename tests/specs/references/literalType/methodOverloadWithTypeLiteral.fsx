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

        [<AllowNullLiteral>]
        [<Interface>]
        type data =
            abstract member encoding: string option with get
            [<ParamObject; Emit("$0")>]
            static member Create (?encoding: string) : data = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type data_1 =
            abstract member permissions: ResizeArray<string> option with get
            [<ParamObject; Emit("$0")>]
            static member Create (?permissions: ResizeArray<string>) : data_1 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
