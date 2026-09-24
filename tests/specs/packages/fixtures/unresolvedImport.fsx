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

        [<Global>]
        [<AllowNullLiteral>]
        type Scale
            [<ParamObject; Emit("$0")>]
            (
                factor: float
            ) =

            member val factor : float = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
