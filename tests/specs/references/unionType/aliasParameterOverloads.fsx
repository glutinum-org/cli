module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("setView", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member setView (center: LatLng, ?zoom: float) : unit = nativeOnly
    [<Import("setView", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member setView (center: LatLngTuple, ?zoom: float) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type LatLng =
    abstract member lat: float with get, set
    abstract member lng: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (lat: float, lng: float) : LatLng = nativeOnly

type LatLngTuple =
    float * float

type LatLngExpression =
    U2<LatLng, LatLngTuple>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
