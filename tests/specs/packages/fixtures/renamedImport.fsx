namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module RenamedImport =

    [<AllowNullLiteral>]
    [<Interface>]
    type Provider =
        abstract member provide: position: DepLib.Position -> DepLib.Range option
        abstract member mark: marker: RenamedImport.marker.Marker -> unit

    module marker =

        [<AllowNullLiteral>]
        [<Interface>]
        type Marker =
            abstract member position: DepLib.Position with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (position: DepLib.Position) : Marker = nativeOnly

module DepLib =

    [<AllowNullLiteral>]
    [<Interface>]
    type Position =
        abstract member line: float with get, set
        abstract member character: float with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (line: float, character: float) : Position = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Range =
        abstract member start: DepLib.Position with get, set
        abstract member ``end``: DepLib.Position with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
