module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("helper", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member helper () : unit = nativeOnly
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Client (?baseURL: string) : Client = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
type Client =
    abstract member health: unit -> bool

[<AllowNullLiteral>]
[<Interface>]
type Internal =
    abstract member hidden: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
