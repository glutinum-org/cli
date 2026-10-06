namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module CrossFileConditional =

    [<AllowNullLiteral>]
    [<Interface>]
    type Request<'S> =
        abstract member body: obj with get, set

    module resolve =

        [<AllowNullLiteral>]
        [<Interface>]
        type Schema =
            abstract member body: obj with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (body: obj) : Schema = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type ResolveBody<'S> =
            interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
