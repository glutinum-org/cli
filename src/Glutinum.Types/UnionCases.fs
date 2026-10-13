namespace Glutinum.Types

open Fable.Core

[<AutoOpen>]
module UnionCases =

    type U2<'T1, 'T2> with

        /// Unchecked, the value as <c>Case1</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase1: 'T1 = nativeOnly

        /// Unchecked, the value as <c>Case2</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase2: 'T2 = nativeOnly

    type U3<'T1, 'T2, 'T3> with

        /// Unchecked, the value as <c>Case1</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase1: 'T1 = nativeOnly

        /// Unchecked, the value as <c>Case2</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase2: 'T2 = nativeOnly

        /// Unchecked, the value as <c>Case3</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase3: 'T3 = nativeOnly

    type U4<'T1, 'T2, 'T3, 'T4> with

        /// Unchecked, the value as <c>Case1</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase1: 'T1 = nativeOnly

        /// Unchecked, the value as <c>Case2</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase2: 'T2 = nativeOnly

        /// Unchecked, the value as <c>Case3</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase3: 'T3 = nativeOnly

        /// Unchecked, the value as <c>Case4</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase4: 'T4 = nativeOnly

    type U5<'T1, 'T2, 'T3, 'T4, 'T5> with

        /// Unchecked, the value as <c>Case1</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase1: 'T1 = nativeOnly

        /// Unchecked, the value as <c>Case2</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase2: 'T2 = nativeOnly

        /// Unchecked, the value as <c>Case3</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase3: 'T3 = nativeOnly

        /// Unchecked, the value as <c>Case4</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase4: 'T4 = nativeOnly

        /// Unchecked, the value as <c>Case5</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase5: 'T5 = nativeOnly

    type U6<'T1, 'T2, 'T3, 'T4, 'T5, 'T6> with

        /// Unchecked, the value as <c>Case1</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase1: 'T1 = nativeOnly

        /// Unchecked, the value as <c>Case2</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase2: 'T2 = nativeOnly

        /// Unchecked, the value as <c>Case3</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase3: 'T3 = nativeOnly

        /// Unchecked, the value as <c>Case4</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase4: 'T4 = nativeOnly

        /// Unchecked, the value as <c>Case5</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase5: 'T5 = nativeOnly

        /// Unchecked, the value as <c>Case6</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase6: 'T6 = nativeOnly

    type U7<'T1, 'T2, 'T3, 'T4, 'T5, 'T6, 'T7> with

        /// Unchecked, the value as <c>Case1</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase1: 'T1 = nativeOnly

        /// Unchecked, the value as <c>Case2</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase2: 'T2 = nativeOnly

        /// Unchecked, the value as <c>Case3</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase3: 'T3 = nativeOnly

        /// Unchecked, the value as <c>Case4</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase4: 'T4 = nativeOnly

        /// Unchecked, the value as <c>Case5</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase5: 'T5 = nativeOnly

        /// Unchecked, the value as <c>Case6</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase6: 'T6 = nativeOnly

        /// Unchecked, the value as <c>Case7</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase7: 'T7 = nativeOnly

    type U8<'T1, 'T2, 'T3, 'T4, 'T5, 'T6, 'T7, 'T8> with

        /// Unchecked, the value as <c>Case1</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase1: 'T1 = nativeOnly

        /// Unchecked, the value as <c>Case2</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase2: 'T2 = nativeOnly

        /// Unchecked, the value as <c>Case3</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase3: 'T3 = nativeOnly

        /// Unchecked, the value as <c>Case4</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase4: 'T4 = nativeOnly

        /// Unchecked, the value as <c>Case5</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase5: 'T5 = nativeOnly

        /// Unchecked, the value as <c>Case6</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase6: 'T6 = nativeOnly

        /// Unchecked, the value as <c>Case7</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase7: 'T7 = nativeOnly

        /// Unchecked, the value as <c>Case8</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase8: 'T8 = nativeOnly

    type U9<'T1, 'T2, 'T3, 'T4, 'T5, 'T6, 'T7, 'T8, 'T9> with

        /// Unchecked, the value as <c>Case1</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase1: 'T1 = nativeOnly

        /// Unchecked, the value as <c>Case2</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase2: 'T2 = nativeOnly

        /// Unchecked, the value as <c>Case3</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase3: 'T3 = nativeOnly

        /// Unchecked, the value as <c>Case4</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase4: 'T4 = nativeOnly

        /// Unchecked, the value as <c>Case5</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase5: 'T5 = nativeOnly

        /// Unchecked, the value as <c>Case6</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase6: 'T6 = nativeOnly

        /// Unchecked, the value as <c>Case7</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase7: 'T7 = nativeOnly

        /// Unchecked, the value as <c>Case8</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase8: 'T8 = nativeOnly

        /// Unchecked, the value as <c>Case9</c> whatever case it holds
        [<Emit("$0")>]
        member _.AsCase9: 'T9 = nativeOnly
