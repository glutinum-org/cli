module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("send", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member send (payload: Exports.send__.payload) : unit = nativeOnly
    [<Import("send", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member send<'Payload> (payload: 'Payload) : unit = nativeOnly
    [<Import("Service", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Service () : Service = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member debug: bool option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?debug: bool) : Options = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Service =
    abstract member create: unit -> JS.Promise<string>
    abstract member create: bodyParams: Service.create.bodyParams * ?options: Options -> JS.Promise<string>
    abstract member create<'BodyParams>: bodyParams: 'BodyParams * ?options: Options -> JS.Promise<string>
    abstract member create: bodyParams: FormData * ?options: Options -> JS.Promise<string>
    abstract member update: id: string * body: Service.update.body -> unit
    abstract member update<'Body>: id: string * body: 'Body -> unit
    abstract member plain: options: Options -> unit

module Service =

    module create =

        [<AllowNullLiteral>]
        [<Interface>]
        type bodyParams =
            [<EmitIndexer>]
            abstract member Item: key: string -> obj with get, set

    module update =

        [<AllowNullLiteral>]
        [<Interface>]
        type body =
            [<EmitIndexer>]
            abstract member Item: key: string -> obj with get, set

module Exports =

    module send__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type payload =
            [<EmitIndexer>]
            abstract member Item: key: string -> obj with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
