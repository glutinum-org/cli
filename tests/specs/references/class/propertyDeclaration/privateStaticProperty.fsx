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
[<Import("SettingsContainer", "REPLACE_ME_WITH_MODULE_NAME")>]
type SettingsContainer =
    [<Emit("""import { SettingsContainer } from "REPLACE_ME_WITH_MODULE_NAME";
SettingsContainer.#privateField{{=$0}}""")>]
    static member inline private ``#privateField``
        with get () : obj =
            nativeOnly
        and set (value: obj) =
            nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
