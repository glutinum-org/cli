module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("id", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member id (value: string) : unit = nativeOnly

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Upper =
    | ABC

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Events =
    | Click
    | Key

[<AllowNullLiteral>]
[<Interface>]
type Loose<'T> =
    interface end

type Loose =
    Loose<string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
