module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type RegExp = Text.RegularExpressions.Regex

[<AllowNullLiteral>]
[<Interface>]
type EmulatedRegExp =
    abstract member rawFlags: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
