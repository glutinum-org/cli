(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<RequireQualifiedAccess>]
[<StringEnum>]
type ColorC =
    | Black

[<RequireQualifiedAccess>]
[<StringEnum>]
type ColorD =
    | BgBlack

[<RequireQualifiedAccess>]
[<StringEnum>]
type ColorE =
    | Red

[<RequireQualifiedAccess>]
[<StringEnum>]
type ColorF =
    | Black
    | BgBlack
    | Red
