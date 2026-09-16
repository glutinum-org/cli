import { expect, test } from 'vitest'
import { generatePackages } from '../../../src/Glutinum.Converter/Generate.fs.js'
import { ofArray } from '../../../src/Glutinum.Converter/fable_modules/fable-library-js.5.2.0/List.js'
import { dirname } from "dirname-filename-esm";
import path from 'node:path';

const __dirname = dirname(import.meta)

// Each fixture is a package directory: a `package.json` pointing at an entry `.d.ts`
// and the files it imports. The package is generated as a single `.fs` file.
const fixtures = [
    "multiFile",
    "duplicateTypes",
    "externalDependency",
    "reExports",
]

const footer = `
(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
`

for (const fixture of fixtures) {
    test(`packages/${fixture}`, async () => {
        // Reference: file://./fixtures/${fixture}
        // Expected: file://./fixtures/${fixture}.fsx
        const packageDir = path.join(__dirname, "fixtures", fixture)
        const result = generatePackages(ofArray([packageDir])) + footer

        const expectedFile = path.join(__dirname, "fixtures", fixture + ".fsx")

        await expect(result).toMatchFileSnapshot(expectedFile)
    })
}
