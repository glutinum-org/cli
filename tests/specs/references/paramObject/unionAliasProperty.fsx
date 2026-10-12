module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("configure", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member configure () : unit = nativeOnly
    [<Import("configure", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member configure (value: Value.U2.Case1) : unit = nativeOnly
    [<Import("configure", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member configure (value: ResizeArray<string>) : unit = nativeOnly

type Value =
    U2<Value.U2.Case1, ResizeArray<string>>

[<AllowNullLiteral>]
[<Interface>]
type Inline =
    abstract member ``inline``: U2<Inline.``inline``.U2.Case1, ResizeArray<string>> option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``inline``: Inline.``inline``.U2.Case1) : Inline = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (``inline``: ResizeArray<string>) : Inline = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (?``inline``: U2<Inline.``inline``.U2.Case1, ResizeArray<string>>) : Inline = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Aliased =
    abstract member alias: Value option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (alias: Value.U2.Case1) : Aliased = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (alias: ResizeArray<string>) : Aliased = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (?alias: Value) : Aliased = nativeOnly

module Value =

    module U2 =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case1 =
            abstract member name: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (name: string) : Case1 = nativeOnly

module Inline =

    module ``inline`` =

        module U2 =

            [<AllowNullLiteral>]
            [<Interface>]
            type Case1 =
                abstract member name: string with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (name: string) : Case1 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
