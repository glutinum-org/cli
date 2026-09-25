module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (options: Huge) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Huge =
    abstract member a1: U3<bool, string, float> option with get, set
    abstract member a2: U3<bool, string, float> option with get, set
    abstract member a3: U3<bool, string, float> option with get, set
    abstract member a4: U3<bool, string, float> option with get, set
    abstract member a5: U3<bool, string, float> option with get, set
    abstract member a6: U3<bool, string, float> option with get, set
    abstract member a7: U3<bool, string, float> option with get, set
    abstract member a8: U3<bool, string, float> option with get, set
    abstract member a9: U3<bool, string, float> option with get, set
    abstract member a10: U3<bool, string, float> option with get, set
    abstract member a11: U3<bool, string, float> option with get, set
    abstract member a12: U3<bool, string, float> option with get, set
    abstract member a13: U3<bool, string, float> option with get, set
    abstract member a14: U3<bool, string, float> option with get, set
    abstract member a15: U3<bool, string, float> option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?a1: U3<bool, string, float>, ?a2: U3<bool, string, float>, ?a3: U3<bool, string, float>, ?a4: U3<bool, string, float>, ?a5: U3<bool, string, float>, ?a6: U3<bool, string, float>, ?a7: U3<bool, string, float>, ?a8: U3<bool, string, float>, ?a9: U3<bool, string, float>, ?a10: U3<bool, string, float>, ?a11: U3<bool, string, float>, ?a12: U3<bool, string, float>, ?a13: U3<bool, string, float>, ?a14: U3<bool, string, float>, ?a15: U3<bool, string, float>) : Huge = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
