module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Fuse", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Fuse () : Fuse = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Fuse =
    abstract member version: string with get, set

(***)
#r "nuget: Fable.Core"
(***)
