namespace Feliz

open Feliz
open Elmish

[<AutoOpen>]
module Extensions =

    type prop with

        static member inline onClick<'Msg>
            (dispatch: Dispatch<'Msg>, msg: 'Msg)
            =
            prop.onClick (fun _ -> dispatch msg)
