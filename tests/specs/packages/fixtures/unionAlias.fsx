namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module UnionAlias =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("AST", "union-alias"); EmitConstructor>]
        static member AST () : AST = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type AST =
        abstract member ``type``: UnionAlias.ast.ExtglobType option with get, set

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type Local =
        | a
        | b

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type WithLocal =
        | a
        | b
        | c

    module ast =

        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.None)>]
        type ExtglobType =
            | ``!``
            | ``?``
            | [<CompiledName("+")>] _PLUS_
            | [<CompiledName("*")>] _STAR_
            | [<CompiledName("@")>] _AT_

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
