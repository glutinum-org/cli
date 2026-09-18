---
title: Command line
order: 1
---

The generator is the npm package `@glutinum/cli`. Run it with `npx`, or install it and run `glue`.

```bash frame="terminal"
npx @glutinum/cli <input>... [--out-file <output>]
npx @glutinum/cli --all [--out-file <output>]
```

## Inputs

An input is one of:

| Input | Example |
| --- | --- |
| An installed package | `chalk`, `@types/vscode` |
| A subpath of a package | `date-fns/locale` |
| A package directory | `./node_modules/chalk` |
| A `.d.ts` file | `./node_modules/chalk/source/index.d.ts` |

Several packages are generated in one file, each with the packages it depends on. A `.d.ts` file is generated alone and cannot be combined with other inputs.

## Options

### `--out-file`

**type:** `path`

**default:** the standard output

The file to write the binding to. Its directory is created.

### `--all`

**type:** `flag`

Generates every package installed in the nearest `node_modules`.

### `--external`

**type:** `<package>` or `<package>=<Module>`

References the package from its own binding instead of generating it. The types are written as `Glutinum.<Module>.X`, with `<Module>` derived from the package name unless given. The option can be repeated.

```bash frame="terminal"
npx @glutinum/cli @types/leaflet --external @types/geojson=Geojson
```

### `--no-externals`

**type:** `flag`

Generates `@types/node` and `@types/web` with the packages using them instead of referencing `Glutinum.Node` and `Glutinum.Web`.

### `--version`

Prints the version of the package.

### `--help`

Prints the usage.

## Examples

```bash frame="terminal"
npx @glutinum/cli chalk --out-file Glutinum.Chalk.fs
npx @glutinum/cli vscode vscode-languageclient --out-file Glutinum.Vscode.fs
npx @glutinum/cli @types/leaflet --external @types/geojson --out-file Glutinum.Leaflet.fs
npx @glutinum/cli --all --out-file Glutinum.fs
npx @glutinum/cli ./node_modules/my-lib/index.d.ts
```
