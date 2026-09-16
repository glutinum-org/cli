import { expect, test } from 'vitest'
import { InMemoryFileSystemHost } from '@ts-morph/bootstrap'
import { installPackage, parsePackageSpec, typesPackageName } from '../../../src/Glutinum.Converter/js/npm.js'
import { generatePackagesWith } from '../../../src/Glutinum.Converter/Generate.fs.js'
import { createFileSystemHost } from '../../../src/Glutinum.Converter/js/host.js'
import { ofArray } from '../../../src/Glutinum.Converter/fable_modules/fable-library-js.5.2.0/List.js'

// A registry with `my-lib` depending on `dep-lib`, and `untyped` described by `@types/untyped`
const registry = {
    "https://data.jsdelivr.com/v1/package/resolve/npm/my-lib@latest": { version: "1.0.0" },
    "https://data.jsdelivr.com/v1/package/npm/my-lib@1.0.0/flat": { files: [{ name: "/package.json" }, { name: "/index.d.ts" }, { name: "/index.js" }] },
    "https://cdn.jsdelivr.net/npm/my-lib@1.0.0/package.json": JSON.stringify({ name: "my-lib", types: "index.d.ts", dependencies: { "dep-lib": "^2.0.0", untyped: "1.x" } }),
    "https://cdn.jsdelivr.net/npm/my-lib@1.0.0/index.d.ts": `import { Color } from "dep-lib";\nimport { Thing } from "untyped";\nexport declare function paint(color: Color, thing: Thing): void;\n`,
    "https://data.jsdelivr.com/v1/package/resolve/npm/dep-lib@%5E2.0.0": { version: "2.3.0" },
    "https://data.jsdelivr.com/v1/package/npm/dep-lib@2.3.0/flat": { files: [{ name: "/package.json" }, { name: "/index.d.ts" }] },
    "https://cdn.jsdelivr.net/npm/dep-lib@2.3.0/package.json": JSON.stringify({ name: "dep-lib", types: "index.d.ts" }),
    "https://cdn.jsdelivr.net/npm/dep-lib@2.3.0/index.d.ts": `export interface Color { hex: string }\n`,
    "https://data.jsdelivr.com/v1/package/resolve/npm/untyped@1.x": { version: "1.4.0" },
    "https://data.jsdelivr.com/v1/package/npm/untyped@1.4.0/flat": { files: [{ name: "/package.json" }, { name: "/index.js" }] },
    "https://cdn.jsdelivr.net/npm/untyped@1.4.0/package.json": JSON.stringify({ name: "untyped", main: "index.js" }),
    "https://data.jsdelivr.com/v1/package/resolve/npm/@types/untyped@latest": { version: "1.0.2" },
    "https://data.jsdelivr.com/v1/package/npm/@types/untyped@1.0.2/flat": { files: [{ name: "/package.json" }, { name: "/index.d.ts" }] },
    "https://cdn.jsdelivr.net/npm/@types/untyped@1.0.2/package.json": JSON.stringify({ name: "@types/untyped", types: "index.d.ts" }),
    "https://cdn.jsdelivr.net/npm/@types/untyped@1.0.2/index.d.ts": `export interface Thing { size: number }\n`,
}

const fakeFetch = async (url) => {
    const body = registry[url]

    if (body === undefined) {
        return { ok: false, status: 404 }
    }

    return {
        ok: true,
        status: 200,
        json: async () => body,
        text: async () => body,
    }
}

test("parsePackageSpec", () => {
    expect(parsePackageSpec("chalk")).toEqual({ name: "chalk", range: "latest" })
    expect(parsePackageSpec("chalk@5")).toEqual({ name: "chalk", range: "5" })
    expect(parsePackageSpec("@scope/name@^1.2")).toEqual({ name: "@scope/name", range: "^1.2" })
    expect(typesPackageName("@scope/name")).toBe("@types/scope__name")
})

test("installPackage downloads the declarations of the package, its dependencies and their @types", async () => {
    const fileSystem = new InMemoryFileSystemHost()
    const messages = []

    const result = await installPackage(fileSystem, "my-lib", { fetch: fakeFetch, onProgress: (m) => messages.push(m) })

    expect(result).toEqual({ name: "my-lib", version: "1.0.0" })
    expect(fileSystem.fileExistsSync("/node_modules/my-lib/index.d.ts")).toBe(true)
    expect(fileSystem.fileExistsSync("/node_modules/my-lib/index.js")).toBe(false)
    expect(fileSystem.fileExistsSync("/node_modules/dep-lib/index.d.ts")).toBe(true)
    expect(fileSystem.fileExistsSync("/node_modules/@types/untyped/index.d.ts")).toBe(true)
    expect(messages.length).toBeGreaterThan(0)

    const host = createFileSystemHost(fileSystem, "/")
    const binding = generatePackagesWith(host, ofArray(["my-lib"]))

    expect(binding).toContain("module MyLib =")
    expect(binding).toContain("static member paint (color: DepLib.Color, thing: Untyped.Thing)")
    expect(binding).toContain("module DepLib =")
    expect(binding).toContain("module Untyped =")
})
