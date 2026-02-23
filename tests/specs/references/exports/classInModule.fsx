module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline vscode
        with get () : vscode_.Exports =
            nativeOnly

module vscode_ =

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
