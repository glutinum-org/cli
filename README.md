# Glutinum.CLI

This is a compiler from `.d.ts` to F# bindings for [Fable](https://fable.io/).

## Contributing

Glutinum.CLI use `./build.sh` or `./build.bat` as a build script.

You can see the available options by running:

```bash
./build.sh --help
```

When using the test command, you can focus on a specific by forwarding arguments to the `ava`:

```bash
# Focus all tests containing "class"
./build.sh test --watch -- --match="**class**"
```

## Architecture

### Glutinum.Converter.CLI

CLI tool which uses `Glutinum.Converter` to handle the conversion.

### Glutinum.Converter

This is the heart of the project. It contains the logic to convert `.d.ts` to F# bindings.

From a macro view, it does the following:

1. Read the TypeScript AST and transform it into GlueAST
2. Transform the GlueAST to FsharpAST
3. Print the F# code from FsharpAST

#### GlueAST

GlueAST philosophy is to follow the TypeScript AST naming convention as much as possible. Its goal is to provide an easier to use AST than the TypeScript one thanks to F# type system (mainly thanks to discriminated unions).

#### FsharpAST

FsharpAST provides a more idiomatic F# AST but also contains Fable specific information.

For example, `FSharpAttribute` doesn't just map to `string` but also Fable/Glutinum specific syntax.

```fs
[<RequireQualifiedAccess>]
type FSharpAttribute =
    | Text of string
    /// <summary>
    /// Generates <c>[&lt;Emit("$0($1...)")&gt;]</c> attribute.
    /// </summary>
    | EmitSelfInvoke
    | Import of string * string
    | ImportAll of string
    | Erase
    | AllowNullLiteral
    | StringEnum of Fable.Core.CaseRules
    | CompiledName of string
    | RequireQualifiedAccess
    | EmitConstructor
    | EmitMacroConstructor of className: string
    | EmitIndexer
```

### Tests

Tests are generated based on the `tests/specs/` folder content.

Each `.d.ts` correspond to a test and should have a matching `.fsx` file, the name of the test is the relative path to the `tests/specs/` folder without the `.d.ts` extension.

For example, if you have a file `tests/specs/exports/variable.d.ts`, you should have a `tests/specs/exports/variable.fsx`.

The name of the test is `exports/variable`.

> If you are using VSCode, the `fsx` file will be nested under the `d.ts` file in your explorer.

The `.fsx` correspond to the expected result suffixed with the following:

```fs

(***)
#r "nuget: Fable.Core"
(***)

```

> This allows us to have IDE support in the `.fsx` file instead of having a lot of syntax errors.

## Tools

One handy tool when working on the TypeScript AST is [TypeScript AST Viewer](https://ts-ast-viewer.com/).

It allows you to have a visual representation of the AST.

> Make sure to use the same version of TypeScript as the one used by Glutinum.CLI (found in the `package.json` file). Otherwise, you might have some differences especially in the values of the `kind` property.
