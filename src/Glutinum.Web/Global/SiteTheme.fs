/// The documentation site sets `data-theme` on `<html>`, the editors follow it
module Glutinum.Web.SiteTheme

open Browser
open Fable.Core
open Fable.Core.JsInterop

let isDark () : bool =
    document.documentElement?dataset?theme = "dark"

let monacoTheme () =
    if isDark () then
        "vs-dark"
    else
        "vs"

let mutable private monaco: obj option = None

/// The Monaco instance, kept from the first editor mounted
let register (instance: obj) = monaco <- Some instance

[<Emit("new MutationObserver(() => $0()).observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] })")>]
let private observeAttribute (onChange: unit -> unit) : unit = jsNative

let observe () =
    observeAttribute (fun () ->
        match monaco with
        | Some monaco -> monaco?editor?setTheme (monacoTheme ())
        | None -> ()
    )
