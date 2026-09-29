namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module Factory =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<ImportDefault("factory")>]
        static member factory () : Factory.Instance = nativeOnly
        [<ImportDefault("factory")>]
        static member factory (args: ResizeArray<string>) : Factory.Instance = nativeOnly
        [<ImportDefault("factory")>]
        static member factory (args: string) : Factory.Instance = nativeOnly
        [<Import("animator", "factory")>]
        static member inline animator: Factory.Animator = nativeOnly
        [<Import("Colors", "factory")>]
        static member inline Colors: Exports.Colors__.Type = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Instance =
        abstract member run: ?hidden: Factory.Hidden -> unit

    [<AllowNullLiteral>]
    [<Interface>]
    type Animator =
        abstract member start: unit -> unit

    [<AllowNullLiteral>]
    [<Interface>]
    type Hidden =
        abstract member enabled: bool with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (enabled: bool) : Hidden = nativeOnly

    module Exports =

        module Colors__ =

            [<AllowNullLiteral>]
            [<Interface>]
            type Type =
                abstract member id: string with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (id: string) : Type = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
