module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Type2", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Type2<'A> () : Type2<'A> = nativeOnly
    [<Import("Type1", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Type1<'A when 'A :> Task> () : Type1<'A> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Type2<'A> =
    interface end

type Type2 =
    Type2<string>

[<AllowNullLiteral>]
[<Interface>]
type Task =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Type1<'A when 'A :> Task> =
    interface end

type Type1 =
    Type1<Task>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
