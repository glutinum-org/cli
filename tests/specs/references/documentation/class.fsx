module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Disposable", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Disposable () : Disposable = nativeOnly

/// <summary>
/// Represents a type which can release resources, such
/// as event listening or a timer.
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type Disposable =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
