module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <summary>
    /// Maximum allowed age.
    /// </summary>
    [<Import("maxAge", "REPLACE_ME_WITH_MODULE_NAME")>]
    [<Obsolete("This option no longer has any effect.\nSend `max_age` yourself and validate the `auth_time` claim\nof the returned token.")>]
    static member inline maxAge: float = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
