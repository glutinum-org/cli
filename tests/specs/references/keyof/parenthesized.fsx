module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Foo =
    abstract member a: string with get, set
    abstract member b: float with get, set

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type FooKeys =
    | a
    | b

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
