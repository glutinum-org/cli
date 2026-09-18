---
title: Contributing a binding
order: 2
---

A binding lives in the [hub](https://github.com/glutinum-org/hub) repository, in `bindings/Glutinum.<Module>/`, with its tests.

## Scaffold it

```bash frame="terminal"
./build.sh new date-fns
./build.sh new @types/leaflet --external Glutinum.Geojson
./build.sh new chart.js --tests browser
```

The command pins the npm package, generates the binding, writes the project and scaffolds the tests. `--external` references another binding for a shared dependency, `--tests browser` runs the tests in Chromium instead of Node.

## Test it

```bash frame="terminal"
./build.sh test Glutinum.DateFns
```

Tests are [Scriptorium](https://github.com/glutinum-org/Scriptorium) tests in `tests/Main.fs`. Cover what a user does first: construct the main object, call the common functions, read the results back.

## Add helpers

A file `Glutinum.<Module>.Extensions.fs` beside the generated one is compiled after it. Put the adapters and extension members that make the API comfortable from F# there, see [extending a binding](../guide/extending.md).

## Keep it current

```bash frame="terminal"
./build.sh upgrade Glutinum.DateFns
./build.sh generate --changed
```

A new version of Glutinum regenerates every binding. The tests say what changed.
