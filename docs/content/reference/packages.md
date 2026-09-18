---
title: NuGet packages
order: 3
---

The packages a generated binding depends on. The header of a generated file names the ones it needs.

## `Glutinum.Types`

The types the generator refers to that `Fable.Core` does not have: `ReadonlyArray<'T>`, `Iterable<'T>`, `AsyncIterable<'T>`, `ArrayLike<'T>`, `PromiseLike<'T>` and the other TypeScript library types. Every binding needs it.

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
