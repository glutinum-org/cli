namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module UnresolvedImport =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("render", "unresolved-import")>]
        static member render (renderer: obj) : unit = nativeOnly
        [<Import("scale", "unresolved-import")>]
        static member scale (value: UnresolvedImport.scale.Scale) : unit = nativeOnly

    module scale =

        [<AllowNullLiteral>]
        [<Interface>]
        type Scale =
            abstract member factor: float with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (factor: float) : Scale = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
