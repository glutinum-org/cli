namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module Dexie =

    [<AllowNullLiteral>]
    [<Interface>]
    type Table<'T> =
        abstract member name: string with get, set
        abstract member get: key: string -> JS.Promise<'T>

    [<AllowNullLiteral>]
    [<Interface>]
    type Dexie =
        abstract member tables: ResizeArray<Dexie.Table<obj>> with get, set
        abstract member Table: Dexie_.Table with get, set
        abstract member table: name: string -> Dexie.Table<obj>

    [<AllowNullLiteral>]
    [<Interface>]
    type Transaction =
        abstract member db: Dexie.Dexie with get, set
        abstract member table: name: string -> Dexie.Table<obj>

    module Dexie_ =

        [<AllowNullLiteral>]
        [<Interface>]
        type Table =
            abstract member prototype: Dexie.Table<obj> with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (prototype: Dexie.Table<obj>) : Table = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
