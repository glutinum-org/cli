module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("g", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member g (options: TooMany) : unit = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type TooMany
    [<ParamObject; Emit("$0")>]
    (
        ?a: U2<bool, string>,
        ?b: U2<bool, string>,
        ?c: U2<bool, string>
    ) =

    member val a : U2<bool, string> option = nativeOnly with get, set
    member val b : U2<bool, string> option = nativeOnly with get, set
    member val c : U2<bool, string> option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
