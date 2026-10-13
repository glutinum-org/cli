namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module AliasedImportHeritage =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("StreamLanguage", "aliased-import-heritage"); EmitConstructor>]
        static member StreamLanguage<'State> () : StreamLanguage<'State> = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type StreamParser<'State> =
        inherit StreamBase.StreamParser<'State>
        abstract member tokenTable: StreamParser.tokenTable option with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    [<Import("StreamLanguage", "aliased-import-heritage")>]
    type StreamLanguage<'State> =
        [<Emit("""import { StreamLanguage } from "aliased-import-heritage";
StreamLanguage.define($0)""")>]
        static member inline define (spec: AliasedImportHeritage.StreamParser<'State>): AliasedImportHeritage.StreamLanguage<'State> = nativeOnly

    module StreamParser =

        [<AllowNullLiteral>]
        [<Interface>]
        type tokenTable =
            [<EmitIndexer>]
            abstract member Item: name: string -> string with get, set

module StreamBase =

    [<AllowNullLiteral>]
    [<Interface>]
    type StreamParser<'State> =
        abstract member name: string option with get, set
        abstract member startState: (float -> 'State) option with get, set
        abstract member token: stream: string * state: 'State -> string option

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
