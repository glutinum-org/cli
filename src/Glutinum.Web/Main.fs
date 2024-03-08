module Glutinum.Web.Main

open Elmish
open Elmish.React
open Elmish.HMR
open Elmish.UrlParser
open Elmish.Navigation
open Fable.Core.JsInterop
open Thoth.Elmish
open Glutinum.Iconify
open Feliz
open Feliz.Bulma
open Feliz.Iconify
open type Feliz.Iconify.Offline.Exports

importSideEffects "./scss/main.scss"

let renderToastWithBulma =
    { new Toast.IRenderer<IconifyIcon> with
        member __.Toast children color =
            Bulma.notification [
                prop.className color
                prop.children children
            ]

        member __.CloseButton onClick = Bulma.delete [ prop.onClick onClick ]

        member __.Title txt = Bulma.title.h5 txt

        member __.Icon(ico: IconifyIcon) =
            Bulma.icon [
                icon.isMedium
                prop.style [ style.fontSize (length.rem 2.0) ]
                prop.children [ Icon [ icon.icon ico ] ]
            ]

        member __.SingleLayout title message =
            Html.div [
                title
                message
            ]

        member __.Message txt = Html.span txt

        member __.SplittedLayout iconView title message =
            Bulma.columns [
                columns.isGapless
                columns.isVCentered
                prop.children [
                    Bulma.column [
                        // TODO: Ensure that the icon is centered
                        // in the column
                        column.is2
                        prop.children [ iconView ]
                    ]
                    Bulma.column [
                        prop.children [
                            title
                            message
                        ]
                    ]
                ]
            ]

        member __.StatusToColor status =
            match status with
            | Toast.Success -> "is-success"
            | Toast.Warning -> "is-warning"
            | Toast.Error -> "is-danger"
            | Toast.Info -> "is-info"
    }

Program.mkProgram App.init App.update App.view
|> Program.toNavigable (parseHash Router.routeParser) App.setRoute
|> Toast.Program.withToast renderToastWithBulma
|> Program.withReactBatched "elmish-app"
|> Program.run
