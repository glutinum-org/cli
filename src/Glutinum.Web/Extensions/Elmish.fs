module Elmish

open Elmish

[<RequireQualifiedAccess>]
module Cmd =

    module OfFunc =

        /// Command to evaluate a simple function
        ///
        /// It discards the result of the function and ignores any exception
        let exec (task: 'a -> unit) (arg: 'a) : Cmd<'msg> =
            let bind _dispatch =
                try
                    task arg
                with _ ->
                    ()

            [ bind ]

    module OfPromise =

        /// Command to evaluate a promise
        ///
        /// It discards the result of the promise and ignores any exception
        /// Command to call `promise` block and map the error
        let exec (task: 'a -> Fable.Core.JS.Promise<_>) (arg: 'a) : Cmd<'msg> =
            let bind _dispatch = task arg |> Promise.catchEnd ignore

            [ bind ]
