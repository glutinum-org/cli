module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type App =
    [<Obsolete("Use `fire` instead.\n```ts\nimport { fire } from 'hono/service-worker'\nfire(app)\n```")>]
    abstract member fire: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
