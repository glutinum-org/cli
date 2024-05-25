# Glutinum.CLI

This is a compiler from `.d.ts` to F# bindings for [Fable](https://fable.io/).

## Contributing

Glutinum.CLI use `./build.sh` or `./build.bat` as a build script.

You can see the available options by running:

```bash
./build.sh --help
```

When using the test command, you can focus on a specific by forwarding arguments to `vitest`:

```bash
# Focus all tests containing related to specs/class/
./build.sh test --watch -- -t class/
```

If you need to run a local version of `@glutinum/cli`, you can use `./build.sh cli [--watch]` and then run `node cli.js <args>`.

### Debugging

If you use VSCode, you can run the build script/commands from the `JavaScript Debug Terminal`. This allows you to have access to the debugger, breakpoints, etc. (it works from the F# files too).

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

#### Vitest

This project use [Vitest](https://vitest.dev/) for running tests.

Vitest was chosen because it seems to have a lot of attention and is actively maintained. Plus, it seems well integrated with VSCode and Rider, allowing us to use the test explorer and even debug the tests using source maps.

If you prefer, it is possible to run the tests via the CLI.

##### VSCode

Install [Vitest plugin](https://marketplace.visualstudio.com/items?itemName=vitest.explorer), then you will be able to run the tests from the test explorer.

##### Rider

You need to add a new configuration of type `Vitest`.

1. `Run > Edit Configurations...`
2. Add a new configuration of type `Vitest`
3. Then you can run the tests by selecting the configuration in the top right corner of the IDE.

##### Specs

> [!TIP]
> Run `./build.sh --help` to see the available options (look for `test specs` command).

Specs tests are used to test isolated TypeScript syntax and their conversion to F#.

They are generated based on the `tests/specs/references` folder.

Each `.d.ts` correspond to a test and have a matching `.fsx` file.

When running `./build.sh test specs`, it will generate a similar hierarchy in the `tests/specs/generated` folder.

Example:

```text
tests/specs/references
├── interfaces
│   ├── callSignature.d.ts
│   ├── callSignature.fsx
│   └── indexSignature
│       ├── numberParameter.d.ts
│       └── numberParameter.fsx
└── typeQuery
    ├── class.d.ts
    ├── class.fsx
    ├── defaultToObj.d.ts
    └── defaultToObj.fsx
```

generates:

```text
tests/specs/generated
├── interfaces
│   ├── index.test.js
│   └── indexSignature
│       └── index.test.js
└── typeQuery
    └── index.test.js
```

`index.test.js` contains all the tests for the `.d.ts` of the same folder.

> [!NOTE]
> If you are using VSCode, the `fsx` file will be nested under the `d.ts` file in your explorer.

The `.fsx` correspond to the expected result suffixed with the following:

```fs

(***)
#r "nuget: Fable.Core"
(***)

```

> This allows us to have IDE support in the `.fsx` file instead of having a lot of syntax errors.

#### `tests/specs/index.js`

Sometimes debugging tests through Vitest runner / extensions, is not easy. This is why, we provide a `./tests/index.js` scripts which allows you to manually check a specs transformation.

```bash
node --enable-source-maps tests/specs tests/specs/references/exports/variable.d.ts
```

This avoid situation where Vitest extension needs a restart of VSCode to work again, etc.

## Tools

One handy tool when working on the TypeScript AST is [TypeScript AST Viewer](https://ts-ast-viewer.com/).

It allows you to have a visual representation of the AST.

> Make sure to use the same version of TypeScript as the one used by Glutinum.CLI (found in the `package.json` file). Otherwise, you might have some differences especially in the values of the `kind` property.
