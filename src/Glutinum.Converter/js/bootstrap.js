import { createProjectSync, ts } from "@ts-morph/bootstrap";
import { describePackage, findPackageDir } from "./resolve.js";

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
 * Create a program from declaration files of the host, following their imports.
 *
 * @param {import("./host.js").Host} host
 * @param {string[]} entryFiles
 * @param {{ withoutDomLib?: boolean, noLib?: boolean }} options
 * @returns
 */
export function createProgramFromFiles(host, entryFiles, options = {}) {
    // ESNext, so that lib types such as `AsyncIterable` resolve instead of being left undefined.
    // `types: []` stops TypeScript from loading every package under `node_modules/@types`.
    // ESNext module + Bundler resolution: follows `exports` maps and node_modules
    const compilerOptions = { target: 99, module: 99, moduleResolution: 100, types: [], strict: true }

    // A package replacing the DOM lib (`@types/web`) redeclares its globals
    if (options.withoutDomLib) {
        compilerOptions.lib = ["lib.esnext.d.ts", "lib.decorators.d.ts", "lib.decorators.legacy.d.ts"]
    }

    // A package made of the ES library files declares the library itself
    if (options.noLib) {
        compilerOptions.noLib = true
    }

    const project = host.createProject(compilerOptions)

    for (const entryFile of entryFiles) {
        project.addSourceFileAtPathSync(entryFile)
    }

    project.resolveSourceFileDependencies()

    return project.createProgram()
}

/**
 * The files reachable from the entry files through imports and references, without
 * going through a package of `excludedRuntimeNames` (`node` for `@types/node`).
 *
 * @param {import("./host.js").Host} host
 * @param {import("typescript").Program} program
 * @param {string[]} entryFiles
 * @param {string[]} excludedRuntimeNames
 * @returns {string[]}
 */
export function reachableFiles(host, program, entryFiles, excludedRuntimeNames) {
    const runtimeNames = new Map();

    const isExcluded = (fileName) => {
        const dir = findPackageDir(host, fileName);

        if (dir === null) {
            return false;
        }

        if (!runtimeNames.has(dir)) {
            const description = describePackage(host, dir);
            runtimeNames.set(dir, description === null ? null : description.runtimeName);
        }

        return excludedRuntimeNames.includes(runtimeNames.get(dir));
    };

    const seen = new Set();
    const queue = [...entryFiles];

    const visit = (fileName) => {
        if (fileName !== undefined && !seen.has(fileName) && !isExcluded(fileName)) {
            queue.push(fileName);
        }
    };

    while (queue.length > 0) {
        const fileName = queue.pop();

        if (seen.has(fileName)) {
            continue;
        }

        const sourceFile = program.getSourceFile(fileName);

        if (sourceFile === undefined) {
            continue;
        }

        seen.add(fileName);

        for (const usage of sourceFile.imports ?? []) {
            const mode = ts.getModeForUsageLocation(sourceFile, usage);
            visit(program.getResolvedModule(sourceFile, usage.text, mode)?.resolvedModule?.resolvedFileName);
        }

        for (const reference of sourceFile.referencedFiles ?? []) {
            visit(host.path.resolve(host.path.dirname(sourceFile.fileName), reference.fileName));
        }

        for (const reference of sourceFile.typeReferenceDirectives ?? []) {
            const resolved = program.getResolvedTypeReferenceDirective(sourceFile, reference.fileName, reference.resolutionMode);
            visit(resolved?.resolvedTypeReferenceDirective?.resolvedFileName);
        }
    }

    return [...seen];
}
