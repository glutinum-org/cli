namespace Glutinum.Types

open Fable.Core.JsInterop
open Glutinum.Types.TypeScript

[<RequireQualifiedAccess>]
module ReadonlyArray =

    let inline ofArray (values: 'T[]) : ReadonlyArray<'T> = !!values

    let inline ofResizeArray (values: ResizeArray<'T>) : ReadonlyArray<'T> = !!values

    let inline ofList (values: 'T list) : ReadonlyArray<'T> = !!(Array.ofList values)

    let inline ofSeq (values: seq<'T>) : ReadonlyArray<'T> = !!(Array.ofSeq values)
