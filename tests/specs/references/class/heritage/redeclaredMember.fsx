module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("BaseService", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member BaseService () : BaseService = nativeOnly
    [<Import("CrudService", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member CrudService<'M> () : CrudService<'M> = nativeOnly
    [<Import("RecordService", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member RecordService<'M> () : RecordService<'M> = nativeOnly
    [<Import("LogService", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member LogService () : LogService = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type CommonOptions =
    abstract member headers: CommonOptions.headers option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?headers: CommonOptions.headers) : CommonOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type RecordOptions =
    inherit CommonOptions
    abstract member expand: string option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?headers: RecordOptions.headers, ?expand: string) : RecordOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("BaseService", "REPLACE_ME_WITH_MODULE_NAME")>]
type BaseService =
    abstract member client: string with get

[<AllowNullLiteral>]
[<Interface>]
[<Import("CrudService", "REPLACE_ME_WITH_MODULE_NAME")>]
type CrudService<'M> =
    inherit BaseService
    abstract member decode<'T>: data: string -> 'T
    abstract member decode: data: string -> 'M
    abstract member getOne<'T>: id: string * ?options: CommonOptions -> JS.Promise<'T>
    abstract member getOne: id: string * ?options: CommonOptions -> JS.Promise<'M>
    abstract member delete: id: string * ?options: CommonOptions -> JS.Promise<bool>

[<AllowNullLiteral>]
[<Interface>]
[<Import("RecordService", "REPLACE_ME_WITH_MODULE_NAME")>]
type RecordService<'M> =
    inherit BaseService
    abstract member collectionIdOrName: string with get
    abstract member getOne<'T>: id: string * ?options: RecordOptions -> JS.Promise<'T>
    abstract member getOne: id: string * ?options: RecordOptions -> JS.Promise<'M>
    abstract member delete: id: string * ?options: CommonOptions -> JS.Promise<bool>
    abstract member decode<'T>: data: string -> 'T
    abstract member decode: data: string -> 'M

type RecordService =
    RecordService<string>

[<AllowNullLiteral>]
[<Interface>]
[<Import("LogService", "REPLACE_ME_WITH_MODULE_NAME")>]
type LogService =
    inherit CrudService<string>
    abstract member delete: id: string * ?options: CommonOptions -> JS.Promise<bool>

module CommonOptions =

    [<AllowNullLiteral>]
    [<Interface>]
    type headers =
        [<EmitIndexer>]
        abstract member Item: key: string -> string with get, set

module RecordOptions =

    [<AllowNullLiteral>]
    [<Interface>]
    type headers =
        [<EmitIndexer>]
        abstract member Item: key: string -> string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
