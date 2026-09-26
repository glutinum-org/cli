module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("minimatch", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline minimatch: Exports.minimatch__.Type = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member debug: bool option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?debug: bool) : Options = nativeOnly

module Exports =

    module minimatch__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            [<Emit("$0($1...)")>]
            abstract member Invoke: p: string * pattern: string * ?options: Options -> bool
            abstract member defaults: (Options -> Exports.minimatch__.Type.defaults) with get, set

        module Type =

            [<AllowNullLiteral>]
            [<Interface>]
            type defaults =
                [<Emit("$0($1...)")>]
                abstract member Invoke: p: string * pattern: string * ?options: Options -> bool
                abstract member defaults: (Options -> unit) with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
