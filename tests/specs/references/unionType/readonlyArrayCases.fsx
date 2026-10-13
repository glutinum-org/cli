module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("names", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member names (value: ResizeArray<string>) : unit = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type Extension =
    | Case1 of Extension.Cases.Case1
    | ExtensionArray of ResizeArray<Extension>

    [<Emit("$0")>]
    static member op_Implicit(value: Extension.Cases.Case1) : Extension = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Extension.Cases.Case1) : Extension = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<Extension>) : Extension = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<Extension>) : Extension = nativeOnly

    /// Unchecked, the value as <c>Case1</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase1: Extension.Cases.Case1 = nativeOnly

    /// Unchecked, the value as <c>ExtensionArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsExtensionArray: ResizeArray<Extension> = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type Names =
    | Case1 of Names.Cases.Case1
    | StringArray of ResizeArray<string>

    [<Emit("$0")>]
    static member op_Implicit(value: Names.Cases.Case1) : Names = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Names.Cases.Case1) : Names = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<string>) : Names = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<string>) : Names = nativeOnly

    /// Unchecked, the value as <c>Case1</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase1: Names.Cases.Case1 = nativeOnly

    /// Unchecked, the value as <c>StringArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsStringArray: ResizeArray<string> = nativeOnly

module Extension =

    module Cases =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case1 =
            abstract member extension: Extension with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (extension: Extension) : Case1 = nativeOnly

module Names =

    module Cases =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case1 =
            abstract member name: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (name: string) : Case1 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
