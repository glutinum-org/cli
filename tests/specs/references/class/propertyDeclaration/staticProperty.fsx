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
type Fuse =
    [<Emit("""import { Fuse } from "REPLACE_ME_WITH_MODULE_NAME";
Fuse.version{{=$0}}""")>]
    static member inline version
        with get () : string =
            nativeOnly
        and set (value: string) =
            nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
