module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Stdio =
    abstract member command: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Http =
    abstract member uri: string with get, set

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
