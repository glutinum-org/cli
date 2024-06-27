module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Logger =
    /// <summary>
    /// Emits a warning message.
    /// </summary>
    /// <param name="message">
    /// The warning message to be logged
    /// </param>
    abstract member warning: message: string -> unit

(***)
#r "nuget: Fable.Core"
(***)
