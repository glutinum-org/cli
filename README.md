Glutinum.Converter

Commands:

Compile the test files: `dotnet fable tests --watch --sourceMaps`

Run the tests: `npx ava tests --watch`

If you want to focus on specific tests, you can use the `--match` option of AVA.

Example:

This command `npx ava tests --watch --match="**mappedType**"` will match all tests containing the string `mappedType` in their name.
