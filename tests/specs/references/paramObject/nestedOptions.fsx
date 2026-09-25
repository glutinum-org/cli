module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Chart", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Chart (options: ChartOptions) : Chart = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type ChartOptions =
    abstract member title: string with get, set
    abstract member axis: AxisOptions option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (title: string, ?axis: AxisOptions) : ChartOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type AxisOptions =
    abstract member min: float option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Chart =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
