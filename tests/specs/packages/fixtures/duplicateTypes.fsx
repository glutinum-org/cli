namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module DuplicateTypes =

    [<AllowNullLiteral>]
    [<Interface>]
    type Item =
        abstract member brand: DuplicateTypes.other.Brand with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (brand: DuplicateTypes.other.Brand) : Item = nativeOnly

    type Color =
        string

    module other =

        [<AllowNullLiteral>]
        [<Interface>]
        type Brand =
            abstract member name: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (name: string) : Brand = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type Color =
            abstract member r: float with get, set
            abstract member g: float with get, set
            abstract member b: float with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (r: float, g: float, b: float) : Color = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
