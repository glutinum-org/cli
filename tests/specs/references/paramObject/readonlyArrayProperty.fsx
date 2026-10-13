module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("RangeSet", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member RangeSet () : RangeSet = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Range =
    abstract member from: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (from: float) : Range = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type RangeSetUpdate =
    abstract member add: ReadonlyArray<Range> option with get, set
    abstract member filterFrom: float option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?add: ResizeArray<Range>, ?filterFrom: float) : RangeSetUpdate = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("RangeSet", "REPLACE_ME_WITH_MODULE_NAME")>]
type RangeSet =
    abstract member update: updateSpec: RangeSetUpdate -> RangeSet
    abstract member ranges: ReadonlyArray<Range> with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
