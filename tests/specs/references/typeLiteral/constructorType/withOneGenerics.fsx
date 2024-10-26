module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Pool", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Pool<'T> () : Pool<'T> = nativeOnly

[<Erase>]
type ClientLikeCtr<'T> =
    | ClientLikeCtr of 'T

    member inline this.Value =
        let (ClientLikeCtr output) = this
        output

[<AllowNullLiteral>]
[<Interface>]
type Pool<'T> =
    abstract member Client: ClientLikeCtr<'T> with get, set

(***)
#r "nuget: Fable.Core"
(***)
