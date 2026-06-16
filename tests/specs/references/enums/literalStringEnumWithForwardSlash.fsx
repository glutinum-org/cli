module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type VectorizePreset =
    | [<CompiledName("@cf/baai/bge-small-en-v1.5")>] ``_AT_cf_SLASH_baai_SLASH_bge-small-en-v1_5``
    | [<CompiledName("openai/text-embedding-ada-002")>] ``openai_SLASH_text-embedding-ada-002``

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
