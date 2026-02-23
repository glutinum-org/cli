module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Disposable2", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Disposable2 () : Disposable2 = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Disposable2 =
    /// <summary>
    /// Dispose this object.
    /// </summary>
    abstract member dispose: unit -> obj

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
