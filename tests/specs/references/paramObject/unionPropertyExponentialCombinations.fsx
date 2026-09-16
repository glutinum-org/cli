module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (options: Huge) : unit = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type Huge
    [<ParamObject; Emit("$0")>]
    (
        ?a1: U3<bool, string, float>,
        ?a2: U3<bool, string, float>,
        ?a3: U3<bool, string, float>,
        ?a4: U3<bool, string, float>,
        ?a5: U3<bool, string, float>,
        ?a6: U3<bool, string, float>,
        ?a7: U3<bool, string, float>,
        ?a8: U3<bool, string, float>,
        ?a9: U3<bool, string, float>,
        ?a10: U3<bool, string, float>,
        ?a11: U3<bool, string, float>,
        ?a12: U3<bool, string, float>,
        ?a13: U3<bool, string, float>,
        ?a14: U3<bool, string, float>,
        ?a15: U3<bool, string, float>
    ) =

    member val a1 : U3<bool, string, float> option = nativeOnly with get, set
    member val a2 : U3<bool, string, float> option = nativeOnly with get, set
    member val a3 : U3<bool, string, float> option = nativeOnly with get, set
    member val a4 : U3<bool, string, float> option = nativeOnly with get, set
    member val a5 : U3<bool, string, float> option = nativeOnly with get, set
    member val a6 : U3<bool, string, float> option = nativeOnly with get, set
    member val a7 : U3<bool, string, float> option = nativeOnly with get, set
    member val a8 : U3<bool, string, float> option = nativeOnly with get, set
    member val a9 : U3<bool, string, float> option = nativeOnly with get, set
    member val a10 : U3<bool, string, float> option = nativeOnly with get, set
    member val a11 : U3<bool, string, float> option = nativeOnly with get, set
    member val a12 : U3<bool, string, float> option = nativeOnly with get, set
    member val a13 : U3<bool, string, float> option = nativeOnly with get, set
    member val a14 : U3<bool, string, float> option = nativeOnly with get, set
    member val a15 : U3<bool, string, float> option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
