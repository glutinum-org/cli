module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Type2<'A> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Task =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Type1<'A when 'A :> Task> =
    interface end

type Type2 =
    Type2<string>

type Type1 =
    Type1<Task>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
