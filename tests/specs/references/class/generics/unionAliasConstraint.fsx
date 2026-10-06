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

type ServerDefinition =
    U2<Stdio, Http>

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
