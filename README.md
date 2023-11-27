Glutinum.Converter

| Tasks | Command |
|---|---|
| Test  | `dotnet fable tests --sourceMaps --run npx ava tests`  |
| Watch test  |   `dotnet fable tests --watch --sourceMaps --run npx ava tests --watch` |
| Watch specific test | `dotnet fable tests --watch --sourceMaps --run npx ava tests --watch --match="**XXX**"` |

You can also use attach a JavaScript debugger while debugging a specific test by compile the CLI tool:

- Terminal: `dotnet fable src/Glutinum.Converter.CLI --watch --sourceMaps`
- JavaScript debug terminal (VSCode): `node src/Glutinum.Converter.CLI/Program.fs.js ./tests/specs/enums/literalNumericEnum.d.ts`
