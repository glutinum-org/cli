module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("format", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member format (date: Date, formatStr: string) : string = nativeOnly
    [<Import("format", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member format (date: float, formatStr: string) : string = nativeOnly
    [<Import("format", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member format (date: string, formatStr: string) : string = nativeOnly
    [<Import("run", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member run<'T> (value: 'T, options: Options) : unit = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type DateArg =
    | Date of Date
    | Number of float
    | String of string

    [<Emit("$0")>]
    static member op_Implicit(value: Date) : DateArg = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Date) : DateArg = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: float) : DateArg = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : DateArg = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: string) : DateArg = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : DateArg = nativeOnly

    /// Unchecked, the value as <c>Date</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsDate: Date = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member value: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: string) : Options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
