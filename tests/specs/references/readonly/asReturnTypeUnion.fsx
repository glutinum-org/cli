module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type TerminalOptions =
    abstract member prefix: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type ExtersionTerminalOptions =
    abstract member suffix: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Foo =
    abstract member terminal: U2<Foo.terminal.U2.ReadOnlyTerminalOptions, Foo.terminal.U2.ReadOnlyExtersionTerminalOptions> with get, set

module Foo =

    module terminal =

        module U2 =

            [<AllowNullLiteral>]
            [<Interface>]
            type ReadOnlyTerminalOptions =
                abstract member prefix: string with get

            [<AllowNullLiteral>]
            [<Interface>]
            type ReadOnlyExtersionTerminalOptions =
                abstract member suffix: string with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
