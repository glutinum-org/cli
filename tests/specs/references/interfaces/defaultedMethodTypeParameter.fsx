module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Query", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Query () : Query = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Element =
    abstract member tagName: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type ParentNode =
    abstract member querySelector<'E>: selectors: string -> 'E option
    abstract member querySelector: selectors: string -> Element option
    abstract member querySelectorAll<'E>: selectors: string -> ResizeArray<'E>
    abstract member querySelectorAll: selectors: string -> ResizeArray<Element>
    abstract member closest<'K>: unit -> 'K

[<AllowNullLiteral>]
[<Interface>]
type Query =
    abstract member find<'E>: selectors: string -> 'E
    abstract member find: selectors: string -> Element

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
