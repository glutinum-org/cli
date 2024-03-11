module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type IntrinsicElements =
    abstract member var: string with get, set

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Keys =
    | [<CompiledName("var")>] var

(***)
#r "nuget: Fable.Core"
(***)
