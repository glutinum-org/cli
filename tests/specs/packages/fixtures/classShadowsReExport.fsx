namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module ClassShadowsReExport =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("getLastGrammarState", "class-shadows-re-export")>]
        static member getLastGrammarState (code: string) : ClassShadowsReExport.types.types.GrammarState = nativeOnly
        [<Import("GrammarState", "class-shadows-re-export"); EmitConstructor>]
        static member GrammarState () : GrammarState = nativeOnly

    type Position =
        ClassShadowsReExport.types.types.Position

    [<AllowNullLiteral>]
    [<Interface>]
    [<Import("GrammarState", "class-shadows-re-export")>]
    type GrammarState =
        inherit ClassShadowsReExport.types.types.GrammarState
        abstract member theme: string with get
        abstract member lang: string with get

    module types =

        module types =

            [<AllowNullLiteral>]
            [<Interface>]
            type GrammarState =
                abstract member theme: string with get
                [<ParamObject; Emit("$0")>]
                static member Create (theme: string) : GrammarState = nativeOnly

            [<AllowNullLiteral>]
            [<Interface>]
            type Position =
                abstract member line: float with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (line: float) : Position = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
