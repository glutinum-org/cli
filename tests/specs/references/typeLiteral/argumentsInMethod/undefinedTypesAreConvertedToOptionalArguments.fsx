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
                    verbose: bool,
                    ?prefix: string,
                    ?suffix: string
                ) =

                member val verbose : bool = nativeOnly with get, set
                member val prefix : string option = nativeOnly with get, set
                member val suffix : string option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
(***)
