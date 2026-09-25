module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Locale =
    abstract member hello: config: Locale.hello.config -> string

module Locale =

    module hello =

        [<AllowNullLiteral>]
        [<Interface>]
        type config =
            abstract member verbose: Locale.hello.config.verbose with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (verbose: Locale.hello.config.verbose) : config = nativeOnly

        module config =

            [<AllowNullLiteral>]
            [<Interface>]
            type verbose =
                abstract member verbose: bool with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (verbose: bool) : verbose = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
