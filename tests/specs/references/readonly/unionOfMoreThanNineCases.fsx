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
    abstract member frames: ReadonlyArray<Stack.frames.Item> with get
    [<ParamObject; Emit("$0")>]
    static member Create (frames: ResizeArray<Stack.frames.Item>) : Stack = nativeOnly

module Stack =

    module frames =

        [<RequireQualifiedAccess>]
        [<Erase>]
        type Item =
            | Case1 of Stack.frames.Item.ReadOnlyA
            | Case2 of Stack.frames.Item.ReadOnlyB
            | Case3 of Stack.frames.Item.ReadOnlyC
            | Case4 of Stack.frames.Item.ReadOnlyD
            | Case5 of Stack.frames.Item.ReadOnlyE
            | Case6 of Stack.frames.Item.ReadOnlyF
            | Case7 of Stack.frames.Item.ReadOnlyG
            | Case8 of Stack.frames.Item.ReadOnlyH
            | Case9 of Stack.frames.Item.ReadOnlyI
            | Case10 of Stack.frames.Item.ReadOnlyJ

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyA) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyA) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyB) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyB) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyC) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyC) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyD) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyD) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyE) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyE) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyF) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyF) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyG) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyG) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyH) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyH) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyI) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyI) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_Implicit(value: Stack.frames.Item.ReadOnlyJ) : Item = nativeOnly

            [<Emit("$0")>]
            static member op_ErasedCast(value: Stack.frames.Item.ReadOnlyJ) : Item = nativeOnly

            /// Unchecked, the value as <c>Case1</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase1: Stack.frames.Item.ReadOnlyA = nativeOnly

            /// Unchecked, the value as <c>Case2</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase2: Stack.frames.Item.ReadOnlyB = nativeOnly

            /// Unchecked, the value as <c>Case3</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase3: Stack.frames.Item.ReadOnlyC = nativeOnly

            /// Unchecked, the value as <c>Case4</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase4: Stack.frames.Item.ReadOnlyD = nativeOnly

            /// Unchecked, the value as <c>Case5</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase5: Stack.frames.Item.ReadOnlyE = nativeOnly

            /// Unchecked, the value as <c>Case6</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase6: Stack.frames.Item.ReadOnlyF = nativeOnly

            /// Unchecked, the value as <c>Case7</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase7: Stack.frames.Item.ReadOnlyG = nativeOnly

            /// Unchecked, the value as <c>Case8</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase8: Stack.frames.Item.ReadOnlyH = nativeOnly

            /// Unchecked, the value as <c>Case9</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase9: Stack.frames.Item.ReadOnlyI = nativeOnly

            /// Unchecked, the value as <c>Case10</c> whatever case it holds
            [<Emit("$0")>]
            member _.AsCase10: Stack.frames.Item.ReadOnlyJ = nativeOnly

        module Item =

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
