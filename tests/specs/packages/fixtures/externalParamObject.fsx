namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Web NuGet package to your project

module DomExtender =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("listen", "dom-extender")>]
        static member listen (target: DomExtender.MyTarget, options: DomExtender.MyListenerOptions) : unit = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type MyListenerOptions =
        inherit Glutinum.Web.AddEventListenerOptions
        abstract member label: string with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (label: string, ?capture: bool, ?once: bool, ?passive: bool, ?signal: Glutinum.Web.AbortSignal) : MyListenerOptions = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type MyTarget =
        inherit Glutinum.Web.EventTarget
        abstract member id: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
