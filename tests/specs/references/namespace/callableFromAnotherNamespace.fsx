module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline _DOLLAR_dbx
        with get () : _DOLLAR_dbx_.Exports =
            nativeOnly

module _DOLLAR_dbx_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.not")>]
        abstract member not: dbx_.not

    [<AutoOpen>]
    module ExportsExtensions =

        type Exports with
            member inline this.not(e: dbx_.Expression) : dbx_.Expression =
                this.not.Invoke(e)

module dbx_ =

    [<AllowNullLiteral>]
    [<Interface>]
    type Expression =
        abstract member build: unit -> string

    type not =
        delegate of e: Expression -> Expression

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
