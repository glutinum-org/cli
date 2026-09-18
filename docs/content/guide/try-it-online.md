---
title: Try it online
order: 2
---

The [web application](../app.md) runs the generator in the browser. Nothing is installed and nothing leaves your machine.

## Single file

Paste the content of a `.d.ts` file in the editor. The F# binding is generated as you type, and a button copies it.

Use this mode to see how a construct is translated before you generate a whole package, and to reduce a problem to a few lines when you [report an issue](https://github.com/glutinum-org/cli/issues).

## Package

Name an installed npm package and a version. The application downloads its declaration files and the ones of the packages it depends on, then generates them together, the same way the [command line](packages.md) does.

## Report an issue

The **Report an issue** button opens a GitHub issue prefilled with the input and the output. Reduce the input to the smallest declaration that shows the problem first.

:::note
The web application is published on every commit, so it is often ahead of the npm package.
:::
