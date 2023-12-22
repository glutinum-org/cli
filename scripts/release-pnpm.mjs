import { release } from "./release-core.mjs"
import { simpleSpawn } from "./await-spawn.mjs"

const cwd = process.cwd();
const baseDirectory = process.argv[2] ? path.resolve(cwd, process.argv[2]) : cwd;

await release({
    baseDirectory: baseDirectory,
    projectFileName: "package.json",
    versionRegex: /(^\s*"version":\s*")(.+)(",\s*$)/gmi,
    publishFn: async () => {
        try {
            await simpleSpawn("pnpm publish --access public", baseDirectory)
        } catch (err) {
            throw "Npm publish failed"
        }
    }
})
