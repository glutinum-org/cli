module Mocha

open Fable.Core

[<Global>]
let describe (name: string) (f: unit -> unit) : unit = jsNative

[<Global>]
let it (msg: string) (f: unit -> unit) : unit = jsNative

[<Global; Emit("it($1...)")>]
let itAsync (msg: string) (f: (obj -> unit) -> unit) : unit = jsNative

[<Global>]
let beforeEach (f: unit -> unit) : unit = jsNative
