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
    static member configure (value: Value.Cases.Case1) : unit = nativeOnly
    [<Import("configure", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member configure (value: ResizeArray<string>) : unit = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type Value =
    | Case1 of Value.Cases.Case1
    | StringArray of ResizeArray<string>

    [<Emit("$0")>]
    static member op_Implicit(value: Value.Cases.Case1) : Value = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Value.Cases.Case1) : Value = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<string>) : Value = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<string>) : Value = nativeOnly

    /// Unchecked, the value as <c>Case1</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase1: Value.Cases.Case1 = nativeOnly

    /// Unchecked, the value as <c>StringArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsStringArray: ResizeArray<string> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Inline =
    abstract member ``inline``: U2<Inline.``inline``.U2.Case1, ResizeArray<string>> option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create () : Inline = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (``inline``: Inline.``inline``.U2.Case1) : Inline = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (``inline``: ResizeArray<string>) : Inline = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Aliased =
    abstract member alias: Value option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create () : Aliased = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (alias: Value.Cases.Case1) : Aliased = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (alias: ResizeArray<string>) : Aliased = nativeOnly

module Value =

    module Cases =

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
