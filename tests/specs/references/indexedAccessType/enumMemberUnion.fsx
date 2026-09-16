module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("createMetaProperty", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member createMetaProperty (keywordToken: SyntaxKind) : unit = nativeOnly

[<RequireQualifiedAccess>]
type SyntaxKind =
    | ImportKeyword = 102
    | NewKeyword = 105

[<AllowNullLiteral>]
[<Interface>]
type MetaProperty =
    abstract member keywordToken: U2<SyntaxKind, SyntaxKind> with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
