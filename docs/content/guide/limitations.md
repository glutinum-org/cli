---
title: Limitations
order: 6
---

Some TypeScript types have no F# equivalent. Glutinum generates `obj` for them rather than failing, and this page lists what they are and what to do.

## What becomes `obj`

### Conditional types the arguments do not decide

`T extends string ? A : B` is resolved when `T` is known, when its default decides it, or when both branches are the same type. A conditional whose result depends on a type parameter of the caller is `obj`.

### Mapped types over a type parameter

`Partial<T>`, `Omit<T, K>`, `Pick<T, K>` and `Record<K, V>` are expanded when their argument is a known type. Applied to a type parameter they are `obj`, and so is a recursive mapped type such as `DeepPartial<T>`.

### Indexed access on a type parameter

`T[K]` and `keyof T` are `obj` unless `T` is known.

### `any`, `unknown`, `symbol`, `never`

All four are `obj`.

## What is dropped

### Constraints on type parameters

`<T extends Node>` is generated as `<'T>`. F# has no way to express the constraint on an interface.

### Partially generic overloads

A method with a defaulted type parameter, `get<T = Element>(selector: string): T`, gets a non-generic overload with the default applied. A method whose type parameters are only partly defaulted gets no such overload, because F# cannot choose between two generic overloads when an optional argument is omitted.

### `this` parameters

`function f(this: Window)` is generated without the parameter, JavaScript does not pass it.

## Escape hatches

`unbox` gives a value the type you know it has:

```fsharp
let days: ResizeArray<Date> = unbox (DateFns.eachDayOfInterval interval)
```

`!!` does the same in an expression, and `createObj` builds an object a `ParamObject` class cannot:

```fsharp
let feature = createObj [ "type" ==> "Feature"; "properties" ==> createObj [] ]
```

When an API is awkward from F#, [extend the binding](extending.md) with a hand-written helper rather than a cast at every call site.

## Report what is wrong

A construct that is generated wrongly, rather than as `obj`, is a bug. Reduce it to a few lines in the [web application](try-it-online.md) and open an issue from there.
