module Fable.Core.JsInterop

open Fable.Core

[<Emit("delete $0")>]
let inline jsDelete<'T> (v: 'T) : unit = jsNative

[<Emit("undefined")>]
let inline jsUndefined<'T> : 'T = jsNative

[<Emit("typeof $0")>]
let internal jsTypeOf _ : string = jsNative
