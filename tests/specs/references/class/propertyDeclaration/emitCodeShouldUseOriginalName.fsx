module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Fuse", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Fuse () : Fuse = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("Fuse", "REPLACE_ME_WITH_MODULE_NAME")>]
type Fuse =
    [<Emit("""import { Fuse } from "REPLACE_ME_WITH_MODULE_NAME";
Fuse.member{{=$0}}""")>]
    static member inline ``member``
        with get () : string =
            nativeOnly
        and set (value: string) =
            nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
