---
title: A package
---

Name an installed package and Glutinum generates it with every package its declarations depend on, as modules of one file.

```bash frame="terminal"
npm install leaflet @types/leaflet
npx @glutinum/cli @types/leaflet --out-file Glutinum.Leaflet.fs
```

## Inputs

An input is one of:

- an installed package, `chalk` or `@types/vscode`
- a package directory, `./node_modules/chalk`
- a subpath of a package, `date-fns/locale`

Generate several packages in the same file. 

```bash frame="terminal"
npx @glutinum/cli vscode vscode-languageclient --out-file Glutinum.Vscode.fs
```

Generates every package installed in the nearest `node_modules`.

```bash frame="terminal"
npx @glutinum/cli --all --out-file Glutinum.fs
```

A package is looked up from the current directory, so run the command where the `node_modules` is.

## The generated modules

Each package is a module named after it: `date-fns` gives `DateFns`, `@types/leaflet` gives `Leaflet`, `@codemirror/state` gives `CodemirrorState`. The functions and values of a package are the static members of its `Exports` type.

```fsharp
open Glutinum.Leaflet

let map = L.map "map"
```

## The DOM, Node and ES types

A package that uses the DOM or the Node types references them from the `Glutinum.Web` and `Glutinum.Node` packages instead of generating them again. The ES library types come from `Glutinum.Types` the same way. The header of the generated file says which ones to add.

`--no-externals` generates the DOM and Node types inline instead. The ES types always come from `Glutinum.Types`.

## Other bindings as dependencies

When a dependency is published as its own binding, reference it instead of generating a copy:

```bash frame="terminal"
npx @glutinum/cli @types/leaflet --external @types/geojson --out-file Glutinum.Leaflet.fs
```

The output refers to `Glutinum.Geojson.GeometryObject` and the project needs a reference to `Glutinum.Geojson`.

The module name is derived from the package name: `@types/geojson` gives `Geojson`, `signature_pad` gives `SignaturePad`, `chart.js` gives `ChartJs`. `<package>=<Module>` sets it when the binding uses another name.

```bash frame="terminal"
npx @glutinum/cli @types/leaflet --external @types/geojson=GeoJson --out-file Glutinum.Leaflet.fs
```

The output then refers to `Glutinum.GeoJson.GeometryObject`.
