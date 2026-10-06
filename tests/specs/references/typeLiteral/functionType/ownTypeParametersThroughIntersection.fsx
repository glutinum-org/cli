module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Base =
    abstract member kind: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (kind: string) : Base = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Req<'G, 'S> =
    abstract member g: 'G with get, set
    abstract member s: 'S with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (g: 'G, s: 'S) : Req<'G, 'S> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type ServerOptions<'Server, 'Logger> =
    abstract member frameworkErrors: ServerOptions.frameworkErrors<Base, 'Server> option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?frameworkErrors: ServerOptions.frameworkErrors<Base, 'Server>) : ServerOptions<'Server, 'Logger> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type HttpOptions<'Server, 'Logger> =
    abstract member frameworkErrors: HttpOptions.frameworkErrors<'Server, obj> option with get, set
    abstract member http: bool with get, set

type ServerOptions<'Server> =
    ServerOptions<'Server, bool>

type HttpOptions<'Server> =
    HttpOptions<'Server, bool>

module ServerOptions =

    type frameworkErrors<'G, 'Server> =
        delegate of error: string * req: Req<'G, 'Server> * res: Req<'G, 'Server> -> unit

module HttpOptions =

    type frameworkErrors<'Server, 'G> =
        delegate of error: string * req: Req<'G, 'Server> * res: Req<'G, 'Server> -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
