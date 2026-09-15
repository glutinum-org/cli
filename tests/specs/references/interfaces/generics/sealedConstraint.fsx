module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type TypeMap =
    abstract member string: string with get, set
    abstract member number: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Picker<'K> =
    abstract member key: 'K with get, set

[<AllowNullLiteral>]
[<Interface>]
type Holder =
    abstract member picker: Picker<string> with get, set

type Picker =
    Picker<Picker.K>

module Picker =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type K =
        | string
        | number

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
