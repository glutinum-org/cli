module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ArtworksData =
    abstract member artworks: ResizeArray<string> with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (artworks: ResizeArray<string>) : ArtworksData = nativeOnly

module Error_ =

    [<AllowNullLiteral>]
    [<Interface>]
    type ErrorHandling =
        abstract member success: bool with get, set
        abstract member error: string option with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (success: bool, ?error: string) : ErrorHandling = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type ArtworksResponse =
    abstract member artworks: ResizeArray<string> with get, set
    abstract member success: bool with get, set
    abstract member error: string option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
