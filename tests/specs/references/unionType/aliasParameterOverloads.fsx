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

[<RequireQualifiedAccess>]
[<Erase>]
type LatLngExpression =
    | LatLng of LatLng
    | LatLngTuple of LatLngTuple

    [<Emit("$0")>]
    static member op_Implicit(value: LatLng) : LatLngExpression = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: LatLng) : LatLngExpression = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: LatLngTuple) : LatLngExpression = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: LatLngTuple) : LatLngExpression = nativeOnly

    /// Unchecked, the value as <c>LatLng</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsLatLng: LatLng = nativeOnly

    /// Unchecked, the value as <c>LatLngTuple</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsLatLngTuple: LatLngTuple = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
