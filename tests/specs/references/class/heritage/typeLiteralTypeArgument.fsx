module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Base", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Base<'I> () : Base<'I> = nativeOnly
    [<Import("RenameFeature", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member RenameFeature () : RenameFeature = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Base<'I> =
    abstract member files: ReadonlyArray<'I> with get, set

[<AllowNullLiteral>]
[<Interface>]
type RenameFeature =
    inherit Base<RenameFeature.Extends>
    abstract member register: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type RenameEvent =
    inherit Base<RenameEvent.Extends>
    abstract member token: string with get, set

module RenameFeature =

    [<Global>]
    [<AllowNullLiteral>]
    type Extends
        [<ParamObject; Emit("$0")>]
        (
            oldUri: string,
            newUri: string
        ) =

        member val oldUri : string = nativeOnly with get, set
        member val newUri : string = nativeOnly with get, set

module RenameEvent =

    [<Global>]
    [<AllowNullLiteral>]
    type Extends
        [<ParamObject; Emit("$0")>]
        (
            oldUri: string,
            newUri: string
        ) =

        member val oldUri : string = nativeOnly with get, set
        member val newUri : string = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
