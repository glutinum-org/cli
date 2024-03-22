module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type Locale =
    abstract member hello: config: Locale.Locale.hello.config -> string

module Locale =

    module Locale =

        module hello =

            [<Global>]
            [<AllowNullLiteral>]
            type config
                [<ParamObject; Emit("$0")>]
                (
                    verbose: Locale.Locale.hello.config.verbose
                ) =

                member val verbose : Locale.Locale.hello.config.verbose = nativeOnly with get, set

            module config =

                [<Global>]
                [<AllowNullLiteral>]
                type verbose
                    [<ParamObject; Emit("$0")>]
                    (
                        verbose: bool
                    ) =

                    member val verbose : bool = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
(***)
