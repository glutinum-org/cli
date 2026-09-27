module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type X =
    abstract member a: string with get, set
    abstract member b: float with get, set
    abstract member c: bool with get, set

[<AllowNullLiteral>]
[<Interface>]
type Y =
    inherit Pick<X, Y.Extends>

module Y =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type Extends =
        | a
        | c

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
