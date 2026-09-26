module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("scale", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member scale (value: Exports.scale__.value) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Foo =
    abstract member a: string with get, set

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type WithNumber =
    | auto
    | Case1 of float

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type WithString =
    | black
    | red
    | Case1 of string

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type WithInterface =
    | none
    | Case1 of Foo

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type WithTypeLiteral =
    | none
    | Case1 of WithTypeLiteral.Cases.Case1

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type WithBooleanLiteralAndUndefined =
    | auto
    | [<CompiledValue(true)>] True
    | Case1 of ResizeArray<string>

[<AllowNullLiteral>]
[<Interface>]
type Props =
    abstract member colorScale: Props.colorScale option with get, set

module WithTypeLiteral =

    module Cases =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case1 =
            abstract member x: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (x: string) : Case1 = nativeOnly

module Props =

    [<RequireQualifiedAccess>]
    [<Erase(CaseRules.None)>]
    type colorScale =
        | grayscale
        | blue
        | Case1 of ResizeArray<string>

module Exports =

    module scale__ =

        [<RequireQualifiedAccess>]
        [<Erase(CaseRules.None)>]
        type value =
            | grayscale
            | blue
            | Case1 of float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
