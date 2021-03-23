[<AutoOpen>]
module Globals

open Fable.Core

[<Import("*", "assert")>]
let Assert: Node.Assert.IExports = jsNative
