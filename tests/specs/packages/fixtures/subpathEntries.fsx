namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module SubpathEntries =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("Scope", "subpath-entries"); EmitConstructor>]
        static member Scope () : Scope = nativeOnly

    type Scope =
        SubpathEntries.scope_scope.Scope

    module auto =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("auto", "subpath-entries/auto")>]
            static member auto () : unit = nativeOnly

    module scope =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("Scope", "subpath-entries/scope/index.js"); EmitConstructor>]
            static member Scope () : Scope = nativeOnly

        type Scope =
            SubpathEntries.scope_scope.Scope

    module scope_scope =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("Scope", "subpath-entries/scope/scope.js"); EmitConstructor>]
            static member Scope () : Scope = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type Scope =
            abstract member revert: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
