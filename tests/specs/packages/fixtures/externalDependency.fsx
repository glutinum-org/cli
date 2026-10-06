namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module ExternalDependency =

    [<AllowNullLiteral>]
    [<Interface>]
    type Logger =
        abstract member stream: obj with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (stream: obj) : Logger = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
