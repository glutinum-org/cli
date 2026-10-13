import { expect, test } from 'vitest'
import { generatePackagesOfArray } from '../../../src/Glutinum.Converter/Generate.fs.js'
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
    "renamedImport",
    "unionAlias",
    "ambientModules",
    "globalScript",
    "externalRuntime",
    "globalAugmentation",
    "typesOnlyDependency",
    "subpathEntries",
    "exportsMap",
    "exportsMapEntry",
    "factory",
    "sameScopeAcrossModules",
    "subpathCasing",
    "unresolvedImport",
    "namespaceImport",
    "externalParamObject",
    "crossFileConditional",
    "unboundTypeParameter",
    "reExportedConstraint",
    "reExportedValueAndType",
    "iteratorObjectBase",
    "nestedAliasApplication",
    "classShadowsReExport",
    "typeNamedLikePackage",
    "typeofUnexportedNamespace",
    "externalLibNamespace",
    "reExportedGenericFunctionAlias",
    "aliasedImportHeritage",
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
        const result = generatePackagesOfArray(null, [packageDir]) + footer

        const expectedFile = path.join(__dirname, "fixtures", fixture + ".fsx")

        await expect(result).toMatchFileSnapshot(expectedFile)
    })
}

// The browser has no disk: the same package read from an in-memory file system gives the same binding
import { createInMemoryHost } from '../../../src/Glutinum.Converter/Hosting.fs.js'
import fs from 'node:fs'

test("packages/multiFile in memory", async () => {
    const packageDir = path.join(__dirname, "fixtures", "multiFile")
    const host = createInMemoryHost("/")
    const fileSystem = host.createProject({}).fileSystem

    for (const file of fs.readdirSync(packageDir)) {
        fileSystem.writeFileSync("/node_modules/multi-file/" + file, fs.readFileSync(path.join(packageDir, file), "utf8"))
    }

    const result = generatePackagesOfArray(host, ["multi-file"]) + footer

    await expect(result).toMatchFileSnapshot(path.join(__dirname, "fixtures", "multiFile.fsx"))
})
