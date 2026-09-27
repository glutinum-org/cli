module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Class", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Class () : Class = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Class =
    [<Emit("""import { Class } from "REPLACE_ME_WITH_MODULE_NAME";
Class.extend($0)""")>]
    static member inline extend (props: obj): obj = nativeOnly
    [<Emit("""import { Class } from "REPLACE_ME_WITH_MODULE_NAME";
Class.include($0)""")>]
    static member inline ``include`` (props: obj): obj = nativeOnly
    [<Emit("""import { Class } from "REPLACE_ME_WITH_MODULE_NAME";
Class.mergeOptions($0)""")>]
    static member inline mergeOptions (props: obj): obj = nativeOnly
    [<Emit("""import { Class } from "REPLACE_ME_WITH_MODULE_NAME";
Class.addInitHook($0)""")>]
    static member inline addInitHook (initHookFn: (unit -> unit)): obj = nativeOnly
    [<Emit("""import { Class } from "REPLACE_ME_WITH_MODULE_NAME";
Class.addInitHook($0, $1)""")>]
    static member inline addInitHook (methodName: string, [<ParamArray>] args: obj []): obj = nativeOnly
    [<Emit("""import { Class } from "REPLACE_ME_WITH_MODULE_NAME";
Class.callInitHooks()""")>]
    static member inline callInitHooks () : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
