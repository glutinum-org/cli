module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("date", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline date: Date = nativeOnly
    [<Import("MyDate", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member MyDate () : MyDate = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("MyDate", "REPLACE_ME_WITH_MODULE_NAME")>]
type MyDate =
    abstract member toDate: unit -> Date

[<RequireQualifiedAccess>]
[<Erase>]
type MyDateUnion =
    | String of string
    | Date of Date

    [<Emit("$0")>]
    static member op_Implicit(value: string) : MyDateUnion = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : MyDateUnion = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: Date) : MyDateUnion = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Date) : MyDateUnion = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Date</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsDate: Date = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
