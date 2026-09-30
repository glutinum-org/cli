module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

type Nested =
    ReadonlyArray<U2<obj, string>>

type Tree =
    ResizeArray<U2<obj, float>>

type Json =
    U5<string, float, bool, ResizeArray<obj>, Json.U5.Case5> option

module Json =

    module U5 =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case5 =
            [<EmitIndexer>]
            abstract member Item: key: string -> Json with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
