module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type ErrorHandling =
    abstract member success: bool with get, set
    abstract member error: string with get, set

[<AllowNullLiteral>]
type ArtworksData =
    abstract member artworks: ResizeArray<string> with get, set

[<AllowNullLiteral>]
type ArtworksResponse =
    abstract member success: bool with get, set
    abstract member error: string with get, set
    abstract member artworks: ResizeArray<string> with get, set

(***)
#r "nuget: Fable.Core"
(***)
