module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("g", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member g (options: TooMany) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type TooMany =
    abstract member a: U2<bool, string> option with get, set
    abstract member b: U2<bool, string> option with get, set
    abstract member c: U2<bool, string> option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?a: U2<bool, string>, ?b: U2<bool, string>, ?c: U2<bool, string>) : TooMany = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
