---
title: TypeScript to F#
order: 2
toc:
  to: 3
---

What each TypeScript construct is generated as.

## Declarations

| TypeScript | F# |
| --- | --- |
| `function f(): void` | `static member f () : unit` on `Exports`, with `Import` |
| `const x: number` | `static member inline x: float` on `Exports` |
| `class C` | interface `C`, and `static member C ()` on `Exports` with `EmitConstructor` |
| `static` member of a class | `static member inline` on the interface, emitted as JavaScript |
| `interface I` | `[<AllowNullLiteral>] [<Interface>] type I` |
| `type A = B` | `type A = B` |
| `namespace N` | `module N`, its values on `N.Exports` |
| `enum E { A = 1 }` | `type E = A = 1` |
| `enum E { A = "a" }` | `[<StringEnum>] type E = A` |
| `enum E { A = "a", B = 1 }` | `[<Erase>] type E = String of string \| Number of float`, with a constant per member |
| `export = f` | `Exports.f`, through the default import |
| `export default C` | the declaration, through the default import |
| `export as namespace N` | nothing |
| `declare global` | a `global_` module |

## Members

| TypeScript | F# |
| --- | --- |
| `x: T` | `abstract member x: T with get, set` |
| `readonly x: T` | `abstract member x: T with get` |
| `x?: T` | `abstract member x: T option with get, set` |
| `m(a: A): R` | `abstract member m: a: A -> R` |
| `m(a?: A)` | `abstract member m: ?a: A -> R` |
| `m(...a: A[])` | `[<ParamArray>] a: A []` |
| `get x(): T` and `set x(v: T)` | one property |
| `[key: string]: T` | `[<EmitIndexer>] abstract member Item: key: string -> T` |
| `new (a: A): R` | `[<EmitConstructor>] abstract member Create: a: A -> R` |
| `(a: A): R` call signature | `[<Emit("$0($1...)")>] abstract member Invoke: a: A -> R` |
| `m(this: T, a: A)` | `abstract member m: a: A -> R` |
| `m<T = D>()` | the member, and an overload without `T` with `D` in its place |
| overloads | one member per overload |

## Types

| TypeScript | F# |
| --- | --- |
| `string`, `number`, `boolean` | `string`, `float`, `bool` |
| `bigint` | `bigint` |
| `void`, `undefined` | `unit`, `obj` |
| `any`, `unknown`, `never`, `symbol` | `obj` |
| `T[]`, `Array<T>` | `ResizeArray<T>` |
| `readonly T[]` | `ReadonlyArray<T>`, `ResizeArray<T>` as a parameter |
| `[A, B]` | `A * B` |
| `[A, ...B[]]` | `ResizeArray<obj>` |
| `A \| B` | `U2<A, B>` |
| `"a" \| "b"` | a `StringEnum` type |
| `1 \| 2` | an enum type |
| `T \| undefined`, `T \| null` | `T option` |
| `(a: A) => R` | `A -> R` |
| `(a: A, b: B) => R` | a `delegate of a: A * b: B -> R` |
| `{ x: T }` in a parameter | a class with a `ParamObject` constructor |
| `{ x: T }` elsewhere | an interface in the module of the parent |
| `Date` | `Date` of `Glutinum.Types`, built with `Date.Create` |
| `Promise<T>`, `RegExp` | `JS.Promise<T>`, `RegExp` |
| `Map<K, V>`, `Set<T>` | `Map<K, V>`, `Set<T>` from `Fable.Core.JS` |
| `HTMLElement` and the DOM | `Glutinum.Web.HTMLElement` |
| `Buffer` and the Node API | `Glutinum.Node.Buffer` |
| `typeof x` | the type of `x` |
| `keyof T`, `T` known | a `StringEnum` of its keys |
| `T[K]`, `T` known | the type of the member |
| `` `on${K}` `` | a `StringEnum` when it enumerates |
| `x is T` | `bool` |
| `asserts x is T` | `unit` |
| `T extends U ? A : B` | `A` or `B` when decided, `obj` otherwise |
| `Partial<T>` | the members of `T`, optional |
| `Required<T>`, `Pick<T, K>`, `Omit<T, K>` | the members it keeps |
| `Record<K, V>` | an interface with an indexer |
| `Readonly<T>` | the members of `T`, read only |
| `ReturnType<F>`, `Parameters<F>`, `Awaited<T>` | the resolved type |
| `Exclude<T, undefined>` | `T` |
| a mapped type over a type parameter | `obj` |
| a generic type parameter `T extends C` | `'T` |
