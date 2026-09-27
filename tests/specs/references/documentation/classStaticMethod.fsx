module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Disposable3", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Disposable3 () : Disposable3 = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Disposable3 =
    /// <summary>
    /// Dispose this object.
    /// </summary>
    [<Obsolete("Use the static dispose method instead.")>]
    [<Emit("""import { Disposable3 } from "REPLACE_ME_WITH_MODULE_NAME";
Disposable3.dispose()""")>]
    static member inline dispose () : obj = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
