module Docs.Site

open Feliz.ViewEngine
open Nacara.Core
open Nacara.Plugins
open Nacara.Theme

/// The packages published from this repository, read from their Release builds
let apiOptions =
    { FSharpApi.defaults with
        Root = "reference"
        Title = "API reference"
        Sources =
            [
                FSharpApiSource.create
                    "../src/Glutinum.Types/bin/Release/netstandard2.1/Glutinum.Types.dll"
                FSharpApiSource.create
                    "../bindings/Glutinum.Web/bin/Release/netstandard2.1/Glutinum.Web.dll"
                FSharpApiSource.create
                    "../bindings/Glutinum.Node/bin/Release/netstandard2.1/Glutinum.Node.dll"
            ]
    }

let theme =
    Theme.defaults
    |> Theme.navbar
        [
            NavbarSection("Guide", "guide", "guide/getting-started.md")
            NavbarSection("Reference", "reference", "/reference/")
            // NavbarSection("Bindings", "bindings", "bindings/index.md")
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
            Menu.section
                "Reference"
                [
                    Menu.page "guide/command-line.md"
                    Menu.page "guide/mapping.md"
                    Menu.page "guide/nuget-packages.md"
                ]
        ]
    // |> Theme.menu
    //     "bindings"
    //     [
    //         Menu.section
    //             "Bindings"
    //             [ Menu.page "bindings/index.md"; Menu.page "bindings/contributing.md" ]
    //     ]
    |> Theme.editUrl "https://github.com/glutinum-org/cli/edit/main/docs"
    |> Theme.footer (
        Html.p
            [
                Html.text "Built with "
                Html.a [ prop.href "https://github.com/MangelMaxime/Nacara"; prop.text "Nacara" ]
                Html.text " · "
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

let reference =
    FSharpApi.collection "reference" DocFrontMatter.decoder apiOptions
    |> Collection.title _.Title
    |> Collection.layout (Theme.layout theme)

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
    |> FSharpApi.register apiOptions
    |> Theme.register theme
    |> Site.collection content
    |> Site.collection reference

[<EntryPoint>]
let main argv = Nacara.run site argv
