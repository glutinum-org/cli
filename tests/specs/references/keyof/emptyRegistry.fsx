module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("create", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member create<'M> (id: 'M) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Mutators<'S, 'A> =
    interface end

type MutatorIdentifier =
    obj

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
