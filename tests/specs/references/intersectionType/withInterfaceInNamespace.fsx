module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ArtworksData =
    abstract member artworks: ResizeArray<string> with get, set

module Error =

    [<AllowNullLiteral>]
    [<Interface>]
    type ErrorHandling =
        abstract member success: bool with get, set
        abstract member error: string option with get, set

[<AllowNullLiteral>]
[<Interface>]
type ArtworksResponse =
    abstract member artworks: ResizeArray<string> with get, set
    abstract member success: bool with get, set
    abstract member error: string option with get, set

(***)
#r "nuget: Fable.Core"
(***)
