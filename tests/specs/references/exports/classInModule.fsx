module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    [<Emit("$0.vscode")>]
    static member inline vscode_
        with get () : vscode.Exports =
            nativeOnly

module vscode =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("new $0.LanguageModelError($1...)")>]
        abstract member LanguageModelError: unit -> LanguageModelError

    [<AllowNullLiteral>]
    [<Interface>]
    type LanguageModelError =
        interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
