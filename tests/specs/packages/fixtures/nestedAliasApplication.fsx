namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module NestedAliasApplication =

    type Callback<'T> =
        'T -> unit

    [<AllowNullLiteral>]
    [<Interface>]
    type Observable<'T> =
        abstract member subscribe: callback: NestedAliasApplication.Callback<'T> -> unit
        abstract member waitUntil: ?predicate: ('T -> bool) -> JS.Promise<'T>

    [<AllowNullLiteral>]
    [<Interface>]
    type Room<'P> =
        abstract member id: string with get
        abstract member events: Room.events<'P> with get
        [<ParamObject; Emit("$0")>]
        static member Create (id: string, events: Room.events<'P>) : Room<'P> = nativeOnly

    type OpaqueRoom =
        NestedAliasApplication.Room<string>

    module Room =

        [<AllowNullLiteral>]
        [<Interface>]
        type events<'P> =
            abstract member presence: NestedAliasApplication.Observable<'P> with get
            abstract member connection: NestedAliasApplication.Observable<unit> with get
            [<ParamObject; Emit("$0")>]
            static member Create (presence: NestedAliasApplication.Observable<'P>, connection: NestedAliasApplication.Observable<unit>) : events<'P> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
