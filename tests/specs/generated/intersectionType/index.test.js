
//------------------------------------------------------------------------------
//        This code was generated by `./build.sh test specs`
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------

import { expect, test } from 'vitest'
import { generateBindingFile } from '../../../..//src/Glutinum.Converter/Generate.fs.js'
import { dirname } from "dirname-filename-esm";
import path from 'node:path';

const __dirname = dirname(import.meta)

test('intersectionType/simple', () => {
    const filePath = path.join(__dirname, '../..//references/intersectionType/simple.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
    