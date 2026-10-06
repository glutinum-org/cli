module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("run", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member run (options: Exports.run__.options) : unit = nativeOnly
    [<Import("runGeneric", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member runGeneric<'T> (options: Exports.runGeneric__.options) : unit = nativeOnly
    [<Import("runAlias", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member runAlias<'T> (options: Picked<'T>) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member a: string with get, set
    abstract member b: float option with get, set
    abstract member c: bool with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (a: string, c: bool, ?b: float) : Options = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Labeled<'T> =
    abstract member value: 'T with get, set
    abstract member label: string with get, set
    abstract member extra: bool with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: 'T, label: string, extra: bool) : Labeled<'T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Picked<'T> =
    abstract member value: 'T with get, set
    abstract member label: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: 'T, label: string) : Picked<'T> = nativeOnly

module Exports =

    module run__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member a: string with get, set
            abstract member b: float option with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (a: string, ?b: float) : options = nativeOnly

    module runGeneric__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member a: string with get, set
            abstract member b: float option with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (a: string, ?b: float) : options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
