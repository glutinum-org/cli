module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("DayJs", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member DayJs () : DayJs = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type DayJs =
    interface end

type PluginFunc =
    delegate of c: DayJs -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
