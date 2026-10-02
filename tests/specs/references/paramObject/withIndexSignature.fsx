module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("send", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member send (options: SendOptions) : unit = nativeOnly
    [<Import("get", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member get (options: RecordOptions) : unit = nativeOnly
    [<Import("fill", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member fill (bag: Bag) : unit = nativeOnly
    [<Import("measure", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``measure`` (sized: Sized<string>) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type SendOptions =
    [<EmitIndexer>]
    abstract member Item: key: string -> obj with get, set
    abstract member fields: string option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?fields: string) : SendOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type RecordOptions =
    inherit SendOptions
    abstract member expand: string option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?fields: string, ?expand: string) : RecordOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Bag =
    [<EmitIndexer>]
    abstract member Item: key: string -> obj with get, set

[<AllowNullLiteral>]
[<Interface>]
type Sized<'T> =
    abstract member length: float with get
    [<EmitIndexer>]
    abstract member Item: n: int -> 'T with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
