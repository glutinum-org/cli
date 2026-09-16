module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("getLine", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member getLine (sourceFile: SourceFileLike) : float = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type SourceFileLike =
    abstract member text: string with get
    abstract member getLineAndCharacterOfPosition: pos: float -> float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
