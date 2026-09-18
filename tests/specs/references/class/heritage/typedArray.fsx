module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Buffer", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Buffer<'TArrayBuffer> () : Buffer<'TArrayBuffer> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Buffer<'TArrayBuffer> =
    abstract member write: string: string -> float
    abstract member subarray: ?start: float * ?``end``: float -> Buffer<'TArrayBuffer>

type Buffer =
    Buffer<ArrayBufferLike>

[<AllowNullLiteral>]
[<Interface>]
type Callable =
    abstract member name: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
