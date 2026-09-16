module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Sep =
    | [<CompiledName("\\")>] _BACKSLASH_
    | [<CompiledName("/")>] _SLASH_

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type ExtglobType =
    | ``!``
    | ``?``
    | [<CompiledName("+")>] _PLUS_
    | [<CompiledName("*")>] _STAR_
    | [<CompiledName("@")>] _AT_

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Brackets =
    | [<CompiledName("[")>] _LBRACKET_
    | [<CompiledName("]")>] _RBRACKET_
    | [<CompiledName("&")>] _AMP_
    | [<CompiledName("\"")>] _QUOTE_
    | ``(``
    | ``)``
    | ``=``

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
