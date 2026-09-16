const DECLARATION_FILE = /\.d\.[cm]?ts$/;
const REGISTRY = "https://data.jsdelivr.com/v1/package";
const CDN = "https://cdn.jsdelivr.net/npm";
const CONCURRENCY = 8;

/**
 * `@types/foo` for `foo`, `@types/scope__foo` for `@scope/foo`
 *
 * @param {string} name
 */
export function typesPackageName(name) {
    return "@types/" + name.replace(/^@/, "").replace("/", "__");
}

/**
 * `chalk`, `chalk@5`, `@scope/name@^1.2`
 *
 * @param {string} spec
 * @returns {{ name: string, range: string }}
 */
export function parsePackageSpec(spec) {
    const at = spec.indexOf("@", 1);

    if (at === -1) {
        return { name: spec, range: "latest" };
    }

    return { name: spec.slice(0, at), range: spec.slice(at + 1) || "latest" };
}

/**
 * Download the declaration files of a package and of the packages it depends on into
 * `/node_modules` of the file system, like `npm install` would, from jsDelivr.
 *
 * @param {import("@ts-morph/bootstrap").FileSystemHost} fileSystem
 * @param {string} spec package name, with an optional version range
 * @param {{ fetch?: typeof fetch, onProgress?: (message: string) => void, skip?: string[] }} options
 * @returns {Promise<{ name: string, version: string }>} the package installed for `spec`
 */
export async function installPackage(fileSystem, spec, options = {}) {
    const fetchImpl = options.fetch ?? globalThis.fetch;
    const onProgress = options.onProgress ?? (() => {});
    // `@types/node` describes the runtime, it is not needed to generate a package
    const skip = new Set(options.skip ?? ["@types/node"]);
    const installed = new Map();

    const getJson = async (url) => {
        const response = await fetchImpl(url);

        if (!response.ok) {
            throw new Error(`${url}: ${response.status}`);
        }

        return await response.json();
    };

    const getText = async (url) => {
        const response = await fetchImpl(url);

        if (!response.ok) {
            throw new Error(`${url}: ${response.status}`);
        }

        return await response.text();
    };

    const install = async (name, range) => {
        if (installed.has(name) || skip.has(name)) {
            return installed.get(name) ?? null;
        }

        const resolveResponse = await fetchImpl(`${REGISTRY}/resolve/npm/${name}@${encodeURIComponent(range)}`);

        if (resolveResponse.status === 404) {
            throw new Error(`No package named '${name}' on npm`);
        }

        if (!resolveResponse.ok) {
            throw new Error(`${resolveResponse.url}: ${resolveResponse.status}`);
        }

        const { version } = await resolveResponse.json();

        if (version === null || version === undefined) {
            throw new Error(`No version of '${name}' matches '${range}'`);
        }

        installed.set(name, { name, version });
        onProgress(`${name}@${version}: listing files`);

        const { files } = await getJson(`${REGISTRY}/npm/${name}@${version}/flat`);

        const wanted = files
            .map((file) => file.name)
            .filter((file) => DECLARATION_FILE.test(file) || file.endsWith("/package.json"));

        let done = 0;

        const worker = async () => {
            while (wanted.length > 0) {
                const file = wanted.shift();
                const content = await getText(`${CDN}/${name}@${version}${file}`);

                fileSystem.writeFileSync(`/node_modules/${name}${file}`, content);
                done++;
                onProgress(`${name}@${version}: ${done} files`);
            }
        };

        await Promise.all(Array.from({ length: CONCURRENCY }, worker));

        const packageJson = JSON.parse(fileSystem.readFileSync(`/node_modules/${name}/package.json`, "utf8"));

        const hasDeclarations = files.some((file) => DECLARATION_FILE.test(file.name));

        // A package without declaration files (`ws`) is described by its `@types` package
        if (!hasDeclarations && !name.startsWith("@types/")) {
            try {
                await install(typesPackageName(name), "latest");
            } catch {
                // No `@types` package either, the package stays untyped
            }
        }

        for (const [dependency, dependencyRange] of Object.entries(packageJson.dependencies ?? {})) {
            await install(dependency, dependencyRange);
        }

        return installed.get(name);
    };

    const { name, range } = parsePackageSpec(spec);
    const result = await install(name, range);

    // `ws` itself has no declaration file: the package to generate is its `@types` package
    return result;
}
