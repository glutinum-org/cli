module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

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

[<AllowNullLiteral>]
[<Interface>]
type Stack =
    abstract member frames: ReadonlyArray<Stack.frames> with get
    [<ParamObject; Emit("$0")>]
    static member Create (frames: ReadonlyArray<Stack.frames>) : Stack = nativeOnly

module Stack =

    [<RequireQualifiedAccess>]
    [<Erase>]
    type frames =
        | Case1 of Stack.frames.ReadOnlyA
        | Case2 of Stack.frames.ReadOnlyB
        | Case3 of Stack.frames.ReadOnlyC
        | Case4 of Stack.frames.ReadOnlyD
        | Case5 of Stack.frames.ReadOnlyE
        | Case6 of Stack.frames.ReadOnlyF
        | Case7 of Stack.frames.ReadOnlyG
        | Case8 of Stack.frames.ReadOnlyH
        | Case9 of Stack.frames.ReadOnlyI
        | Case10 of Stack.frames.ReadOnlyJ

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyA) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyA) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyB) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyB) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyC) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyC) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyD) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyD) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyE) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyE) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyF) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyF) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyG) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyG) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyH) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyH) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyI) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyI) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_Implicit(value: Stack.frames.ReadOnlyJ) : frames = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: Stack.frames.ReadOnlyJ) : frames = nativeOnly

    module frames =

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyA =
            abstract member a: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyB =
            abstract member b: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyC =
            abstract member c: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyD =
            abstract member d: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyE =
            abstract member e: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyF =
            abstract member f: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyG =
            abstract member g: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyH =
            abstract member h: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyI =
            abstract member i: string with get

        [<AllowNullLiteral>]
        [<Interface>]
        type ReadOnlyJ =
            abstract member j: string with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
