module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("styles", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline styles: Exports.styles.Type = nativeOnly

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Signals =
    | SIGINT
    | SIGTERM

[<AllowNullLiteral>]
[<Interface>]
type SignalConstants =
    abstract member SIGINT: float with get, set
    abstract member SIGTERM: float with get, set

module Exports =

    module styles =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            abstract member special: string with get, set
            abstract member number: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
