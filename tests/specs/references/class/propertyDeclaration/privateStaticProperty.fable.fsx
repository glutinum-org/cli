module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("SettingsContainer", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member SettingsContainer () : SettingsContainer = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type SettingsContainer =
    static member inline private ``#privateField``
        with get () : unit =
            emitJsExpr () $$"""
import { SettingsContainer } from "REPLACE_ME_WITH_MODULE_NAME";
SettingsContainer.#privateField"""
        and set (value: unit) =
            emitJsExpr (value) $$"""
import { SettingsContainer } from "REPLACE_ME_WITH_MODULE_NAME";
SettingsContainer.#privateField = $0"""

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
