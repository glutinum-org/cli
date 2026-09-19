---
title: Bindings
---

Bindings generated with Glutinum and published on NuGet, with tests that run them in Node or in a browser.

## Runtimes

Generated from this repository.

| Package | Generated from | Usage |
| --- | --- | --- |
| `Glutinum.Web` | `@types/web` | `open type Glutinum.Web.Exports` |
| `Glutinum.Node` | `@types/node` | `open Glutinum.Node` |

## Libraries

Generated from the [hub](https://github.com/glutinum-org/hub) repository, one folder per binding with its tests.

| Package | Generated from | Tests |
| --- | --- | --- |
| `Glutinum.Chalk` | `chalk` | Node |
| `Glutinum.ChartJs` | `chart.js` | Chromium |
| `Glutinum.Codemirror` | `codemirror` | Chromium |
| `Glutinum.DateFns` | `date-fns` | Node |
| `Glutinum.Dayjs` | `dayjs` | Node |
| `Glutinum.Express` | `@types/express` | Node |
| `Glutinum.Geojson` | `@types/geojson` | Node |
| `Glutinum.Jspdf` | `jspdf` | Chromium |
| `Glutinum.Leaflet` | `@types/leaflet` | Chromium |
| `Glutinum.Playwright` | `playwright` | Node |
| `Glutinum.SignaturePad` | `signature_pad` | Chromium |
| `Glutinum.Yargs` | `yargs` | Node |

Each binding pins the version of the npm package it was generated from. Its changelog says which one.

## Use one

```bash frame="terminal"
dotnet add package Glutinum.DateFns
npm install date-fns
```

The npm package is still needed, the binding only describes it.

```fsharp
open Glutinum.DateFns

let days = DateFns.eachDayOfInterval interval
```

A binding that depends on another one references it, `Glutinum.Leaflet` brings `Glutinum.Geojson`.

## Ask for one

Open an issue on the [hub](https://github.com/glutinum-org/hub/issues) with the npm package name, or [contribute it](contributing.md).
