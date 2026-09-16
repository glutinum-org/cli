const DECLARATION_FILE = /\.d\.[cm]?ts$/;

/**
 * Directory of the nearest `node_modules` folder, walking up from `cwd`.
 *
 * @param {import("./host.js").Host} host
 * @param {string} cwd
 * @returns {string | null}
 */
export function findNodeModules(host, cwd) {
    const { path, fs } = host;
    let dir = path.resolve(cwd);

    while (true) {
        const candidate = path.join(dir, "node_modules");

        if (fs.directoryExists(candidate)) {
            return candidate;
        }

        const parent = path.dirname(dir);

        if (parent === dir) {
            return null;
        }

        dir = parent;
    }
}

/**
 * Real path of a directory: pnpm links `node_modules/<pkg>` into its store, the
 * dependencies of a package are only reachable from the real location.
 *
 * @param {string} dir
 * @returns {string}
 */
function realDir(host, dir) {
    return host.fs.realPath(dir);
}

/**
 * Directory of the package that owns `file`: the closest parent directory containing a `package.json`.
 *
 * @param {import("./host.js").Host} host
 * @param {string} file
 * @returns {string | null}
 */
export function findPackageDir(host, file) {
    const { path, fs } = host;
    let dir = path.dirname(path.resolve(file));

    while (true) {
        const packageJsonPath = path.join(dir, "package.json");

        // `dist/esm/package.json` files only holding `{ "type": "module" }` do not delimit a package
        if (fs.fileExists(packageJsonPath) && JSON.parse(fs.readFile(packageJsonPath)).name !== undefined) {
            return realDir(host, dir);
        }

        const parent = path.dirname(dir);

        if (parent === dir) {
            return null;
        }

        dir = parent;
    }
}

/**
 * The `types` entries found in a package.json `exports` map, keyed by subpath.
 *
 * @param {unknown} exportsField
 * @returns {{ subpath: string, file: string }[]}
 */
function collectExportsTypes(exportsField) {
    const result = [];

    const visit = (subpath, node) => {
        if (node === null || node === undefined) {
            return;
        }

        if (typeof node === "string") {
            if (DECLARATION_FILE.test(node)) {
                result.push({ subpath, file: node });
            }

            return;
        }

        if (typeof node === "object") {
            for (const [key, value] of Object.entries(node)) {
                if (key.startsWith(".")) {
                    visit(key, value);
                } else if (key === "types" || key === "typings") {
                    visit(subpath, value);
                } else {
                    // Conditions such as `import`, `default`, `node`, ...
                    visit(subpath, value);
                }
            }
        }
    };

    if (typeof exportsField === "string" || (exportsField && !Object.keys(exportsField).some((key) => key.startsWith(".")))) {
        visit(".", exportsField);
    } else {
        visit(".", exportsField);
    }

    // Keep the first declaration file found for a subpath
    const bySubpath = new Map();

    for (const entry of result) {
        if (!bySubpath.has(entry.subpath)) {
            bySubpath.set(entry.subpath, entry.file);
        }
    }

    return [...bySubpath.entries()].map(([subpath, file]) => ({ subpath, file }));
}

/**
 * Describe an installed package: its runtime name and declaration entry points.
 *
 * @param {import("./host.js").Host} host
 * @param {string} packageDir
 * @returns {{ name: string, runtimeName: string, dir: string, entryFile: string, subpathEntries: { subpath: string, file: string }[] } | null}
 */
