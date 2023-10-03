(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<RequireQualifiedAccess>]
[<StringEnum>]
type Direction =
    | [<CompiledName("UP")>] Up
    | [<CompiledName("DOWN")>] Down
    | [<CompiledName("LEFT")>] Left
    | [<CompiledName("RIGHT")>] Right
