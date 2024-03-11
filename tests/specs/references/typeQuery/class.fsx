module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("DayJs", "module"); EmitConstructor>]
    static member DayJs () : DayJs = nativeOnly

[<AllowNullLiteral>]
type DayJs =
    interface end

[<AllowNullLiteral>]
type PluginFunc =
    [<Emit("$0($1...)")>]
    abstract member Invoke: c: DayJs -> unit

(***)
#r "nuget: Fable.Core"
(***)
