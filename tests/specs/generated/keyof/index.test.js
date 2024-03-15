
//------------------------------------------------------------------------------
//        This code was generated by `./build.sh test specs`
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------

import { expect, test } from 'vitest'
import { generateBindingFile } from '../../../..//src/Glutinum.Converter/Generate.fs.js'
import { dirname } from "dirname-filename-esm";
import path from 'node:path';

const __dirname = dirname(import.meta)

test('keyof/simpleObject', () => {
    const filePath = path.join(__dirname, '../..//references/keyof/simpleObject.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})

test('keyof/quotedProperty', () => {
    const filePath = path.join(__dirname, '../..//references/keyof/quotedProperty.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})

test('keyof/method', () => {
    const filePath = path.join(__dirname, '../..//references/keyof/method.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})