module Glutinum.Web.Tests.Main

open Fable.Core
open Scriptorium.Nib.Browser
open Glutinum.Playwright

open type Scriptorium.Nib.Browser.UserEvents
open type Scriptorium.Nib.Browser.BrowserTest
open type Scriptorium.Quill.Runner
open type Scriptorium.Quill.Test

[<Import("readFileSync", "node:fs")>]
let private readFileSync (path: string, encoding: string) : string = jsNative

// The page built by `./build.sh test bindings` from `page/Page.fs` exercises the binding in Chromium.
// Chromium refuses a module script of a `file://` page, so the bundle is inlined in the content.
let private pageContent =
    let html = readFileSync ("page/dist/index.html", "utf8")

    let script =
        System.Text.RegularExpressions.Regex.Match(html, "assets/[^\"]+\\.js").Value

    let bundle = readFileSync ("page/dist/" + script, "utf8")

    $"<!doctype html><html><head><meta charset=\"utf-8\"></head><body><script type=\"module\">{bundle}</script></body></html>"

let private testPageText (name: string) (selector: string) (expected: string) =
    testPage (
        name,
        fun page ->
            promise {
                do! page.setContent pageContent
                do! assertLocator (page.locator selector) (haveText expected)
            }
    )

[<EntryPoint>]
let main _ =
    runTests
        [
            testList (
                "Glutinum.Web",
                [
                    testPageText "the DOM is built from F#" "#title" "Hello from Fable"
                    testPageText "querySelector gives an Element" "#query" "H1"
                    testPageText
                        "querySelector with a type argument"
                        "#query-typed"
                        "Hello from Fable"
                    testPageText "a NodeList is enumerable" "#nodelist" "H1,BUTTON"
                    testPageText "setTimeout takes a lambda" "#timeout" "fired"
                    testPageText "fetch resolves a response" "#fetch" "200 {\"answer\":42}"
                    testPageText
                        "createElement with a typed key gives the element type"
                        "#created"
                        "typed"

                    testPage (
                        "addEventListener with a typed key gives the event type",
                        fun page ->
                            promise {
                                do! page.setContent pageContent
                                do! click (page.locator "#button")

                                do!
                                    assertLocator
                                        (page.locator "#typed")
                                        (haveText "pointer: true mouse")
                            }
                    )

                    testPage (
                        "an event listener is a delegate",
                        fun page ->
                            promise {
                                do! page.setContent pageContent
                                do! click (page.locator "#button")

                                do!
                                    assertLocator
                                        (page.locator "#clicked")
                                        (haveText "clicks: 1 type: click")
                            }
                    )
                ]
            )
        ]
