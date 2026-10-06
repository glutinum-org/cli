module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Story =
    abstract member id: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (id: string) : Story = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Args =
    abstract member mount: Args.mount<obj> with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (mount: Args.mount<obj>) : Args = nativeOnly

module Args =

    type mount<'Component> =
        delegate of id: string * ?props: 'Component * ?options: Story -> JS.Promise<Story>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
