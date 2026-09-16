module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Chart", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Chart (options: ChartOptions) : Chart = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type ChartOptions
    [<ParamObject; Emit("$0")>]
    (
        title: string,
        ?axis: AxisOptions
    ) =

    member val title : string = nativeOnly with get, set
    member val axis : AxisOptions option = nativeOnly with get, set

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
