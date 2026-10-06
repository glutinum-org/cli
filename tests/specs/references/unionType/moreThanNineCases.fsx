module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (value: Exports.f__.value option) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type A =
    abstract member a: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (a: string) : A = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type B =
    abstract member b: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (b: string) : B = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type C =
    abstract member c: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (c: string) : C = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type D =
    abstract member d: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (d: string) : D = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type E =
    abstract member e: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (e: string) : E = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type F =
    abstract member f: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (f: string) : F = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type G =
    abstract member g: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (g: string) : G = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type H =
    abstract member h: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (h: string) : H = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type I =
    abstract member i: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (i: string) : I = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type J =
    abstract member j: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (j: string) : J = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type Big =
    | Case1 of A
    | Case2 of B
    | Case3 of C
    | Case4 of D
    | Case5 of E
    | Case6 of F
    | Case7 of G
    | Case8 of H
    | Case9 of I
    | Case10 of J

    [<Emit("$0")>]
    static member op_Implicit(value: A) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: A) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: B) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: B) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: C) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: C) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: D) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: D) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: E) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: E) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: F) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: F) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: G) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: G) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: H) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: H) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: I) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: I) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: J) : Big = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: J) : Big = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Holder =
    abstract member value: Holder.value with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: Holder.value) : Holder = nativeOnly

module Holder =

    [<RequireQualifiedAccess>]
    [<Erase>]
    type value =
        | Case1 of A
        | Case2 of B
        | Case3 of C
        | Case4 of D
        | Case5 of E
        | Case6 of F
        | Case7 of G
        | Case8 of H
        | Case9 of I
        | Case10 of J

        [<Emit("$0")>]
        static member op_Implicit(value: A) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: A) : value = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: B) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: B) : value = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: C) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: C) : value = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: D) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: D) : value = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: E) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: E) : value = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: F) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: F) : value = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: G) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: G) : value = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: H) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: H) : value = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: I) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: I) : value = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: J) : value = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: J) : value = nativeOnly

module Exports =

    module f__ =

        [<RequireQualifiedAccess>]
        [<Erase>]
        type value =
            | Case1 of A
            | Case2 of B
            | Case3 of C
            | Case4 of D
            | Case5 of E
            | Case6 of F
            | Case7 of G
            | Case8 of H
            | Case9 of I
            | Case10 of J

            [<Emit("$0")>]
            static member op_Implicit(value: A) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: A) : value = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: B) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: B) : value = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: C) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: C) : value = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: D) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: D) : value = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: E) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: E) : value = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: F) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: F) : value = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: G) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: G) : value = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: H) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: H) : value = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: I) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: I) : value = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: J) : value = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: J) : value = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
