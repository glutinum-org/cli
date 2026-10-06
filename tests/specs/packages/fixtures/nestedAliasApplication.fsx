namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module NestedAliasApplication =

    type Callback<'T> =
        delegate of event: 'T -> unit

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

    [<AllowNullLiteral>]
    [<Interface>]
    type OpaqueRoom =
        abstract member id: string with get
        abstract member events: OpaqueRoom.events with get
        [<ParamObject; Emit("$0")>]
        static member Create (id: string, events: OpaqueRoom.events) : OpaqueRoom = nativeOnly

    module Room =

        [<AllowNullLiteral>]
        [<Interface>]
        type events<'P> =
            abstract member presence: Room.events.presence<'P> with get
            abstract member connection: Room.events.connection with get
            [<ParamObject; Emit("$0")>]
            static member Create (presence: Room.events.presence<'P>, connection: Room.events.connection) : events<'P> = nativeOnly

        module events =

            [<AllowNullLiteral>]
            [<Interface>]
            type presence<'P> =
                abstract member subscribe: callback: NestedAliasApplication.Callback<'P> -> unit
                abstract member waitUntil: ?predicate: ('P -> bool) -> JS.Promise<'P>
                [<ParamObject; Emit("$0")>]
                static member Create (subscribe: (NestedAliasApplication.Callback<'P> -> unit), waitUntil: (('P -> bool) option -> JS.Promise<'P>)) : presence<'P> = nativeOnly

            [<AllowNullLiteral>]
            [<Interface>]
            type connection =
                abstract member subscribe: callback: NestedAliasApplication.Callback<unit> -> unit
                abstract member waitUntil: ?predicate: (unit -> bool) -> JS.Promise<unit>
                [<ParamObject; Emit("$0")>]
                static member Create (subscribe: (NestedAliasApplication.Callback<unit> -> unit), waitUntil: ((unit -> bool) option -> JS.Promise<unit>)) : connection = nativeOnly

    module OpaqueRoom =

        [<AllowNullLiteral>]
        [<Interface>]
        type events =
            abstract member presence: NestedAliasApplication.Observable<string> with get
            abstract member connection: OpaqueRoom.events.connection with get
            [<ParamObject; Emit("$0")>]
            static member Create (presence: NestedAliasApplication.Observable<string>, connection: OpaqueRoom.events.connection) : events = nativeOnly

        module events =

            [<AllowNullLiteral>]
            [<Interface>]
            type connection =
                abstract member subscribe: callback: NestedAliasApplication.Callback<unit> -> unit
                abstract member waitUntil: ?predicate: (unit -> bool) -> JS.Promise<unit>
                [<ParamObject; Emit("$0")>]
                static member Create (subscribe: (NestedAliasApplication.Callback<unit> -> unit), waitUntil: ((unit -> bool) option -> JS.Promise<unit>)) : connection = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
