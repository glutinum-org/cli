module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type CreateArtistBioBase =
    abstract member artistID: string with get, set
    abstract member thirdParty: bool option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Pagination =
    abstract member page: float with get, set
    abstract member pageSize: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type CreateArtistBioRequest =
    abstract member artistID: string with get, set
    abstract member thirdParty: bool option with get, set
    abstract member html: string with get, set
    abstract member markdown: string with get, set
    abstract member page: float with get, set
    abstract member pageSize: float with get, set

(***)
#r "nuget: Fable.Core"
(***)
