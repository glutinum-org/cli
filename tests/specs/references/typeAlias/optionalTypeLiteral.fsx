module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

type Documentation =
    Documentation.Value option

type Docs =
    ReadonlyArray<Docs.Value> option

[<AllowNullLiteral>]
[<Interface>]
type Named =
    abstract member name: string with get, set

type MaybeNamed =
    Named option

module Documentation =

    [<Global>]
    [<AllowNullLiteral>]
    type Value
        [<ParamObject; Emit("$0")>]
        (
            kind: string
        ) =

        member val kind : string = nativeOnly with get

module Docs =

    [<Global>]
    [<AllowNullLiteral>]
    type Value
        [<ParamObject; Emit("$0")>]
        (
            kind: string
        ) =

        member val kind : string = nativeOnly with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
