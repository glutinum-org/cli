---
title: NuGet packages
order: 10
---

The packages a generated binding depends on. The header of a generated file names the ones it needs.

## `Glutinum.Types`

The ES library types the generator refers to that `Fable.Core` does not define: `ReadonlyArray<'T>`, `ArrayLike<'T>`, `PromiseLike<'T>`, `Iterator<'T>`, `Generator<'T>`, `ReadonlyMap<'K, 'V>` and their neighbours. It is generated from the `lib.es*.d.ts` files of TypeScript. `Array`, `Date`, `Promise`, `Map`, `Set` and the typed arrays are the ones of `Fable.Core`. Every binding needs it.

A `ReadonlyArray<'T>` is a sequence, `Seq.map` and `for` read it, and `.[i]` indexes it.

The static side of the globals is on `Exports`: `Date` with its constructor, and `Number`, `String`, `Object`, `Symbol` and `Reflect` with every member the library declares. A number stays a `float`, a string a `string`, an object an `obj`. `JSON`, `Math`, `Promise`, `Map` and `Set` are the ones of `Fable.Core`.

```fsharp
open type Glutinum.Types.TypeScript.Exports

let today = Date.Create(2026, 8, 17)
let stamp = Date.now ()
let whole = Number.isInteger 3.0
let text = String.fromCharCode (72.0, 105.0)
let keys = Object.keys {| a = 1 |}
```

```bash frame="terminal"
dotnet add package Glutinum.Types
```

## `Glutinum.Web`

The DOM, generated from `@types/web`. A binding referring to `HTMLElement`, `Event`, `fetch` or any browser API references this package.

```fsharp
open type Glutinum.Web.Exports

let button = document.querySelector "button"
```

## `Glutinum.Node`

The Node.js API, generated from `@types/node`. The modules are under `Glutinum.Node`, their functions on their `Exports` type.

```fsharp
open Glutinum.Node

fs.Exports.readFileSync ("file.txt", "utf8")
```

## Bindings of libraries

Bindings of JavaScript libraries are published as `Glutinum.<Library>` packages. The [bindings](../bindings/index.md) page lists them.
