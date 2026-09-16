import { createProjectSync } from "@ts-morph/bootstrap";

/**
 *
 * @param {string} filePath
 * @param {string} source
 * @returns
 */
export default function createProgramForCLI(filePath, source) {

    const project = createProjectSync({})

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
        compilerOptions: { target: 99, module: 99, moduleResolution: 100, types: [] },
        skipAddingFilesFromTsConfig: true,
    })

    for (const entryFile of entryFiles) {
        project.addSourceFileAtPathSync(entryFile)
    }

    project.resolveSourceFileDependencies()

    return project.createProgram()
}
