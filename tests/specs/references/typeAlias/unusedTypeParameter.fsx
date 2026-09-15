module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Unused<'T> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type PartiallyUsed<'T, 'U> =
    interface end

type Used<'T> =
    ResizeArray<'T>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
