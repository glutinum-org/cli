---
title: TypeScript to F#
toc:
  to: 3
---

What each TypeScript construct is generated as.

:::note
This table is maintained by hand and can fall behind the generator. When the output does not match it, trust the output and [report the difference](limitations.md).
:::

## Declarations

<table>
<thead>
<tr><th>TypeScript</th><th>F#</th></tr>
</thead>
<tbody>
<tr><td><code>function f(): void</code></td><td><code>static member f () : unit</code> on <code>Exports</code>, with <code>Import</code></td></tr>
<tr><td><code>const x: number</code></td><td><code>static member inline x: float</code> on <code>Exports</code></td></tr>
<tr><td><code>class C</code></td><td>interface <code>C</code>, and <code>static member C ()</code> on <code>Exports</code> with <code>EmitConstructor</code></td></tr>
<tr><td><code>static</code> member of a class</td><td><code>static member inline</code> on the interface, emitted as JavaScript</td></tr>
<tr><td><code>interface I</code></td><td><code>[&lt;AllowNullLiteral&gt;] [&lt;Interface&gt;] type I</code></td></tr>
<tr><td><code>type A = B</code></td><td><code>type A = B</code></td></tr>
<tr><td><code>namespace N</code></td><td><code>module N</code>, its values on <code>N.Exports</code></td></tr>
<tr><td><code>enum E { A = 1 }</code></td><td><code>type E = A = 1</code></td></tr>
<tr><td><code>enum E { A = "a" }</code></td><td><code>[&lt;StringEnum&gt;] type E = A</code></td></tr>
<tr><td><code>enum E { A = "a", B = 1 }</code></td><td><code>[&lt;Erase&gt;] type E = String of string | Number of float</code>, with a constant per member</td></tr>
<tr><td><code>export = f</code></td><td><code>Exports.f</code>, through the default import</td></tr>
<tr><td><code>export default C</code></td><td>the declaration, through the default import</td></tr>
<tr><td><code>export as namespace N</code></td><td>nothing</td></tr>
<tr><td><code>declare global</code></td><td>a <code>global_</code> module</td></tr>
</tbody>
</table>

## Members

<table>
<thead>
<tr><th>TypeScript</th><th>F#</th></tr>
</thead>
<tbody>
<tr><td><code>x: T</code></td><td><code>abstract member x: T with get, set</code></td></tr>
<tr><td><code>readonly x: T</code></td><td><code>abstract member x: T with get</code></td></tr>
<tr><td><code>x?: T</code></td><td><code>abstract member x: T option with get, set</code></td></tr>
<tr><td><code>m(a: A): R</code></td><td><code>abstract member m: a: A -> R</code></td></tr>
<tr><td><code>m(a?: A)</code></td><td><code>abstract member m: ?a: A -> R</code></td></tr>
<tr><td><code>m(...a: A[])</code></td><td><code>[&lt;ParamArray&gt;] a: A []</code></td></tr>
<tr><td><code>get x(): T</code> and <code>set x(v: T)</code></td><td>one property</td></tr>
<tr><td><code>[key: string]: T</code></td><td><code>[&lt;EmitIndexer&gt;] abstract member Item: key: string -> T</code></td></tr>
<tr><td><code>new (a: A): R</code></td><td><code>[&lt;EmitConstructor&gt;] abstract member Create: a: A -> R</code></td></tr>
<tr><td><code>(a: A): R</code> call signature</td><td><code>[&lt;Emit("$0($1...)")&gt;] abstract member Invoke: a: A -> R</code></td></tr>
<tr><td><code>m(this: T, a: A)</code></td><td><code>abstract member m: a: A -> R</code></td></tr>
<tr><td><code>m&lt;T&gt;(a: T): T</code></td><td><code>abstract member m&lt;'T&gt;: a: 'T -> 'T</code></td></tr>
<tr><td><code>m&lt;T = D&gt;()</code></td><td>the member, and an overload without <code>T</code> with <code>D</code> in its place</td></tr>
<tr><td>overloads</td><td>one member per overload</td></tr>
</tbody>
</table>

## Types

