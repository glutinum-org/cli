module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("TextDocumentLanguageFeature", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member TextDocumentLanguageFeature<'PO, 'RO, 'CO> () : TextDocumentLanguageFeature<'PO, 'RO, 'CO> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member selector: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type NotifyingFeature<'P> =
    abstract member onNotificationSent: 'P with get, set

[<AllowNullLiteral>]
[<Interface>]
type TextDocumentLanguageFeature<'PO, 'RO, 'CO> =
    inherit NotifyingFeature<'RO>
    abstract member onNotificationSent: 'RO with get, set
    abstract member getOptions: unit -> 'RO

type TextDocumentLanguageFeature<'PO, 'RO> =
    TextDocumentLanguageFeature<'PO, 'RO, obj>

[<AllowNullLiteral>]
[<Interface>]
type SendFeature<'T> =
    abstract member send: 'T with get, set

[<AllowNullLiteral>]
[<Interface>]
type CM<'C, 'S> =
    abstract member client: 'C with get, set
    abstract member server: 'S with get, set

type CM =
    CM<string option, string option>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
