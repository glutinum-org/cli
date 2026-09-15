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

            [<Global>]
            [<AllowNullLiteral>]
            type at
                [<ParamObject; Emit("$0")>]
                (
                    x: float,
                    y: float
                ) =

                member val x : float = nativeOnly with get, set
                member val y : float = nativeOnly with get, set

        module line =

            [<Global>]
            [<AllowNullLiteral>]
            type from
                [<ParamObject; Emit("$0")>]
                (
                    x: float
                ) =

                member val x : float = nativeOnly with get, set

            [<Global>]
            [<AllowNullLiteral>]
            type ``to``
                [<ParamObject; Emit("$0")>]
                (
                    x: float
                ) =

                member val x : float = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
