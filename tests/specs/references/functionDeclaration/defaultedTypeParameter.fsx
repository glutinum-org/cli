module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("layerGroup", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member layerGroup<'P> (?layers: ResizeArray<string>) : LayerGroup<'P> = nativeOnly
    [<Import("layerGroup", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member layerGroup (?layers: ResizeArray<string>) : LayerGroup<obj> = nativeOnly
    [<Import("layerGroup", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member layerGroup () : LayerGroup<obj> = nativeOnly
    [<Import("empty", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member empty<'P> () : LayerGroup<'P> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type LayerGroup<'P> =
    abstract member count: float with get, set

type LayerGroup =
    LayerGroup<obj>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
