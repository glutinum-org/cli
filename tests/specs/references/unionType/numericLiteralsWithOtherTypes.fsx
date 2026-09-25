module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type WithString =
    | [<CompiledValue(1)>] ``1``
    | [<CompiledValue(2)>] ``2``
    | Case1 of string

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type WithTypeLiteralAndUndefined =
    | [<CompiledValue(0)>] ``0``
    | [<CompiledValue(1)>] ``1``
    | Case1 of WithTypeLiteralAndUndefined.Cases.Case1

[<AllowNullLiteral>]
[<Interface>]
type Props =
    abstract member size: Props.size option with get, set

module WithTypeLiteralAndUndefined =

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
    type size =
        | [<CompiledValue(1)>] ``1``
        | [<CompiledValue(2)>] ``2``
        | Case1 of ResizeArray<float>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
