module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("create", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member create (?options: Options) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member animation: float option with get, set
    abstract member group: string option with get, set
    abstract member setData: Options.setData option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?animation: float, ?group: string, ?setData: Options.setData) : Options = nativeOnly

module Options =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"animation\"")>]
        static member inline animation: Key<float> = nativeOnly
        [<Emit("\"group\"")>]
        static member inline group: Key<string> = nativeOnly
        [<Emit("\"setData\"")>]
        static member inline setData: Key<Options.setData> = nativeOnly

    type setData =
        delegate of dataTransfer: string * draggedElement: float -> unit

[<AllowNullLiteral>]
[<Interface>]
type Sortable =
    abstract member option<'K>: name: Options.Key<'K> * value: 'K -> unit
    abstract member option<'K>: name: Options.Key<'K> -> 'K

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
