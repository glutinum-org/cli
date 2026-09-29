namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module NodeLike =

    type _Helper =
        string

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
            [<ImportDefault("node:path")>]
            static member inline path: NodeLike.path.path_.PlatformPath = nativeOnly
            [<ImportDefault("node:path"); Emit("$0.join($1...)")>]
            static member join ([<ParamArray>] paths: string []) : string = nativeOnly
            [<ImportDefault("node:path")>]
            [<Emit("$0.sep")>]
            static member inline sep: string = nativeOnly

        module path_ =

            [<AllowNullLiteral>]
            [<Interface>]
            type PlatformPath =
                abstract member join: [<ParamArray>] paths: string [] -> string
                abstract member sep: string with get, set

        type PlatformPath =
            path_.PlatformPath

    module stream =

        module web =

            [<AllowNullLiteral>]
            [<Interface>]
            type ReadableStream =
                abstract member locked: bool with get, set

    module util =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("format", "node:util")>]
            static member format (format: string) : string = nativeOnly

        module types =

            [<AbstractClass>]
            [<Erase>]
            type Exports =
                [<Import("isDate", "node:util/types")>]
                static member isDate (value: obj) : bool = nativeOnly

    module Exports =

        type os =
            os.Exports

        type path =
            path.Exports

        type util =
            util.Exports

        module util =

            type types =
                util.types.Exports

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
