module Docs.Site

open Feliz.ViewEngine
open Nacara.Core
open Nacara.Plugins
open Nacara.Theme

let theme =
    Theme.defaults
    |> Theme.navbar
        [
            NavbarSection("Guide", "guide", "guide/getting-started.md")
            NavbarSection("Reference", "reference", "reference/command-line.md")
            NavbarSection("Bindings", "bindings", "bindings/index.md")
            NavbarSection("Try it", "app", "app.md")
        ]
    |> Theme.navbarEnd
        [
            NavbarDynamicWidget Search.trigger
            NavbarIcon("GitHub", "https://github.com/glutinum-org/cli", Icons.github)
        ]
    |> Theme.menu
        "guide"
        [
            Menu.section
                "Start"
                [
                    Menu.page "guide/getting-started.md"
                    Menu.page "guide/try-it-online.md"
                    Menu.page "guide/single-file.md"
                    Menu.page "guide/packages.md"
                ]
            Menu.section
                "Use the output"
                [
                    Menu.page "guide/reading-the-output.md"
                    Menu.page "guide/limitations.md"
                    Menu.page "guide/extending.md"
                ]
        ]
    |> Theme.menu
        "reference"
        [
            Menu.section
                "Reference"
                [
                    Menu.page "reference/command-line.md"
                    Menu.page "reference/mapping.md"
                    Menu.page "reference/packages.md"
                ]
        ]
    |> Theme.menu
        "bindings"
        [
            Menu.section
                "Bindings"
                [ Menu.page "bindings/index.md"; Menu.page "bindings/contributing.md" ]
        ]
    |> Theme.editUrl "https://github.com/glutinum-org/cli/edit/main/docs"
    |> Theme.footer (
        Html.p
            [
                Html.text "Glutinum is built with F# · "
                Html.a [ prop.href "https://github.com/glutinum-org/cli"; prop.text "Source" ]
            ]
    )

let content =
    Theme.docs theme "content"
    // A host serves `404.html` from the root, not `404/index.html`
    |> Collection.route (fun page ->
        if RelativePath.value page.RelativePath = "404.md" then
            Route.file page.Locale "404.html"
        else
            Collection.defaultRoute page
    )

let site =
    Site.create "Glutinum"
    |> Site.description
        "F# bindings for JavaScript libraries, generated from their TypeScript declarations"
    |> Site.baseUrl "/"
    |> Site.origin "https://glutinum.net"
    |> Site.output "output"
    |> Site.staticFiles "static"
    |> Site.stylesheet "assets/landing.css"
    |> Markdown.register
    |> TreeSitter.register
    |> Search.register
    |> Sitemap.register
    |> LightningCss.register
    |> Nuglify.minifyHtml
    |> Nuglify.minifyJs
    |> GitHubPages.register
    |> Theme.register theme
    |> Site.collection content

[<EntryPoint>]
let main argv = Nacara.run site argv
