module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

/// <summary>
/// Defines a type for plugin functions.
/// </summary>
/// <typeparam name="T">
/// The type of the <c>option</c> parameter
/// </typeparam>
type PluginFunc<'T> =
    'T

(***)
#r "nuget: Fable.Core"
(***)
