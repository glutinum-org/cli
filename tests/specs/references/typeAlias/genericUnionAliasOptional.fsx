module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type UpdateOutputFileStampsProject =
    abstract member kind: int with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (kind: int) : UpdateOutputFileStampsProject = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type BuildInvalidedProject<'T> =
    abstract member kind: int with get, set
    abstract member program: 'T with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (kind: int, program: 'T) : BuildInvalidedProject<'T> = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type InvalidatedProject<'T> =
    | UpdateOutputFileStampsProject of UpdateOutputFileStampsProject
    | BuildInvalidedProject of BuildInvalidedProject<'T>

    [<Emit("$0")>]
    static member op_Implicit(value: UpdateOutputFileStampsProject) : InvalidatedProject<'T> = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: UpdateOutputFileStampsProject) : InvalidatedProject<'T> = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: BuildInvalidedProject<'T>) : InvalidatedProject<'T> = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: BuildInvalidedProject<'T>) : InvalidatedProject<'T> = nativeOnly

    /// Unchecked, the value as <c>UpdateOutputFileStampsProject</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsUpdateOutputFileStampsProject: UpdateOutputFileStampsProject = nativeOnly

    /// Unchecked, the value as <c>BuildInvalidedProject</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsBuildInvalidedProject: BuildInvalidedProject<'T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type SolutionBuilder<'T> =
    abstract member getNextInvalidatedProject: unit -> InvalidatedProject option

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
