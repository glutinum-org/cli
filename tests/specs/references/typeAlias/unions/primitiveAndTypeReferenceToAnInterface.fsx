module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type FormatObject =
    abstract member locale: string option with get, set
    abstract member format: string option with get, set
    abstract member utc: bool option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?locale: string, ?format: string, ?utc: bool) : FormatObject = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type OptionType =
    | FormatObject of FormatObject
    | String of string
    | StringArray of ResizeArray<string>

    [<Emit("$0")>]
    static member op_Implicit(value: FormatObject) : OptionType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: FormatObject) : OptionType = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: string) : OptionType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : OptionType = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<string>) : OptionType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<string>) : OptionType = nativeOnly

    /// Unchecked, the value as <c>FormatObject</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsFormatObject: FormatObject = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>StringArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsStringArray: ResizeArray<string> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
