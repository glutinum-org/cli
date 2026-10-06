module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member colorMode: Options.colorMode option with get, set
    abstract member mode: Options.mode option with get, set
    abstract member plain: U2<string, bool> option with get, set
    abstract member withNumber: Options.withNumber option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?colorMode: Options.colorMode, ?mode: Options.mode, ?withNumber: Options.withNumber) : Options = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (plain: string, ?colorMode: Options.colorMode, ?mode: Options.mode, ?withNumber: Options.withNumber) : Options = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (plain: bool, ?colorMode: Options.colorMode, ?mode: Options.mode, ?withNumber: Options.withNumber) : Options = nativeOnly

module Options =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type colorMode =
        | [<CompiledValue(true)>] True
        | [<CompiledValue(false)>] False
        | auto

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type mode =
        | strip
        | transform
        | [<CompiledValue(false)>] False

    [<RequireQualifiedAccess>]
    [<Erase(CaseRules.None)>]
    type withNumber =
        | IPv4
        | IPv6
        | Case1 of float

        [<Emit("$0")>]
        static member op_Implicit(value: float) : withNumber = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: float) : withNumber = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
