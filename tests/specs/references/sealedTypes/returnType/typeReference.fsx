module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("showInformationMessage", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member showInformationMessage ([<ParamArray>] items: string []) : Thenable<string option> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Thenable<'T> =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
