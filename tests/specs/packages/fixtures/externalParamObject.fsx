namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Web NuGet package to your project

module DomExtender =

    [<AllowNullLiteral>]
    [<Interface>]
    type MyListenerOptions =
        inherit Glutinum.Web.AddEventListenerOptions
        abstract member label: string with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type MyTarget =
        inherit Glutinum.Web.EventTarget
        abstract member id: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
