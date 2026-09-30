module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("format", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member format (date: Date, formatStr: string) : string = nativeOnly
    [<Import("format", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member format (date: float, formatStr: string) : string = nativeOnly
    [<Import("format", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member format (date: string, formatStr: string) : string = nativeOnly
    [<Import("format", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member format (date: DateArg, formatStr: string) : string = nativeOnly
    [<Import("run", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member run<'T> (value: 'T, options: Options) : unit = nativeOnly

type DateArg =
    U3<Date, float, string>

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member value: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: string) : Options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
