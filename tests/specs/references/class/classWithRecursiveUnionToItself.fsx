module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("MyClass", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member MyClass () : MyClass = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("MyClass", "REPLACE_ME_WITH_MODULE_NAME")>]
type MyClass =
    abstract member contains: otherBoundsOrLatLng: MyClass -> bool
    abstract member contains: otherBoundsOrLatLng: string -> bool

[<RequireQualifiedAccess>]
[<Erase>]
type MyUnion =
    | MyClass of MyClass
    | String of string

    [<Emit("$0")>]
    static member op_Implicit(value: MyClass) : MyUnion = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: MyClass) : MyUnion = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: string) : MyUnion = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : MyUnion = nativeOnly

    /// Unchecked, the value as <c>MyClass</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsMyClass: MyClass = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
