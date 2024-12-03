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
type ExtensionTerminalOptions =
    abstract member suffix: string with get, set

type Foo =
    U2<Foo.U2.ReadOnlyTerminalOptions, Foo.U2.ReadOnlyExtensionTerminalOptions>

module Foo =

    module U2 =

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyTerminalOptions =
            abstract member prefix: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyExtensionTerminalOptions =
            abstract member suffix: string with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
