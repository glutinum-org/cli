module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("MyType", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member MyType<'TResult1> () : MyType<'TResult1> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("MyType", "REPLACE_ME_WITH_MODULE_NAME")>]
type MyType<'TResult1> =
    abstract member a: 'TResult1 with get, set

[<RequireQualifiedAccess>]
[<Erase>]
type T<'TResult1> =
    | TResult1 of 'TResult1
    | MyType of MyType<'TResult1>

    [<Emit("$0")>]
    static member op_Implicit(value: 'TResult1) : T<'TResult1> = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: 'TResult1) : T<'TResult1> = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: MyType<'TResult1>) : T<'TResult1> = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: MyType<'TResult1>) : T<'TResult1> = nativeOnly

    /// Unchecked, the value as <c>TResult1</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsTResult1: 'TResult1 = nativeOnly

    /// Unchecked, the value as <c>MyType</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsMyType: MyType<'TResult1> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
