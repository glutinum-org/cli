module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module NodeLike =

    module os =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("hostname", "os")>]
            static member hostname () : string = nativeOnly
            [<Import("platform", "os")>]
            static member platform () : string = nativeOnly
            [<Import("EOL", "os")>]
            static member inline EOL: string = nativeOnly

    module path =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<ImportAll("path")>]
            static member inline path: NodeLike.path.path_.PlatformPath = nativeOnly

        module path_ =

            [<AllowNullLiteral>]
            [<Interface>]
            type PlatformPath =
                abstract member join: [<ParamArray>] paths: string [] -> string
                abstract member sep: string with get, set

    module Exports =

        type os =
            os.Exports

        type path =
            path.Exports

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
