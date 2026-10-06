module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Props =
    abstract member shape: Props.shape with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (shape: Props.shape) : Props = nativeOnly

module Props =

    [<RequireQualifiedAccess>]
    [<TypeScriptTaggedUnion("kind", CaseRules.None)>]
    type shape =
        | a of value: string
        | b of count: float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
