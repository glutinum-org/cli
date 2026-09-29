module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

type Documentation =
    ResizeArray<Documentation.Item>

type Docs =
    ReadonlyArray<Docs.Item>

module Documentation =

    [<AllowNullLiteral>]
    [<Interface>]
    type Item =
        abstract member kind: string with get
        [<ParamObject; Emit("$0")>]
        static member Create (kind: string) : Item = nativeOnly

module Docs =

    [<AllowNullLiteral>]
    [<Interface>]
    type Item =
        abstract member kind: string with get
        [<ParamObject; Emit("$0")>]
        static member Create (kind: string) : Item = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
