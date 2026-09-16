module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

type Documentation =
    ResizeArray<Documentation.ResizeArray>

type Docs =
    ReadonlyArray<Docs.ReadonlyArray>

module Documentation =

    [<Global>]
    [<AllowNullLiteral>]
    type ResizeArray
        [<ParamObject; Emit("$0")>]
        (
            kind: string
        ) =

        member val kind : string = nativeOnly with get

module Docs =

    [<Global>]
    [<AllowNullLiteral>]
    type ReadonlyArray
        [<ParamObject; Emit("$0")>]
        (
            kind: string
        ) =

        member val kind : string = nativeOnly with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
