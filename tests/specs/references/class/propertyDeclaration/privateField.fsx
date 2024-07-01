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
    interface end

(***)
#r "nuget: Fable.Core"
(***)
