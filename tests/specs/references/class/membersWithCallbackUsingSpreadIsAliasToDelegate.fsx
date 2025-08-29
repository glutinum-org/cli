module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Commander", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Commander () : Commander = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Commander =
    abstract member action: fn: System.Delegate -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
