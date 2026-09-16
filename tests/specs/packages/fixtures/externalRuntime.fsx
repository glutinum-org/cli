module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module DomLibUser =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("Panel", "dom-lib-user"); EmitConstructor>]
        static member Panel () : Panel = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Widget =
        abstract member element: Glutinum.Web.HTMLElement with get, set
        abstract member stats: Glutinum.Node.Stats with get, set
        abstract member onClick: listener: Glutinum.Web.EventListener -> unit
        abstract member ``open``: unit -> JS.Promise<Glutinum.Web.Response>

    [<AllowNullLiteral>]
    [<Interface>]
    type Panel =
        inherit Glutinum.Web.EventTarget
        abstract member render: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
