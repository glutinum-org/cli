module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("DayJs", "module"); EmitConstructor>]
    static member DayJs () : DayJs = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type DayJs =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type PluginFunc =
    [<Emit("$0($1...)")>]
    abstract member Invoke: c: DayJs -> unit

(***)
#r "nuget: Fable.Core"
(***)
