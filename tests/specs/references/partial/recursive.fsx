module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type BrowserPage =
    abstract member extend: methods: BrowserPage.extend.methods -> BrowserPage

module BrowserPage =

    module extend =

        [<AllowNullLiteral>]
        [<Interface>]
        type methods =
            abstract member extend: methods: obj -> BrowserPage

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
