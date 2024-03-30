module Glutinum.Converter.Log

open Glutinum.Chalk
open Fable.Core

let success (text: string) =
    JS.console.log (chalk.greenBright.Invoke text)

let log (text: string) = JS.console.log text

let info (text: string) =
    JS.console.info (chalk.blueBright.Invoke text)

let warn (text: string) =
    JS.console.warn (chalk.yellowBright.Invoke text)

let error (text: string) =
    JS.console.error (chalk.redBright.Invoke text)
