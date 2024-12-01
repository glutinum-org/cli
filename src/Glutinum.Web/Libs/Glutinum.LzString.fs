module rec Glutinum.LzString

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("compressToBase64", "lz-string")>]
    static member compressToBase64(input: string) : string = nativeOnly

    [<Import("decompressFromBase64", "lz-string")>]
    static member decompressFromBase64(input: string) : string = nativeOnly

    [<Import("compressToUTF16", "lz-string")>]
    static member compressToUTF16(input: string) : string = nativeOnly

    [<Import("decompressFromUTF16", "lz-string")>]
    static member decompressFromUTF16(compressed: string) : string = nativeOnly

    [<Import("compressToUint8Array", "lz-string")>]
    static member compressToUint8Array(uncompressed: string) : JS.Uint8Array = nativeOnly

    [<Import("decompressFromUint8Array", "lz-string")>]
    static member decompressFromUint8Array(compressed: JS.Uint8Array) : string = nativeOnly

    [<Import("compressToEncodedURIComponent", "lz-string")>]
    static member compressToEncodedURIComponent(input: string) : string = nativeOnly

    [<Import("decompressFromEncodedURIComponent", "lz-string")>]
    static member decompressFromEncodedURIComponent(compressed: string) : string = nativeOnly

    [<Import("compress", "lz-string")>]
    static member compress(input: string) : string = nativeOnly

    [<Import("decompress", "lz-string")>]
    static member decompress(compressed: string) : string = nativeOnly
