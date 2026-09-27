module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline ts
        with get () : ts_.Exports =
            nativeOnly

module ts_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.server")>]
        abstract member server: server.Exports with get

    module server =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Emit("$0.tryConvertScriptKindName($1...)")>]
            abstract member tryConvertScriptKindName: scriptKindName: ScriptKindName -> ScriptKind
            [<Emit("$0.tryConvertScriptKindName($1...)")>]
            abstract member tryConvertScriptKindName: scriptKindName: ScriptKind -> ScriptKind
            [<Emit("$0.convertScriptKindName($1...)")>]
            abstract member convertScriptKindName: scriptKindName: ts_.server.protocol.ScriptKindName -> ScriptKind

        module protocol =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type ScriptKindName =
                | TS
                | JS

    [<RequireQualifiedAccess>]
    type ScriptKind =
        | JS = 1
        | TS = 3

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
