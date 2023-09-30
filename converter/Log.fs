module Glutinum.Converter.Log

open Glutinum.Chalk
open Fable.Core

let success (text : string) = JS.console.log(chalk.greenBright.Invoke text)

let info (text : string) = JS.console.log(chalk.blueBright.Invoke text)

let warn (text : string) = JS.console.log(chalk.yellowBright.Invoke text)

let error (text : string) = JS.console.log(chalk.redBright.Invoke text)

let debug (text : string) = JS.console.log text
