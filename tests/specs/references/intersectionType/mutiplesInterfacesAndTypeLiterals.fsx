module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type CreateArtistBioBase =
    abstract member artistID: string with get, set
    abstract member thirdParty: bool option with get, set

[<AllowNullLiteral>]
type Pagination =
    abstract member page: float with get, set
    abstract member pageSize: float with get, set

[<AllowNullLiteral>]
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
