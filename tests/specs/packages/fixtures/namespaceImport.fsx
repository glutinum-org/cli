namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module NamespaceImport =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("useColors", "namespace-import")>]
        static member useColors (value: NamespaceImport.colors.Exports) : unit = nativeOnly
        [<Import("useShapes", "namespace-import")>]
        static member useShapes (value: obj) : unit = nativeOnly
        [<Import("useBarrel", "namespace-import")>]
        static member useBarrel (value: obj) : unit = nativeOnly

    module barrel =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("red", "namespace-import/barrel.js")>]
            static member red () : string = nativeOnly

    module colors =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("red", "namespace-import/colors.js")>]
            static member red () : string = nativeOnly

    module shapes =

        [<AllowNullLiteral>]
        [<Interface>]
        type Circle =
            abstract member radius: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
