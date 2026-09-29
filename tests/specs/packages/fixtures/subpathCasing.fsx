namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

module SubpathCasing =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("version", "subpath-casing")>]
        static member inline version: string = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Options =
        abstract member verbose: bool with get, set

    module Fs =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("readFile", "subpath-casing/Fs")>]
            static member readFile (path: string, ?options: SubpathCasing.Internal.helper.Options) : string = nativeOnly

    module Internal =

        module helper =

            [<AllowNullLiteral>]
            [<Interface>]
            type Options =
                abstract member encoding: string with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (encoding: string) : Options = nativeOnly

    module addDays =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("addDays", "subpath-casing/addDays")>]
            static member addDays (date: Date, amount: float) : Date = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
