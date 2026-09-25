module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline google
        with get () : google_.Exports =
            nativeOnly

module google_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.maps")>]
        abstract member maps: maps.Exports with get

    module maps =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Emit("new $0.Map($1...)")>]
            abstract member Map: element: string * ?options: MapOptions -> Map

        [<AllowNullLiteral>]
        [<Interface>]
        type MapOptions =
            abstract member zoom: float option with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (?zoom: float) : MapOptions = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type Map =
            interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
