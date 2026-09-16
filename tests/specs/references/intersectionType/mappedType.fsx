module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Value =
    abstract member raw: string with get, set

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Keys =
    | Accept
    | Location

[<AllowNullLiteral>]
[<Interface>]
type Headers =
    abstract member Accept: string option with get, set
    abstract member Location: string option with get, set

[<AllowNullLiteral>]
[<Interface>]
type WithExtra =
    abstract member Accept: string option with get, set
    abstract member Location: string option with get, set
    abstract member extra: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Required =
    abstract member Accept: Value with get, set
    abstract member Location: Value with get, set
    abstract member other: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
