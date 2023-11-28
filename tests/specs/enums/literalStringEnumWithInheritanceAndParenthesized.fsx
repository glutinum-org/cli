module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<RequireQualifiedAccess>]
[<StringEnum>]
type ColorA =
    | Black

[<RequireQualifiedAccess>]
[<StringEnum>]
type ColorB =
    | BgBlack

[<RequireQualifiedAccess>]
[<StringEnum>]
type ColorC =
    | Red

[<RequireQualifiedAccess>]
[<StringEnum>]
type Color =
    | Black
    | BgBlack
    | Red
