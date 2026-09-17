module Glutinum.Web.Tests.Page

open System
open Fable.Core
open Glutinum

open type Glutinum.Web.Exports

// The page exercises the binding in a real browser, the test reads the results in the DOM

let private report (id: string) (text: string) =
    let element = document.createElement "p"
    element.id <- id
    element.textContent <- text
    document.body.appendChild element |> ignore

document.title <- "Glutinum.Web tests"

let title = document.createElement "h1"
title.id <- "title"
title.textContent <- "Hello from Fable"
title.classList.add ("box", "demo")
title.style.padding <- "1rem"
document.body.appendChild title |> ignore

// Events: the listener is a delegate
let button = document.createElement "button"
button.id <- "button"
button.textContent <- "Click me"
let mutable clicks = 0

button.addEventListener (
    "click",
    fun (event: Web.Event) ->
        clicks <- clicks + 1
        report "clicked" $"clicks: {clicks} type: {event.``type``}"
)

// Typed events: the key carries the event type, `ev` is a `PointerEvent`
button.addEventListener (
    Web.HTMLElementEventMap.Keys.click,
    fun ev -> report "typed" $"pointer: {ev.clientX >= 0.0} {ev.pointerType}"
)

document.body.appendChild button |> ignore

// Typed elements: the key carries the element type, `input` is an `HTMLInputElement`
let input = document.createElement Web.HTMLElementTagNameMap.Keys.input
input.value <- "typed"
report "created" input.value

// Querying: without a type argument the result is an `Element`
match document.querySelector "#title" with
| Some element -> report "query" element.tagName
| None -> report "query" "not found"

match document.querySelector<Web.HTMLElement> "#title" with
| Some element -> report "query-typed" element.innerText
| None -> report "query-typed" "not found"

// Iterating a NodeList
let tags =
    document.querySelectorAll "h1, button"
    |> Seq.map (fun node -> node.nodeName)
    |> String.concat ","

report "nodelist" tags

// Timers
setTimeout (Action(fun () -> report "timeout" "fired"), 10.0) |> ignore

// Fetch, from a data URL since the page is a local file
promise {
    let! response = fetch "data:application/json,{\"answer\":42}"
    let! data = response.json ()
    report "fetch" $"{response.status} {JS.JSON.stringify data}"
}
|> Promise.start
