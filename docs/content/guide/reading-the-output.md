---
title: Reading the output
order: 5
---

A generated file is a `module rec` holding one type per TypeScript declaration. This page shows the shapes you will meet. The [mapping reference](../reference/mapping.md) lists every construct.

## Functions and constructors

The values a package exports are static members of its `Exports` type. A function is imported by name, a class gets a constructor member marked `EmitConstructor`.

```fsharp
[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("format", "date-fns")>]
    static member format (date: Date, formatStr: string) : string = nativeOnly

    [<Import("Map", "leaflet"); EmitConstructor>]
    static member Map (element: string, ?options: MapOptions) : Map = nativeOnly
```

Open the type to call them without the prefix:

```fsharp
open type Glutinum.DateFns.Exports

format (today, "yyyy-MM-dd")
```

## Interfaces and classes

Both become an F# interface with abstract members. A property is `with get, set`, a method is a member with named parameters.

```fsharp
[<AllowNullLiteral>]
[<Interface>]
type Map =
    abstract member setView: center: LatLng * zoom: float -> Map
    abstract member zoom: float with get, set
```

The static members of a class are `static member inline` on the interface, emitted as JavaScript.

## Optional members and parameters

An optional property is an `option`. An optional parameter takes `?`:

```fsharp
abstract member timeout: float option with get, set
abstract member connect: host: string * ?port: float -> unit
```

## Unions

A union of string literals is a string enum you match on:

```fsharp
[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Position =
    | top
    | bottom
```

Other unions are `U2`, `U3` and so on, from `Fable.Core`. Pass a value with `U2.Case1`, read one with a match:

```fsharp
element.doc <- Some(U2.Case1 "text")

match request.params.["id"] with
| U2.Case1 single -> single
| U2.Case2 several -> String.concat "," several
```

An overloaded parameter is generated as one overload per case, so a union in parameter position rarely needs `U2`.

## Objects you construct

A type literal used as a parameter, `options: { radius?: number }`, becomes a class with a `ParamObject` constructor. Named arguments build the object:

```fsharp
L.circleMarker (paris, CircleMarkerOptions(radius = 12))
```

An interface is built with `jsOptions`:

```fsharp
jsOptions<MapOptions> (fun options -> options.zoom <- Some 13)
```

## Callbacks

A function type with one parameter is an F# lambda. With several it is a delegate, and F# converts a lambda to it at the call site:

```fsharp
app.get ("/users", RequestHandler(fun request response -> response.json users))
```

## Arrays and tuples

An array is a `ResizeArray`. A `readonly` array in a return type is `ReadonlyArray` from `Glutinum.Types`, and a `ResizeArray` when it is a parameter. A tuple is an F# tuple.

## Nested types

A type declared inside another, such as the type of an option or an event map, lives in a module named after the parent, `Map.Options`, `Editor.on`.

## Namespaces

A namespace is a module. Its functions are on the `Exports` type of the module, its types beside it.

```fsharp
Node.fs.Exports.readFileSync ("file.txt", "utf8")
```

## Enums

A numeric enum is an F# enum. A string enum is a string enum. An enum mixing both is an erased union, its members are constants:

```fsharp
[<RequireQualifiedAccess>]
[<Erase>]
type Level =
    | String of string
    | Number of float
    static member inline Verbose: Level = Level.String "verbose"
    static member inline Error: Level = Level.Number 2.0
```
