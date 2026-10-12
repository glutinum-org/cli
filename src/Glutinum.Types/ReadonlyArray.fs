namespace Glutinum.Types

open Fable.Core
open Glutinum.Types.TypeScript

[<RequireQualifiedAccess>]
module ReadonlyArray =

    [<Emit("$0")>]
    let ofArray (values: 'T[]) : ReadonlyArray<'T> = nativeOnly

    [<Emit("$0")>]
    let ofResizeArray (values: ResizeArray<'T>) : ReadonlyArray<'T> = nativeOnly

    [<Emit("Array.from($0)")>]
    let ofList (values: 'T list) : ReadonlyArray<'T> = nativeOnly

    [<Emit("Array.from($0)")>]
    let ofSeq (values: seq<'T>) : ReadonlyArray<'T> = nativeOnly
