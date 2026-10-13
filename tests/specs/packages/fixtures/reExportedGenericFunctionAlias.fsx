namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module ReExportedGenericFunctionAlias =

    type FormatRelativeFn =
        delegate of token: string * date: obj -> string

    type Mapper<'T> =
        'T -> unit

    module locale =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("formatRelative", "re-exported-generic-function-alias/locale")>]
            static member inline formatRelative: ReExportedGenericFunctionAlias.FormatRelativeFn = nativeOnly
            [<Import("mapNumber", "re-exported-generic-function-alias/locale")>]
            static member inline mapNumber: ReExportedGenericFunctionAlias.Mapper<float> = nativeOnly

        type FormatRelativeFn =
            ReExportedGenericFunctionAlias.FormatRelativeFn

        type Mapper<'T> =
            ReExportedGenericFunctionAlias.Mapper<'T>

    module Exports =

        type locale =
            locale.Exports

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
