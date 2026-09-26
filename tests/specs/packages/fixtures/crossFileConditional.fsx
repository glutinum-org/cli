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

        [<AllowNullLiteral>]
        [<Interface>]
        type ResolveBody<'S> =
            interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
