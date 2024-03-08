module Glutinum.Web.Components.Loader

open Fable.Core
open Feliz

[<ImportDefault("./Loader.module.scss")>]
let private classes: CssModules.Components.Loader = nativeOnly

[<Erase>]
type Loader =
    static member Loader() =
        Html.div [
            prop.className classes.container
            prop.children [ Html.span [ prop.className classes.loader ] ]
        ]
