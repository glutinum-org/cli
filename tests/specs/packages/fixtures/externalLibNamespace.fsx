namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Web NuGet package to your project

module ExternalLibNamespace =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("instantiate", "external-lib-namespace")>]
        static member instantiate (bytes: Glutinum.Web.BufferSource) : JS.Promise<Glutinum.Web.WebAssembly_.Instance> = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Loaded =
        abstract member exports: Glutinum.Web.WebAssembly_.Exports with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (exports: Glutinum.Web.WebAssembly_.Exports) : Loaded = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
