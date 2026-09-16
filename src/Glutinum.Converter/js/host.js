import { createProjectSync, InMemoryFileSystemHost } from "@ts-morph/bootstrap";

/**
 * @typedef {object} Host
 * @property {string} cwd
 * @property {{ join: (...parts: string[]) => string, dirname: (p: string) => string, basename: (p: string) => string, resolve: (...parts: string[]) => string }} path
 * @property {{ fileExists: (p: string) => boolean, directoryExists: (p: string) => boolean, readFile: (p: string) => string, readDirectory: (p: string) => string[], realPath: (p: string) => string }} fs
 * @property {(compilerOptions: object) => import("@ts-morph/bootstrap").Project} createProject
 */

/**
 * `/` separated paths, the only ones of the in-memory file system.
 */
export const posixPath = {
    normalize(p) {
        const absolute = p.startsWith("/");
        const segments = [];

        for (const segment of p.split("/")) {
            if (segment === "" || segment === ".") {
                continue;
            }

            if (segment === "..") {
                if (segments.length > 0 && segments[segments.length - 1] !== "..") {
                    segments.pop();
                } else if (!absolute) {
                    segments.push("..");
                }

                continue;
            }

            segments.push(segment);
        }

        const joined = segments.join("/");

        return absolute ? "/" + joined : joined === "" ? "." : joined;
    },

    join(...parts) {
        return posixPath.normalize(parts.filter((part) => part !== "").join("/"));
    },

    dirname(p) {
        const normalized = posixPath.normalize(p);
        const index = normalized.lastIndexOf("/");

        if (index === -1) {
            return ".";
        }

        return index === 0 ? "/" : normalized.slice(0, index);
    },

    basename(p) {
        const normalized = posixPath.normalize(p);

        return normalized.slice(normalized.lastIndexOf("/") + 1);
    },

    resolve(...parts) {
        let result = "/";

        for (const part of parts) {
            result = part.startsWith("/") ? part : result + "/" + part;
        }

        return posixPath.normalize(result);
    },
};

/**
 * A host over a ts-morph file system, the in-memory one in the browser.
 *
 * @param {import("@ts-morph/bootstrap").FileSystemHost} fileSystem
 * @param {string} cwd
 * @returns {Host}
 */
export function createFileSystemHost(fileSystem, cwd = "/") {
    return {
        cwd,
        fileSystem,
        path: posixPath,
        fs: {
            fileExists: (p) => fileSystem.fileExistsSync(p),
            directoryExists: (p) => fileSystem.directoryExistsSync(p),
            readFile: (p) => fileSystem.readFileSync(p, "utf8"),
            readDirectory: (p) => fileSystem.readDirSync(p).map((entry) => entry.name),
            realPath: (p) => {
                try {
                    return fileSystem.realpathSync(p);
                } catch {
                    return p;
                }
            },
        },
        createProject: (compilerOptions) =>
            createProjectSync({ fileSystem, compilerOptions, skipAddingFilesFromTsConfig: true }),
    };
}

/**
 * @param {string} cwd
 * @returns {Host}
 */
export function createInMemoryHost(cwd = "/") {
    return createFileSystemHost(new InMemoryFileSystemHost(), cwd);
}

/**
 * The host of the CLI, over the disk.
 *
 * @param {typeof import("node:fs")} fs
 * @param {typeof import("node:path")} path
 * @param {string} cwd
 * @returns {Host}
 */
export function createNodeHost(fs, path, cwd) {
    return {
        cwd,
        path: {
            join: (...parts) => path.join(...parts),
            dirname: (p) => path.dirname(p),
            basename: (p) => path.basename(p),
            resolve: (...parts) => path.resolve(...parts),
        },
        fs: {
            fileExists: (p) => fs.existsSync(p) && fs.statSync(p).isFile(),
            directoryExists: (p) => fs.existsSync(p) && fs.statSync(p).isDirectory(),
            readFile: (p) => fs.readFileSync(p, "utf8"),
            readDirectory: (p) => fs.readdirSync(p),
            realPath: (p) => {
                try {
                    return fs.realpathSync(p);
                } catch {
                    return p;
                }
            },
        },
        createProject: (compilerOptions) => createProjectSync({ compilerOptions, skipAddingFilesFromTsConfig: true }),
    };
}
