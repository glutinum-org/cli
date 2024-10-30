module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type FirebaseSignInProvider =
    | [<CompiledName("apple.com")>] apple_com

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
