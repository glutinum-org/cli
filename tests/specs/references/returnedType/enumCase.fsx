module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("test", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member test (scope: TaskScope) : test = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type test =
    interface end

[<RequireQualifiedAccess>]
type TaskScope =
    | Global = 1
    | Workspace = 2

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