<table>
<thead>
<tr><th>TypeScript</th><th>F#</th></tr>
</thead>
<tbody>
<tr><td><code>string</code>, <code>number</code>, <code>boolean</code></td><td><code>string</code>, <code>float</code>, <code>bool</code></td></tr>
<tr><td><code>bigint</code></td><td><code>bigint</code></td></tr>
<tr><td><code>void</code>, <code>undefined</code></td><td><code>unit</code>, <code>obj</code></td></tr>
<tr><td><code>any</code>, <code>unknown</code>, <code>never</code>, <code>symbol</code></td><td><code>obj</code></td></tr>
<tr><td><code>T[]</code>, <code>Array&lt;T&gt;</code></td><td><code>ResizeArray&lt;T&gt;</code></td></tr>
<tr><td><code>readonly T[]</code></td><td><code>ReadonlyArray&lt;T&gt;</code>, <code>ResizeArray&lt;T&gt;</code> as a parameter</td></tr>
<tr><td><code>[A, B]</code></td><td><code>A * B</code></td></tr>
<tr><td><code>[A, ...B[]]</code></td><td><code>ResizeArray&lt;obj&gt;</code></td></tr>
<tr><td><code>A | B</code> in a parameter</td><td>one overload per case, up to 16 combinations</td></tr>
<tr><td><code>A | B</code> elsewhere</td><td><code>U2&lt;A, B&gt;</code>, up to <code>U9</code></td></tr>
<tr><td>more than nine cases</td><td><code>obj</code></td></tr>
<tr>
<td><code>"a" | B</code>, literals and types</td>
<td>

an erased union, its literals named and its types numbered

```fsharp
[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type T =
    | a
    | Case1 of B
```

</td>
</tr>
<tr><td><code>"a" | "b"</code></td><td>a <code>StringEnum</code> type</td></tr>
<tr><td><code>1 | 2</code></td><td>an enum type</td></tr>
<tr><td><code>true | false</code></td><td><code>bool</code></td></tr>
<tr><td><code>T | undefined</code>, <code>T | null</code></td><td><code>T option</code></td></tr>
<tr><td><code>(a: A) => R</code></td><td><code>A -> R</code></td></tr>
<tr><td><code>(a: A, b: B) => R</code></td><td>a <code>delegate of a: A * b: B -> R</code></td></tr>
<tr><td><code>{ x: T }</code> in a parameter</td><td>a class with a <code>ParamObject</code> constructor</td></tr>
<tr><td><code>{ x: T }</code> elsewhere</td><td>an interface in the module of the parent</td></tr>
<tr><td><code>Date</code></td><td><code>Date</code> of <code>Glutinum.Types</code>, built with <code>Date.Create</code></td></tr>
<tr><td><code>Promise&lt;T&gt;</code>, <code>RegExp</code></td><td><code>JS.Promise&lt;T&gt;</code>, <code>RegExp</code></td></tr>
<tr><td><code>Map&lt;K, V&gt;</code>, <code>Set&lt;T&gt;</code></td><td><code>Map&lt;K, V&gt;</code>, <code>Set&lt;T&gt;</code> from <code>Fable.Core.JS</code></td></tr>
<tr><td><code>HTMLElement</code> and the DOM</td><td><code>Glutinum.Web.HTMLElement</code></td></tr>
<tr><td><code>Buffer</code> and the Node API</td><td><code>Glutinum.Node.Buffer</code></td></tr>
<tr><td><code>typeof x</code></td><td>the type of <code>x</code></td></tr>
<tr><td><code>keyof T</code>, <code>T</code> known</td><td>a <code>StringEnum</code> of its keys</td></tr>
<tr><td><code>T[K]</code>, <code>T</code> known</td><td>the type of the member</td></tr>
<tr><td><code>`on${K}`</code></td><td>a <code>StringEnum</code> when it enumerates</td></tr>
<tr><td><code>x is T</code></td><td><code>bool</code></td></tr>
<tr><td><code>asserts x is T</code></td><td><code>unit</code></td></tr>
<tr><td><code>T extends U ? A : B</code></td><td><code>A</code> or <code>B</code> when decided, <code>obj</code> otherwise</td></tr>
<tr><td><code>Partial&lt;T&gt;</code></td><td>the members of <code>T</code>, optional</td></tr>
<tr><td><code>Required&lt;T&gt;</code>, <code>Pick&lt;T, K&gt;</code>, <code>Omit&lt;T, K&gt;</code></td><td>the members it keeps</td></tr>
<tr><td><code>Record&lt;K, V&gt;</code></td><td>an interface with an indexer</td></tr>
<tr><td><code>Readonly&lt;T&gt;</code></td><td>the members of <code>T</code>, read only</td></tr>
<tr><td><code>ReturnType&lt;F&gt;</code>, <code>Parameters&lt;F&gt;</code>, <code>Awaited&lt;T&gt;</code></td><td>the resolved type</td></tr>
<tr><td><code>Exclude&lt;T, undefined&gt;</code></td><td><code>T</code></td></tr>
<tr><td>a mapped type over a type parameter</td><td><code>obj</code></td></tr>
<tr><td>a generic type parameter <code>T extends C</code></td><td><code>'T</code></td></tr>
</tbody>
</table>
