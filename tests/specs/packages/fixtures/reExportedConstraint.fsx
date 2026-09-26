namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

module ReExportedConstraint =

    type Pipeline<'TKey> =
        ReExportedConstraint.stream.Pipeline<'TKey>

    type Sink<'TName> =
        ReExportedConstraint.stream.Sink<'TName>

    type Store<'TKey> =
        ReExportedConstraint.stream.Store<'TKey>

    module stream =

        type Key =
            ReadonlyArray<obj>

        [<AllowNullLiteral>]
        [<Interface>]
        type Pipeline<'TKey> =
            abstract member key: 'TKey with get, set

        [<AllowNullLiteral>]
        [<Interface>]
        type Sink<'TName> =
            abstract member name: 'TName with get, set

        [<AllowNullLiteral>]
        [<Interface>]
        type Store<'TKey> =
            abstract member key: 'TKey with get, set

        type Sink =
            Sink<U2<string, float>>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
