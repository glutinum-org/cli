---
title: A package
order: 4
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

Several inputs are generated in the same file. `--all` generates every package installed in the nearest `node_modules`.

```bash frame="terminal"
npx @glutinum/cli vscode vscode-languageclient --out-file Glutinum.Vscode.fs
npx @glutinum/cli --all --out-file Glutinum.fs
```

A package is looked up from the current directory, so run the command where the `node_modules` is.

## The generated modules

Each package is a module named after it: `date-fns` gives `DateFns`, `@types/leaflet` gives `Leaflet`, `@codemirror/state` gives `CodemirrorState`. The functions and values of a package are the static members of its `Exports` type.

```fsharp
open Glutinum.Leaflet

let map = L.map "map"
```

## The DOM and Node types

A package that uses the DOM or the Node types references them from the `Glutinum.Web` and `Glutinum.Node` packages instead of generating them again. The header of the generated file says which one to add.

`--no-externals` generates them inline instead.

## Other bindings as dependencies

When a dependency is published as its own binding, reference it instead of generating a copy:

```bash frame="terminal"
npx @glutinum/cli @types/leaflet --external @types/geojson --out-file Glutinum.Leaflet.fs
```

The output refers to `Glutinum.Geojson.GeometryObject` and the project needs a reference to `Glutinum.Geojson`. The module name is derived from the package name, `<package>=<Module>` sets it.
