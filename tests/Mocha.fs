module Mocha

open Fable.Core

let [<Global>] describe (name: string) (f: unit->unit) : unit = jsNative
let [<Global>] it (msg: string) (f: unit->unit) : unit = jsNative
let [<Global; Emit("it($1...)")>] itAsync (msg: string) (f: (obj->unit)->unit) : unit = jsNative
let [<Global>] beforeEach (f: unit->unit) : unit = jsNative
