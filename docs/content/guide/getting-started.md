---
title: Getting started
order: 1
---

Glutinum generates an F# binding from the TypeScript declarations of a JavaScript package. The binding is a single `.fs` file you add to a [Fable](https://fable.io) project.

## Requirements

- Node.js 20 or later
- A Fable project, with the `Fable.Core` package

## Generate a binding

Install the package whose binding you want, then run Glutinum on it:

```bash frame="terminal"
npm install chalk
npx @glutinum/cli chalk --out-file Glutinum.Chalk.fs
```

The first line of the generated file says which NuGet packages it needs. Every binding needs `Glutinum.Types`. A binding that uses the DOM or Node types also needs `Glutinum.Web` or `Glutinum.Node`.

```bash frame="terminal"
dotnet add package Glutinum.Types
```

Add the file to the project before the files that use it:

```xml title="MyApp.fsproj" ins={3}
<ItemGroup>
    <Compile Include="Glutinum.Chalk.fs" />
    <Compile Include="Program.fs" />
</ItemGroup>
```

## Use it

Functions and constructors are static members of the `Exports` type of the binding. Open it with `open type` and call them like any F# member:

```fsharp title="Program.fs"
open Glutinum.Chalk

printfn "%s" (chalk.red "Hello")
```

The [reading the output](reading-the-output.md) page explains how each TypeScript construct is exposed.

## Without installing

The [web application](try-it-online.md) runs the same generator in the browser. Paste a `.d.ts` file or name a package, and copy the result.

## Next steps

- [Generate a whole package](packages.md), with the packages it depends on
- [Generate a single file](single-file.md), for a declaration you wrote yourself
- [Command line reference](command-line.md)
