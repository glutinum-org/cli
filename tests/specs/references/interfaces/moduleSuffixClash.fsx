module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Module =
    abstract member ``type``: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Formatter =
    abstract member add: name: string * fc: Formatter_.add.fc -> unit

[<AllowNullLiteral>]
[<Interface>]
type FormatterModule =
    inherit Module
    inherit Formatter
    abstract member ``type``: string with get, set

module Formatter_ =

    module add =

        type fc =
            delegate of value: obj * lng: string option * options: obj -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
