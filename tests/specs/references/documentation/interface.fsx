module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

/// <summary>
/// Represents a type which can release resources, such
/// as event listening or a timer.
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type Disposable =
    abstract member dispose: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
