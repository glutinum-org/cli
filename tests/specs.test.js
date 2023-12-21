import { expect, test } from 'vitest'
// import { generateBindingFile } from "../src/Glutinum.Converter/Generate.fs.js";
import { generateBindingFile } from "../fableBuild/Generate.js"
import fs from 'fs'
import { dirname } from 'dirname-filename-esm'

const __dirname = dirname(import.meta);

fs.readdirSync(`${__dirname}/specs`, { recursive: true })
    .filter(f => f.endsWith(".d.ts"))
    .filter(f => f.indexOf("disabled.") === -1)
    .map(f => f.replace(".d.ts", ""))
    .forEach(f => {
        test(f, () => {
            const path = `${__dirname}/specs/${f}.d.ts`
            const snapshotFile = `${__dirname}/specs/${f}.fsx`
            const res = generateBindingFile(path);
            let finalRes = `${res}
(***)
#r "nuget: Fable.Core"
(***)
`

            expect(finalRes).toMatchFileSnapshot(snapshotFile)
        })
    })
