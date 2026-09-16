module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("LinkedMap", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member LinkedMap<'K, 'V> () : LinkedMap<'K, 'V> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type LinkedMap<'K, 'V> =
    abstract member forEach: callbackfn: LinkedMap.forEach.callbackfn<'K, 'V> * ?thisArg: obj -> unit

module LinkedMap =

    module forEach =

        type callbackfn<'K, 'V> =
            delegate of value: 'V * key: 'K * map: LinkedMap<'K, 'V> -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
