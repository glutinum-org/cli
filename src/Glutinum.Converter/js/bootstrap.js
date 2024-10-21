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
