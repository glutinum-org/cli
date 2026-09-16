import fs from "node:fs";
import path from "node:path";

const DECLARATION_FILE = /\.d\.[cm]?ts$/;

/**
 * Directory of the nearest `node_modules` folder, walking up from `cwd`.
 *
 * @param {string} cwd
 * @returns {string | null}
 */
export function findNodeModules(cwd) {
    let dir = path.resolve(cwd);

    while (true) {
        const candidate = path.join(dir, "node_modules");

        if (fs.existsSync(candidate) && fs.statSync(candidate).isDirectory()) {
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
function realDir(dir) {
    try {
        return fs.realpathSync(dir);
    } catch {
        return dir;
    }
}

/**
 * Directory of the package that owns `file`: the closest parent directory containing a `package.json`.
 *
 * @param {string} file
 * @returns {string | null}
 */
export function findPackageDir(file) {
    let dir = path.dirname(path.resolve(file));

    while (true) {
        const packageJsonPath = path.join(dir, "package.json");

        // `dist/esm/package.json` files only holding `{ "type": "module" }` do not delimit a package
        if (fs.existsSync(packageJsonPath) && JSON.parse(fs.readFileSync(packageJsonPath, "utf8")).name !== undefined) {
            return realDir(dir);
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
 * @param {string} packageDir
 * @returns {{ name: string, runtimeName: string, dir: string, entryFile: string, subpathEntries: { subpath: string, file: string }[] } | null}
 */
export function describePackage(packageDir) {
    const packageJsonPath = path.join(packageDir, "package.json");

    if (!fs.existsSync(packageJsonPath)) {
        return null;
    }

    const pkg = JSON.parse(fs.readFileSync(packageJsonPath, "utf8"));
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
        if (!DECLARATION_FILE.test(file) && fs.existsSync(file + ".d.ts")) {
            file = file + ".d.ts";
        }

        if (!fs.existsSync(file) || !DECLARATION_FILE.test(file)) {
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
 * @param {string} input package name, package directory or `.d.ts` path
 * @param {string} cwd
 * @returns {{ kind: "file", file: string } | { kind: "package", packageDir: string }}
 */
export function resolveInput(input, cwd = process.cwd()) {
    const asPath = path.resolve(cwd, input);

    if (DECLARATION_FILE.test(input) && fs.existsSync(asPath)) {
        return { kind: "file", file: asPath };
    }

    if (fs.existsSync(asPath) && fs.statSync(asPath).isDirectory()) {
        return { kind: "package", packageDir: realDir(asPath) };
    }

    const nodeModules = findNodeModules(cwd);

    if (nodeModules !== null) {
        for (const candidate of [input, path.join("@types", input.replace(/^@/, "").replace("/", "__"))]) {
            const packageDir = path.join(nodeModules, candidate);

            if (fs.existsSync(path.join(packageDir, "package.json"))) {
                return { kind: "package", packageDir: realDir(packageDir) };
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
 * @param {string} cwd
 * @returns {string[]} package directories
 */
export function listInstalledPackages(cwd = process.cwd()) {
    const nodeModules = findNodeModules(cwd);

    if (nodeModules === null) {
        return [];
    }

    const result = [];

    for (const entry of fs.readdirSync(nodeModules)) {
        if (entry.startsWith(".")) {
            continue;
        }

        const entryPath = path.join(nodeModules, entry);

        if (entry.startsWith("@")) {
            for (const scoped of fs.readdirSync(entryPath)) {
                result.push(path.join(entryPath, scoped));
            }
        } else {
            result.push(entryPath);
        }
    }

    return result
        .filter((dir) => fs.existsSync(path.join(dir, "package.json")))
        .map(realDir)
        .filter((dir) => describePackage(dir) !== null);
}
