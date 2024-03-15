
//------------------------------------------------------------------------------
//        This code was generated by `./build.sh test specs`
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------

import { expect, test } from 'vitest'
import { generateBindingFile } from '../../../..//src/Glutinum.Converter/Generate.fs.js'
import { dirname } from "dirname-filename-esm";
import path from 'node:path';

const __dirname = dirname(import.meta)

test('interfaces/readonlyProperty', () => {
    const filePath = path.join(__dirname, '../..//references/interfaces/readonlyProperty.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('interfaces/nullUnionIsTranslatedAsOption', () => {
    const filePath = path.join(__dirname, '../..//references/interfaces/nullUnionIsTranslatedAsOption.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('interfaces/quotedProperty', () => {
    const filePath = path.join(__dirname, '../..//references/interfaces/quotedProperty.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('interfaces/readAndWriteProperty', () => {
    const filePath = path.join(__dirname, '../..//references/interfaces/readAndWriteProperty.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('interfaces/undefinedUnionIsTranslatedAsOption', () => {
    const filePath = path.join(__dirname, '../..//references/interfaces/undefinedUnionIsTranslatedAsOption.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('interfaces/method', () => {
    const filePath = path.join(__dirname, '../..//references/interfaces/method.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('interfaces/methodWithArguments', () => {
    const filePath = path.join(__dirname, '../..//references/interfaces/methodWithArguments.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('interfaces/callSignature', () => {
    const filePath = path.join(__dirname, '../..//references/interfaces/callSignature.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
    