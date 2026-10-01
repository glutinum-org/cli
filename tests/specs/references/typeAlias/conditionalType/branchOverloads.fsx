module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("unbox", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member unbox<'T> (arg: ElementHandle<'T>) : 'T = nativeOnly
    [<Import("unbox", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member unbox<'T> (arg: JSHandle<'T>) : 'T = nativeOnly
    [<Import("unbox", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member unbox<'Arg> (arg: 'Arg) : obj = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type ElementHandle<'T> =
    abstract member element: 'T with get, set

[<AllowNullLiteral>]
[<Interface>]
type JSHandle<'T> =
    abstract member value: 'T with get, set

[<AllowNullLiteral>]
[<Interface>]
type Unboxed<'Arg> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type NonNull<'T> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Page =
    abstract member evaluate<'R, 'T>: pageFunction: ('T -> 'R) * arg: ElementHandle<'T> -> JS.Promise<'R>
    abstract member evaluate<'R, 'T>: pageFunction: ('T -> 'R) * arg: JSHandle<'T> -> JS.Promise<'R>
    abstract member evaluate<'R, 'Arg>: pageFunction: (obj -> 'R) * arg: 'Arg -> JS.Promise<'R>
    abstract member unbox<'T>: arg: ElementHandle<'T> -> 'T
    abstract member unbox<'T>: arg: JSHandle<'T> -> 'T
    abstract member unbox<'Arg>: arg: 'Arg -> obj
    abstract member nonNull<'T>: value: 'T -> 'T

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
