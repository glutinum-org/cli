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

    [<AllowNullLiteral>]
    [<Interface>]
    type ResizeArray =
        abstract member kind: string with get
        [<ParamObject; Emit("$0")>]
        static member Create (kind: string) : ResizeArray = nativeOnly

module Docs =

    [<AllowNullLiteral>]
    [<Interface>]
    type ReadonlyArray =
        abstract member kind: string with get
        [<ParamObject; Emit("$0")>]
        static member Create (kind: string) : ReadonlyArray = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
