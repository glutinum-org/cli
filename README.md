# Glutinum.Converter

| Tasks | Command |
|---|---|
| Test  | `dotnet fable src/Glutinum.Converter --outDir fableBuild --sourceMaps --run vitest run`  |
| Watch test  |   `dotnet fable src/Glutinum.Converter --outDir fableBuild --sourceMaps --watch --run vitest run` |

You can also attach a JavaScript debugger while debugging a specific test by compile the CLI tool:

- Terminal: `dotnet fable src/Glutinum.Converter.CLI --watch --sourceMaps`
- JavaScript debug terminal (VSCode): `node src/Glutinum.Converter.CLI/Program.fs.js ./tests/specs/enums/literalNumericEnum.d.ts`
