namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Iterable<'T> = Collections.Generic.IEnumerable<'T>

module IteratorObjectBase =

    [<AllowNullLiteral>]
    [<Interface>]
    type FormDataIterator<'T> =
        inherit Iterable<'T>

    [<AllowNullLiteral>]
    [<Interface>]
    type FormData =
        abstract member entries: unit -> IteratorObjectBase.FormDataIterator<string * string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
