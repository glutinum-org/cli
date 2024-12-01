module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Iterable<'T> = Collections.Generic.IEnumerable<'T>

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("DataTransfer", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member DataTransfer () : DataTransfer = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type DataTransfer =
    inherit Iterable<string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
