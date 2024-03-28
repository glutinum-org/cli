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
    interface end

(***)
#r "nuget: Fable.Core"
(***)
