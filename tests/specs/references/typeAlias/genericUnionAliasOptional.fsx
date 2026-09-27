module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type UpdateOutputFileStampsProject =
    abstract member kind: int with get, set

[<AllowNullLiteral>]
[<Interface>]
type BuildInvalidedProject<'T> =
    abstract member kind: int with get, set
    abstract member program: 'T with get, set

type InvalidatedProject<'T> =
    U2<UpdateOutputFileStampsProject, BuildInvalidedProject<'T>>

[<AllowNullLiteral>]
[<Interface>]
type SolutionBuilder<'T> =
    abstract member getNextInvalidatedProject: unit -> InvalidatedProject option

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
