module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<TypeScriptTaggedUnion("kind", CaseRules.None)>]
type Geometry =
    | point of at: Geometry.Cases.point.at
    | line of from: Geometry.Cases.line.from * ``to``: Geometry.Cases.line.``to``

module Geometry =

    module Cases =

        module point =

            [<AllowNullLiteral>]
            [<Interface>]
            type at =
                abstract member x: float with get, set
                abstract member y: float with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (x: float, y: float) : at = nativeOnly

        module line =

            [<AllowNullLiteral>]
            [<Interface>]
            type from =
                abstract member x: float with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (x: float) : from = nativeOnly

            [<AllowNullLiteral>]
            [<Interface>]
            type ``to`` =
                abstract member x: float with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (x: float) : ``to`` = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
