module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type TelemetryLogger =
    abstract member logError: eventName: string * ?data: TelemetryLogger.logError.data -> unit
    abstract member logError: error: Exception * ?data: TelemetryLogger.logError.data_1 -> unit

module TelemetryLogger =

    module logError =

        [<AllowNullLiteral>]
        [<Interface>]
        type data =
            [<EmitIndexer>]
            abstract member Item: key: string -> obj with get, set

        [<AllowNullLiteral>]
        [<Interface>]
        type data_1 =
            [<EmitIndexer>]
            abstract member Item: key: string -> obj with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
