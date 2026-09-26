module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("funcA", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member funcA<'R> (task: Exports.funcA__.task<'R>) : Thenable<'R> = nativeOnly
    [<Import("funcB", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member funcB<'R> (task: Exports.funcB__.task<'R>) : Thenable<'R> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Thenable<'R> =
    interface end

module Exports =

    module funcA__ =

        type task<'R> =
            delegate of progress: obj * data: obj -> Thenable<'R>

    module funcB__ =

        type task<'R> =
            delegate of progress: obj * data: obj -> Thenable<'R> * Thenable<'R>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
