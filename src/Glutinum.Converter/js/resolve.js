import { ts } from "@ts-morph/bootstrap";

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
 * Whether a `package.json` `exports` map names a file that is not a declaration file.
 *
 * @param {unknown} node
 * @returns {boolean}
 */
function exportsRuntimeFile(node) {
    if (typeof node === "string") {
        return !DECLARATION_FILE.test(node);
    }

    if (node !== null && typeof node === "object") {
        return Object.values(node).some(exportsRuntimeFile);
    }

    return false;
}

/**
 * Whether the package ships JavaScript. `undici-types` is declaration files and nothing else,
 * importing it throws at runtime.
 *
 * @param {import("./host.js").Host} host
 * @param {string} packageDir
 * @param {Record<string, unknown>} pkg
 * @returns {boolean}
 */
function hasRuntime(host, packageDir, pkg) {
    const { path, fs } = host;

    // `csstype` declares `"main": ""`
    const declared = (field) => (typeof field === "string" ? field !== "" : field !== undefined && field !== null);

    if (declared(pkg.main) || declared(pkg.module) || declared(pkg.bin) || declared(pkg.browser)) {
        return true;
    }

    if (exportsRuntimeFile(pkg.exports)) {
        return true;
    }

    // `.glutinum-runtime` is written by the downloader of the web app, which only fetches declarations
    return ["index.js", "index.mjs", "index.cjs", ".glutinum-runtime"].some((file) =>
        fs.fileExists(path.join(packageDir, file))
    );
}

/**
 * `[major, minor, patch]` of a version, missing parts are 0
 *
 * @param {string} version
 */
function parseVersion(version) {
    const parts = version.split(".").map((part) => parseInt(part, 10));

    return [parts[0] || 0, parts[1] || 0, parts[2] || 0];
}

/**
 * @param {number[]} a
 * @param {number[]} b
 */
function compareVersions(a, b) {
    for (let i = 0; i < 3; i++) {
        if (a[i] !== b[i]) {
            return a[i] - b[i];
        }
    }

    return 0;
}

/**
 * Whether `version` satisfies one comparator of a `typesVersions` range (`<=5.5`, `>=4.2`, `*`)
 *
 * @param {number[]} version
 * @param {string} comparator
 */
function satisfiesComparator(version, comparator) {
    const match = comparator.match(/^(>=|<=|>|<|=|\^|~)?\s*(.+)$/);

    if (match === null) {
        return false;
    }

    const operator = match[1] ?? "=";
    const text = match[2];

    if (text === "*" || text === "x") {
        return true;
    }

    const parts = text.split(".").map((part) => parseInt(part, 10)).filter((part) => !isNaN(part));
    const lower = [parts[0] ?? 0, parts[1] ?? 0, parts[2] ?? 0];

    // `5.5` stands for every `5.5.x`, the bound after it is `5.6.0`
    const upper =
        parts.length === 1 ? [lower[0] + 1, 0, 0]
        : parts.length === 2 ? [lower[0], lower[1] + 1, 0]
        : [lower[0], lower[1], lower[2] + 1];

    switch (operator) {
        case ">=":
            return compareVersions(version, lower) >= 0;
        case ">":
            return compareVersions(version, upper) >= 0;
        case "<":
            return compareVersions(version, lower) < 0;
        case "<=":
            return compareVersions(version, upper) < 0;
        case "^":
            return compareVersions(version, lower) >= 0 && compareVersions(version, [lower[0] + 1, 0, 0]) < 0;
        case "~":
            return compareVersions(version, lower) >= 0 && compareVersions(version, [lower[0], lower[1] + 1, 0]) < 0;
        default:
            return compareVersions(version, lower) >= 0 && compareVersions(version, upper) < 0;
    }
}

/**
 * @param {string} version
 * @param {string} range
 */
function satisfiesRange(version, range) {
    const parsed = parseVersion(version);

    return range
        .split("||")
        .some((alternative) => alternative.trim().split(/\s+/).every((comparator) => satisfiesComparator(parsed, comparator)));
}

/**
 * The declaration file to use for the bundled TypeScript, following the `typesVersions` of the
 * package (`{ "<=5.5": { "*": ["ts5.5/*"] } }`).
 *
 * @param {Record<string, Record<string, string[]>> | undefined} typesVersions
 * @param {string} file
 */
function applyTypesVersions(typesVersions, file) {
    if (typesVersions === undefined || typesVersions === null) {
        return file;
    }

    const paths = Object.entries(typesVersions).find(([range]) => satisfiesRange(ts.version, range))?.[1];

    if (paths === undefined) {
        return file;
    }

    const normalized = file.replace(/^\.\//, "");

    for (const [pattern, targets] of Object.entries(paths)) {
        const [prefix, suffix = ""] = pattern.split("*");

        if (normalized.startsWith(prefix) && normalized.endsWith(suffix) && targets.length > 0) {
            const captured = normalized.slice(prefix.length, normalized.length - suffix.length);

            return targets[0].replace("*", captured);
        }
    }

    return file;
}

/**
 * Describe an installed package: its runtime name and declaration entry points.
 *
 * @param {import("./host.js").Host} host
 * @param {string} packageDir
 * @returns {{ name: string, runtimeName: string, hasRuntime: boolean, dir: string, typesRoot: string, entryFile: string, subpathEntries: { subpath: string, file: string }[] } | null}
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
        const mappedFile = applyTypesVersions(pkg.typesVersions, candidate.file);
        let file = path.resolve(packageDir, mappedFile);

        // `"types": "./lib/umd/main"` is allowed without an extension
        if (!DECLARATION_FILE.test(file) && fs.fileExists(file + ".d.ts")) {
            file = file + ".d.ts";
        }

        if (!fs.fileExists(file) || !DECLARATION_FILE.test(file)) {
            continue;
        }

        if (!entries.some((entry) => entry.subpath === candidate.subpath || entry.file === file)) {
            entries.push({ subpath: candidate.subpath, file, isMapped: mappedFile !== candidate.file });
        }
    }

    const main = entries.find((entry) => entry.subpath === ".");

    if (main === undefined) {
        return null;
    }

    return {
        name,
        runtimeName,
        hasRuntime: name.startsWith("@types/") || hasRuntime(host, packageDir, pkg),
        dir: packageDir,
        // The files of a `typesVersions` folder are named as if they were at the root
        typesRoot: main.isMapped ? path.dirname(main.file) : packageDir,
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
        // `date-fns/locale` is a subpath of `date-fns`, generated with the package
        const segments = input.split("/");
        const packageName = input.startsWith("@") ? segments.slice(0, 2).join("/") : segments[0];

        // A package without declaration files (`ws`) is described by its `@types` package
        for (const candidate of [packageName, path.join("@types", packageName.replace(/^@/, "").replace("/", "__"))]) {
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
