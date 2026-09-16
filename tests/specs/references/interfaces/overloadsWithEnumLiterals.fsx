module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
type SyntaxKind =
    | SuperKeyword = 108
    | ThisKeyword = 110

[<AllowNullLiteral>]
[<Interface>]
type SuperExpression =
    abstract member kind: SyntaxKind with get

[<AllowNullLiteral>]
[<Interface>]
type ThisExpression =
    abstract member kind: SyntaxKind with get

[<AllowNullLiteral>]
[<Interface>]
type NodeFactory =
    abstract member createToken: token: SyntaxKind -> SuperExpression

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
