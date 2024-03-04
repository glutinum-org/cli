import { createProjectSync } from "@ts-morph/bootstrap";

export default function createProgram(source : string) {

    const project = createProjectSync({
        useInMemoryFileSystem: true,
    })

    project.createSourceFile('index.d.ts', source)

    return project.createProgram()
}
