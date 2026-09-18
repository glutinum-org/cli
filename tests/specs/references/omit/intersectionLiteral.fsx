module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Message =
    abstract member uid: string with get, set
    abstract member nsp: string with get, set
    abstract member ``type``: string with get, set
    abstract member data: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Publisher =
    abstract member publish: message: Publisher.publish.message -> unit

[<AllowNullLiteral>]
[<Interface>]
type Distributive<'T> =
    abstract member send: message: obj -> unit

[<AllowNullLiteral>]
[<Interface>]
type Sender =
    abstract member send: message: Sender.send.message -> unit
    abstract member name: string with get, set

module Publisher =

    module publish =

        [<AllowNullLiteral>]
        [<Interface>]
        type message =
            abstract member ``type``: string with get, set
            abstract member data: float with get, set

module Sender =

    module send =

        [<AllowNullLiteral>]
        [<Interface>]
        type message =
            abstract member nsp: string with get, set
            abstract member ``type``: string with get, set
            abstract member data: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
