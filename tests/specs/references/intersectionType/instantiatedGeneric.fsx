module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member selector: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type RegistrationType<'RO> =
    abstract member ``method``: string with get, set
    abstract member options: 'RO with get, set

[<AllowNullLiteral>]
[<Interface>]
type DynamicFeature<'RO> =
    abstract member registrationType: RegistrationType<'RO> with get, set
    abstract member register: options: 'RO -> ResizeArray<'RO>
    abstract member plain: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type SendFeature<'T> =
    abstract member send: 'T with get, set

[<AllowNullLiteral>]
[<Interface>]
type TextDocumentFeature =
    abstract member registrationType: RegistrationType<Options> with get, set
    abstract member register: options: Options -> ResizeArray<Options>
    abstract member plain: float with get, set
    abstract member send: (string -> JS.Promise<unit>) with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
