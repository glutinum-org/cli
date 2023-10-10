(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type Options =
    abstract level: float with get, set
