namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module NodeLike =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Global("Buffer")>]
        static member inline Buffer: NodeLike.BufferConstructor = nativeOnly
        [<Global("process")>]
        static member inline ``process``: NodeLike.NodeJS.Process = nativeOnly
        [<Global("queueMicrotask")>]
        static member queueMicrotask (callback: (unit -> unit)) : unit = nativeOnly

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type BufferEncoding =
        | utf8
        | hex

    [<AllowNullLiteral>]
    [<Interface>]
    type BufferConstructor =
        abstract member from: data: string * ?encoding: NodeLike.BufferEncoding -> NodeLike.Buffer

    [<AllowNullLiteral>]
    [<Interface>]
    type Buffer =
        abstract member toString: ?encoding: NodeLike.BufferEncoding -> string

    type ImplicitBlob =
        string

    module NodeJS =

        [<AllowNullLiteral>]
        [<Interface>]
        type Process =
            abstract member cwd: unit -> string
            abstract member platform: string with get, set

    module buffer =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<ImportAll("buffer")>]
            static member inline buffer
                with get () : buffer_.Exports =
                    nativeOnly

        module buffer_ =

            [<AbstractClass>]
            [<Erase>]
            type Exports =
                [<Emit("$0.isUtf8($1...)")>]
                abstract member isUtf8: input: NodeLike.Buffer -> bool

            type ImplicitArrayBuffer =
                obj

    module fs =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("readFileSync", "fs")>]
            static member readFileSync (path: string, encoding: NodeLike.BufferEncoding) : string = nativeOnly
            [<Import("readFileSync", "fs")>]
            static member readFileSync (path: string) : NodeLike.Buffer = nativeOnly
            [<Import("cwd", "fs")>]
            static member cwd () : NodeLike.NodeJS.Process = nativeOnly

    module Exports =

        type buffer =
            buffer.Exports

        type fs =
            fs.Exports

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
