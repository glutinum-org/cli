import { expect, test } from 'vitest'
import { generateBindingFile } from "../src/Glutinum.Converter/Generate.fs.js";
import fs from 'fs'
import dirname from 'dirname-filename-esm'

const __dirname = dirname(import.meta);

fs.readdirSync(`{__SOURCE_DIRECTORY__}/specs`, { recursive: true })
    .filter(f => f.endsWith(".d.ts"))
    .filter(f => f.indexOf("disabled.") > -1)
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
`})})

//     test('dynamic2', () => {
//         const path = "/Users/mmangel/Workspaces/Github/glutinum-org/Glutinum.Converter/tests/specs/indexedAccessType/interfaceWithSingleMethod.d.ts"
//         const snapshotFile = "/Users/mmangel/Workspaces/Github/glutinum-org/Glutinum.Converter/tests/specs/indexedAccessType/interfaceWithSingleMethod.fsx"
//         const res = generateBindingFile(path);
//         let finalRes = `${res}
// (***)
// #r "nuget: Fable.Core"
// (***)
// `

//         // console.log(res)
//         expect(finalRes).toMatchFileSnapshot(snapshotFile)
//     })


// // // with custom serializer
// // expect.addSnapshotSerializer({
// //     serialize(val, config, indentation, depth, refs, printer) {
// //         return `${printer(
// //             val,
// //             config,
// //             indentation,
// //             depth,
// //             refs,
// //         )}

// // (***)
// // #r "nuget: Fable.Core"
// // (***)
// //         `
// //     },
// //     test(val) {
// //         console.log(val)
// //         Object.prototype.hasOwnProperty.call(val, 'foo')
// //         return val //&& Object.prototype.hasOwnProperty.call(val, 'foo')
// //     },
// // })

// // test('basic', () => {
// //     // example from docs/guide/snapshot.md

// //     const bar = "dddwddw"


// //     expect(bar).toMatchFileSnapshot('tests/snapshots/basic.snap')
// // })
