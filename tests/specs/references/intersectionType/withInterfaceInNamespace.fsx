module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type ArtworksData =
    abstract member artworks: ResizeArray<string> with get, set

module Error =

    [<AllowNullLiteral>]
    type ErrorHandling =
        abstract member success: bool with get, set
        abstract member error: string option with get, set

[<AllowNullLiteral>]
type ArtworksResponse =
    abstract member artworks: ResizeArray<string> with get, set
    abstract member success: bool with get, set
    abstract member error: string option with get, set

(***)
#r "nuget: Fable.Core"
(***)
