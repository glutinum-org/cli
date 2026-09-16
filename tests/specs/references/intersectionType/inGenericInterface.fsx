module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type KeyFormat =
    | pem
    | der
    | jwk

[<AllowNullLiteral>]
[<Interface>]
type BasePrivateKeyEncodingOptions<'T> =
    abstract member format: 'T with get, set
    abstract member cipher: string option with get, set

[<AllowNullLiteral>]
[<Interface>]
type RSAKeyPairOptions<'PubF, 'PrivF> =
    abstract member modulusLength: float with get, set
    abstract member privateKeyEncoding: RSAKeyPairOptions.privateKeyEncoding<'PrivF> with get, set

module RSAKeyPairOptions =

    [<AllowNullLiteral>]
    [<Interface>]
    type privateKeyEncoding<'PrivF> =
        abstract member format: 'PrivF with get, set
        abstract member cipher: string option with get, set
        abstract member ``type``: RSAKeyPairOptions.privateKeyEncoding.``type`` with get, set

    module privateKeyEncoding =

        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.None)>]
        type ``type`` =
            | pkcs1
            | pkcs8

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
