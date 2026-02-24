module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Options =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type User<'T when 'T :> Options> =
    interface end

type User =
    User<Options>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
