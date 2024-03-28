module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type SettingsContainer =
    static member inline private ``#privateField``
        with get () : unit =
            emitJsExpr () $$"""
import { SettingsContainer } from "module";
SettingsContainer.#privateField"""
        and set (value: unit) =
            emitJsExpr (value) $$"""
import { SettingsContainer } from "module";
SettingsContainer.#privateField = $0"""

(***)
#r "nuget: Fable.Core"
(***)
