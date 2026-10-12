module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("read", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member read<'T> (reader: Reader<'T>) : 'T = nativeOnly
    [<Import("Box", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Box<'T> () : Box<'T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Reader<'T> =
    abstract member tag: 'T with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (tag: 'T) : Reader<'T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("Box", "REPLACE_ME_WITH_MODULE_NAME")>]
type Box<'T> =
    inherit Reader<'T>
    abstract member reader: Reader<'T> with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
