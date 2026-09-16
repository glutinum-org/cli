module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("DiffieHellman", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member DiffieHellman () : DiffieHellman = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type DiffieHellman =
    abstract member setPublicKey: publicKey: string -> unit
    abstract member setPublicKey: publicKey: string * encoding: string -> unit
    abstract member setPrivateKey: privateKey: string -> unit
    abstract member getPrime: unit -> string

[<AllowNullLiteral>]
[<Interface>]
type DiffieHellmanGroup =
    abstract member getPrime: unit -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
