module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Type3<'A, 'B, 'C> =
    interface end

type Type3<'A, 'B> =
    Type3<'A, 'B, obj>

type Type3<'A> =
    Type3<'A, string, obj>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
