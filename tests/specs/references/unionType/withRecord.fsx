module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type MyType =
    U2<string, MyType.U2.Case2>

module MyType =

    module U2 =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case2 =
            [<EmitIndexer>]
            abstract member Item: key: string -> float with get, set

(***)
#r "nuget: Fable.Core"
(***)
