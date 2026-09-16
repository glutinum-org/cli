module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline constants
        with get () : constants_.Exports =
            nativeOnly

module constants_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.RTLD_LAZY")>]
        abstract member RTLD_LAZY: float
        [<Emit("$0.RTLD_NOW")>]
        abstract member RTLD_NOW: float

[<AllowNullLiteral>]
[<Interface>]
type Dl =
    abstract member flags: obj with get, set

type Flags =
    obj

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
