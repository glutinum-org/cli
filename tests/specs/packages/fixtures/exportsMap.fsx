namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module ExportsMap =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("start", "exports-map")>]
        static member start (options: ExportsMap.``internal``.hidden.Options) : ExportsMap.``internal``.hidden.Session = nativeOnly
        [<Import("running", "exports-map")>]
        static member inline running: bool = nativeOnly
        [<Import("alpha", "exports-map")>]
        static member alpha () : unit = nativeOnly
        [<Import("beta", "exports-map/utils")>]
        static member beta () : unit = nativeOnly

    module ``internal`` =

        module hidden =

            [<AllowNullLiteral>]
            [<Interface>]
            type Options =
                abstract member verbose: bool with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (verbose: bool) : Options = nativeOnly

            [<AllowNullLiteral>]
            [<Interface>]
            type Session =
                abstract member id: string with get
                abstract member close: unit -> unit

            type mode =
                float

    module Utils =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("pad", "exports-map/utils")>]
            static member pad (value: string) : string = nativeOnly
            [<Import("beta", "exports-map/utils")>]
            static member beta () : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
