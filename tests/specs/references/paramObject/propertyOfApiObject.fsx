module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("createTerminal", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member createTerminal (options: TerminalOptions) : Terminal = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type TerminalOptions =
    abstract member name: string option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Terminal =
    abstract member options: TerminalOptions with get
    abstract member dispose: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
