namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

module UnboundTypeParameter =

    [<AllowNullLiteral>]
    [<Interface>]
    type Register =
        interface end

    type QueryKey =
        ReadonlyArray<obj>

    [<AllowNullLiteral>]
    [<Interface>]
    type QueryFunctionContext<'TQueryKey, 'TPageParam> =
        interface end

    type QueryFunction<'T, 'TQueryKey, 'TPageParam> =
        delegate of context: obj -> 'T

    [<AllowNullLiteral>]
    [<Interface>]
    type QueryPersister<'T, 'TQueryKey, 'TPageParam> =
        interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type OmitKeyof<'TObject, 'TKey when 'TKey :> obj> =
        interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type Options<'TQueryFnData, 'TQueryKey, 'TPageParam> =
        abstract member queryKey: 'TQueryKey with get, set
        abstract member persister: (UnboundTypeParameter.QueryFunction<'TQueryFnData, 'TQueryKey, obj> -> 'TQueryFnData) option with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type Client =
        abstract member setDefaults<'TQueryFnData>: options: Client.setDefaults.options<'TQueryFnData> -> unit

    type QueryFunctionContext<'TQueryKey> =
        QueryFunctionContext<'TQueryKey, obj>

    type QueryFunctionContext =
        QueryFunctionContext<ReadonlyArray<obj>, obj>

    type QueryFunction<'T, 'TQueryKey> =
        QueryFunction<'T, 'TQueryKey, obj>

    type QueryFunction<'T> =
        QueryFunction<'T, ReadonlyArray<obj>, obj>

    type QueryFunction =
        QueryFunction<obj, ReadonlyArray<obj>, obj>

    type QueryPersister<'T, 'TQueryKey> =
        QueryPersister<'T, 'TQueryKey, obj>

    type QueryPersister<'T> =
        QueryPersister<'T, ReadonlyArray<obj>, obj>

    type QueryPersister =
        QueryPersister<obj, ReadonlyArray<obj>, obj>

    type Options<'TQueryFnData, 'TQueryKey> =
        Options<'TQueryFnData, 'TQueryKey, obj>

    type Options<'TQueryFnData> =
        Options<'TQueryFnData, ReadonlyArray<obj>, obj>

    type Options =
        Options<obj, ReadonlyArray<obj>, obj>

    module Client =

        module setDefaults =

            [<AllowNullLiteral>]
            [<Interface>]
            type options<'TQueryFnData> =
                abstract member persister: (UnboundTypeParameter.QueryFunction<'TQueryFnData, ReadonlyArray<obj>, obj> -> 'TQueryFnData) option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
