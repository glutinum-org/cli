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
    | Case2 of ResizeArray<Extension>

    [<Emit("$0")>]
    static member op_Implicit(value: Extension.Cases.Case1) : Extension = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Extension.Cases.Case1) : Extension = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<Extension>) : Extension = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<Extension>) : Extension = nativeOnly

type Names =
    U2<Names.U2.Case1, ResizeArray<string>>

module Extension =

    module Cases =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case1 =
            abstract member extension: Extension with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (extension: Extension) : Case1 = nativeOnly

module Names =

    module U2 =

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