export function describePackage(host, packageDir) {
    const { path, fs } = host;
    const packageJsonPath = path.join(packageDir, "package.json");

    if (!fs.fileExists(packageJsonPath)) {
        return null;
    }

    const pkg = JSON.parse(fs.readFile(packageJsonPath));
    const name = pkg.name ?? path.basename(packageDir);

    // `@types/foo` describes the `foo` package, `@types/scope__foo` the `@scope/foo` one
    let runtimeName = name;

    if (name.startsWith("@types/")) {
        const typed = name.slice("@types/".length);
        runtimeName = typed.includes("__") ? "@" + typed.replace("__", "/") : typed;
    }

    const candidates = [];
    const typesField = pkg.types ?? pkg.typings;

    if (typeof typesField === "string") {
        candidates.push({ subpath: ".", file: typesField });
    }

    candidates.push(...collectExportsTypes(pkg.exports));

    if (typeof pkg.main === "string") {
        candidates.push({ subpath: ".", file: pkg.main.replace(/\.[cm]?js$/, ".d.ts") });
    }

    candidates.push({ subpath: ".", file: "index.d.ts" });

    const entries = [];

    for (const candidate of candidates) {
        let file = path.resolve(packageDir, candidate.file);

        // `"types": "./lib/umd/main"` is allowed without an extension
        if (!DECLARATION_FILE.test(file) && fs.fileExists(file + ".d.ts")) {
            file = file + ".d.ts";
        }

        if (!fs.fileExists(file) || !DECLARATION_FILE.test(file)) {
            continue;
        }

        if (!entries.some((entry) => entry.subpath === candidate.subpath || entry.file === file)) {
            entries.push({ subpath: candidate.subpath, file });
        }
    }

    const main = entries.find((entry) => entry.subpath === ".");

    if (main === undefined) {
        return null;
    }

    return {
        name,
        runtimeName,
        dir: packageDir,
        entryFile: main.file,
        subpathEntries: entries
            .filter((entry) => entry !== main)
            .map((entry) => ({ subpath: entry.subpath.replace(/^\.\//, ""), file: entry.file })),
    };
}

/**
 * Resolve the CLI input to a package directory or a declaration file.
 *
 * @param {import("./host.js").Host} host
 * @param {string} input package name, package directory or `.d.ts` path
 * @returns {{ kind: "file", file: string } | { kind: "package", packageDir: string }}
 */
export function resolveInput(host, input) {
    const { path, fs } = host;
    const asPath = path.resolve(host.cwd, input);

    if (DECLARATION_FILE.test(input) && fs.fileExists(asPath)) {
        return { kind: "file", file: asPath };
    }

    if (fs.directoryExists(asPath)) {
        return { kind: "package", packageDir: realDir(host, asPath) };
    }

    const nodeModules = findNodeModules(host, host.cwd);

    if (nodeModules !== null) {
        // A package without declaration files (`ws`) is described by its `@types` package
        for (const candidate of [input, path.join("@types", input.replace(/^@/, "").replace("/", "__"))]) {
            const packageDir = realDir(host, path.join(nodeModules, candidate));

            if (fs.fileExists(path.join(packageDir, "package.json")) && describePackage(host, packageDir) !== null) {
                return { kind: "package", packageDir };
            }
        }
    }

    throw new Error(
        `Could not find '${input}': it is neither a declaration file, a package directory, nor a package installed in node_modules.`
    );
}

/**
 * Every package installed in the nearest `node_modules` that ships declaration files.
 *
 * @param {import("./host.js").Host} host
 * @returns {string[]} package directories
 */
export function listInstalledPackages(host) {
    const { path, fs } = host;
    const nodeModules = findNodeModules(host, host.cwd);

    if (nodeModules === null) {
        return [];
    }

    const result = [];

    for (const entry of fs.readDirectory(nodeModules)) {
        if (entry.startsWith(".")) {
            continue;
        }

        const entryPath = path.join(nodeModules, entry);

        if (entry.startsWith("@")) {
            for (const scoped of fs.readDirectory(entryPath)) {
                result.push(path.join(entryPath, scoped));
            }
        } else {
            result.push(entryPath);
        }
    }

    return result
        .filter((dir) => fs.fileExists(path.join(dir, "package.json")))
        .map((dir) => realDir(host, dir))
        .filter((dir) => describePackage(host, dir) !== null);
}
