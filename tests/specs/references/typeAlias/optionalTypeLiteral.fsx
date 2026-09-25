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

    [<AllowNullLiteral>]
    [<Interface>]
    type Value =
        abstract member kind: string with get
        [<ParamObject; Emit("$0")>]
        static member Create (kind: string) : Value = nativeOnly

module Docs =

    [<AllowNullLiteral>]
    [<Interface>]
    type Value =
        abstract member kind: string with get
        [<ParamObject; Emit("$0")>]
        static member Create (kind: string) : Value = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
