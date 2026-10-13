module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: string) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: string, value: string) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: string, value: float) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: string, value: bool) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: Buffer) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: Buffer, value: string) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: Buffer, value: float) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: Buffer, value: bool) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: URL) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: URL, value: string) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: URL, value: float) : unit = nativeOnly
    [<Import("open", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``open`` (path: URL, value: bool) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Buffer =
    abstract member length: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (length: float) : Buffer = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type PathLike =
    | String of string
    | Buffer of Buffer
    | URL of URL

    [<Emit("$0")>]
    static member op_Implicit(value: string) : PathLike = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : PathLike = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: Buffer) : PathLike = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Buffer) : PathLike = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: URL) : PathLike = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: URL) : PathLike = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Buffer</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsBuffer: Buffer = nativeOnly

    /// Unchecked, the value as <c>URL</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsURL: URL = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type Value =
    | String of string
    | Number of float
    | Bool of bool

    [<Emit("$0")>]
    static member op_Implicit(value: string) : Value = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : Value = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: float) : Value = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : Value = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: bool) : Value = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: bool) : Value = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

    /// Unchecked, the value as <c>Bool</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsBool: bool = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type Items =
    | StringArray of ResizeArray<string>
    | NumberArray of ResizeArray<float>

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<string>) : Items = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<string>) : Items = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<float>) : Items = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<float>) : Items = nativeOnly

    /// Unchecked, the value as <c>StringArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsStringArray: ResizeArray<string> = nativeOnly

    /// Unchecked, the value as <c>NumberArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumberArray: ResizeArray<float> = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type Shape =
    | Case1 of Shape.Cases.Case1
    | Case2 of Shape.Cases.Case2

    [<Emit("$0")>]
    static member op_Implicit(value: Shape.Cases.Case1) : Shape = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Shape.Cases.Case1) : Shape = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: Shape.Cases.Case2) : Shape = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Shape.Cases.Case2) : Shape = nativeOnly

    /// Unchecked, the value as <c>Case1</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase1: Shape.Cases.Case1 = nativeOnly

    /// Unchecked, the value as <c>Case2</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase2: Shape.Cases.Case2 = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type Tag =
    | String of string
    | Buffer of Buffer

    [<Emit("$0")>]
    static member op_Implicit(value: string) : Tag = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : Tag = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: Buffer) : Tag = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Buffer) : Tag = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Buffer</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsBuffer: Buffer = nativeOnly

type Maybe =
    U2<string, Buffer> option

module Shape =

    module Cases =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case1 =
            abstract member kind: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (kind: string) : Case1 = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type Case2 =
            abstract member size: float with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (size: float) : Case2 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
