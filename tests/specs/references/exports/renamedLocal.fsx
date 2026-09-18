module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("WebMidi", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline WebMidi: WebMidi = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type WebMidi =
    abstract member enabled: bool with get, set
    abstract member enable: unit -> JS.Promise<WebMidi>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
