module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("get", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member get () : Redefines = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Base =
    abstract member value: U2<string, float> with get, set
    abstract member other: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Redefines =
    abstract member value: string with get, set
    abstract member other: float option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
