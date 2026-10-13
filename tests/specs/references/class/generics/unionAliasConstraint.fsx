module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Stdio =
    abstract member command: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (command: string) : Stdio = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Http =
    abstract member uri: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (uri: string) : Http = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type ServerDefinition =
    | Stdio of Stdio
    | Http of Http

    [<Emit("$0")>]
    static member op_Implicit(value: Stdio) : ServerDefinition = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Stdio) : ServerDefinition = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: Http) : ServerDefinition = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Http) : ServerDefinition = nativeOnly

    /// Unchecked, the value as <c>Stdio</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsStdio: Stdio = nativeOnly

    /// Unchecked, the value as <c>Http</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsHttp: Http = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type ServerDefinitionProvider<'T> =
    abstract member provide: unit -> ResizeArray<'T>

type ServerDefinitionProvider =
    ServerDefinitionProvider<ServerDefinition>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
