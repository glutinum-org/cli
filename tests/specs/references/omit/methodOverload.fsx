module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ScreenshotOptions =
    abstract member base64: bool option with get, set
    abstract member save: bool option with get, set

[<AllowNullLiteral>]
[<Interface>]
type BrowserPage =
    abstract member screenshot: options: BrowserPage.screenshot.options -> string
    abstract member screenshot: options: BrowserPage.screenshot.options_1 -> float

module BrowserPage =

    module screenshot =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member base64: bool option with get, set

        [<AllowNullLiteral>]
        [<Interface>]
        type options_1 =
            abstract member save: bool option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
