module Glutinum.Converter.Log

open Glutinum.Chalk
open Fable.Core

let success (text: string) =
    JS.console.error (chalk.greenBright.Invoke text)

let log (text: string) = JS.console.error text

let info (text: string) =
    JS.console.error (chalk.blueBright.Invoke text)

let warn (text: string) =
    JS.console.error (chalk.yellowBright.Invoke text)

let error (text: string) =
    JS.console.error (chalk.redBright.Invoke text)

let debug (text: string) =
    JS.console.error (chalk.gray.Invoke text)
