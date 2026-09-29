namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module ExportsMapEntry =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("devices", "exports-map-entry")>]
        static member inline devices: ResizeArray<ExportsMapEntry.Device> = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Device =
        abstract member name: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
