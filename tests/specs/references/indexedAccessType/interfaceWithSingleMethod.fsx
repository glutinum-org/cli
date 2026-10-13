module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ConfigTypeMap =
    abstract member ``default``: U2<string, float> with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``default``: string) : ConfigTypeMap = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (``default``: float) : ConfigTypeMap = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type ConfigType =
    | String of string
    | Number of float

    [<Emit("$0")>]
    static member op_Implicit(value: string) : ConfigType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : ConfigType = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: float) : ConfigType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : ConfigType = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
