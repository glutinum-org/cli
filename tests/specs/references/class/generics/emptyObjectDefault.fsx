module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("TracingChannel", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member TracingChannel<'StoreType> () : TracingChannel<'StoreType> = nativeOnly
    [<Import("Named", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Named<'Options> () : Named<'Options> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Collection<'StoreType, 'ContextType> =
    abstract member start: 'StoreType with get, set
    abstract member context: 'ContextType with get, set

[<AllowNullLiteral>]
[<Interface>]
type TracingChannel<'StoreType, 'ContextType> =
    inherit Collection<'StoreType, 'ContextType>
    abstract member start: 'StoreType with get, set
    abstract member context: 'ContextType with get, set

type TracingChannel<'StoreType> =
    TracingChannel<'StoreType, obj>

type TracingChannel =
    TracingChannel<obj, obj>

[<AllowNullLiteral>]
[<Interface>]
type Named<'Options> =
    abstract member options: 'Options with get, set

type Named =
    Named<Named.Options>

type Collection<'StoreType> =
    Collection<'StoreType, 'StoreType>

type Collection =
    Collection<obj, obj>

module Named =

    [<Global>]
    [<AllowNullLiteral>]
    type Options
        [<ParamObject; Emit("$0")>]
        (
            verbose: bool
        ) =

        member val verbose : bool = nativeOnly with get, set

module Exports =

    module Named =

        [<Global>]
        [<AllowNullLiteral>]
        type Options
            [<ParamObject; Emit("$0")>]
            (
                verbose: bool
            ) =

            member val verbose : bool = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
