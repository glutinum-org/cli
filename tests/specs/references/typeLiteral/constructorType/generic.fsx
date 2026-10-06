module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Options<'T> =
    abstract member markerClass: Options.markerClass<'T> option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Literal<'T> =
    abstract member factory: Literal.factory<'T> with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (factory: Literal.factory<'T>) : Literal<'T> = nativeOnly

module Options =

    [<AllowNullLiteral>]
    [<Interface>]
    type markerClass<'T> =
        [<EmitConstructor>]
        abstract member Create: unit -> 'T

module Literal =

    [<AllowNullLiteral>]
    [<Interface>]
    type factory<'T> =
        [<EmitConstructor>]
        abstract member Create: unit -> 'T

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
