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
            abstract member prefix: string option with get, set
            abstract member suffix: string option with get, set
            abstract member verbose: bool with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (verbose: bool, ?prefix: string, ?suffix: string) : config = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
