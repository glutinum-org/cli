import { createProjectSync } from "@ts-morph/bootstrap";

/**
 *
 * @param {string} filePath
 * @param {string} source
 * @returns
 */
export default function createProgramForCLI(filePath, source) {

    // `strict`, so that `T | undefined` is kept by the checker
    const project = createProjectSync({ compilerOptions: { strict: true } })

    project.createSourceFile(filePath, source)

    return project.createProgram()
}

/**
 * Create a program from declaration files on disk, following their imports.
 *
 * @param {string[]} entryFiles
 * @returns
 */
export function createProgramFromFiles(entryFiles) {
    // ESNext, so that lib types such as `AsyncIterable` resolve instead of being left undefined.
    // `types: []` stops TypeScript from loading every package under `node_modules/@types`.
    const project = createProjectSync({
        // ESNext module + Bundler resolution: follows `exports` maps and node_modules
        compilerOptions: { target: 99, module: 99, moduleResolution: 100, types: [], strict: true },
        skipAddingFilesFromTsConfig: true,
    })

    for (const entryFile of entryFiles) {
        project.addSourceFileAtPathSync(entryFile)
    }

    project.resolveSourceFileDependencies()

    return project.createProgram()
}
