module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module NodeLike =

    module os =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<ImportAll("os")>]
            static member inline os
                with get () : os_.Exports =
                    nativeOnly

        module os_ =

            [<AbstractClass>]
            [<Erase>]
            type Exports =
                [<Emit("$0.hostname($1...)")>]
                abstract member hostname: unit -> string
                [<Emit("$0.platform($1...)")>]
                abstract member platform: unit -> string
                [<Emit("$0.EOL")>]
                abstract member EOL: string

    module path =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<ImportAll("path")>]
            static member inline path
                with get () : NodeLike.path.path_.path.PlatformPath =
                    nativeOnly

        module path_ =

            [<AbstractClass>]
            [<Erase>]
            type Exports =
                [<Emit("$0.path")>]
                abstract member path: NodeLike.path.path_.path.PlatformPath

            module path =

                [<AllowNullLiteral>]
                [<Interface>]
                type PlatformPath =
                    abstract member join: [<ParamArray>] paths: string [] -> string
                    abstract member sep: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
