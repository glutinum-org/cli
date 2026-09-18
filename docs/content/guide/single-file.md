---
title: A single file
order: 3
---

Give Glutinum the path of a `.d.ts` file and it generates that file alone.

```bash frame="terminal"
npx @glutinum/cli ./node_modules/my-lib/index.d.ts --out-file Glutinum.MyLib.fs
```

Without `--out-file`, the binding is written to the standard output:

```bash frame="terminal"
npx @glutinum/cli ./node_modules/my-lib/index.d.ts > Glutinum.MyLib.fs
```

## What is read

The file and the types it declares. A type imported from another file is generated as a reference to a type that does not exist in the output.

Use this mode for a declaration file you wrote yourself, or for a package made of one file. For anything else, [generate the package](packages.md).

## The module name

The generated file imports from `REPLACE_ME_WITH_MODULE_NAME`. Replace it with the name the JavaScript is imported by, `my-lib` for `import { x } from "my-lib"`.

```fsharp
[<Import("greet", "my-lib")>]
static member greet (name: string) : string = nativeOnly
```
