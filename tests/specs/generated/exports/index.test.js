
//------------------------------------------------------------------------------
//        This code was generated by `./build.sh test specs`
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------

import { expect, test } from 'vitest'
import { generateBindingFile } from '../../../..//src/Glutinum.Converter/Generate.fs.js'
import { dirname } from "dirname-filename-esm";
import path from 'node:path';

const __dirname = dirname(import.meta)

test('exports/functionWithNoArgument', () => {
    const filePath = path.join(__dirname, '../..//references/exports/functionWithNoArgument.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('exports/functionWithTypeReferenceInNamespace', () => {
    const filePath = path.join(__dirname, '../..//references/exports/functionWithTypeReferenceInNamespace.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('exports/severalFunctions', () => {
    const filePath = path.join(__dirname, '../..//references/exports/severalFunctions.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('exports/classInNamespace', () => {
    const filePath = path.join(__dirname, '../..//references/exports/classInNamespace.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('exports/variable', () => {
    const filePath = path.join(__dirname, '../..//references/exports/variable.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('exports/class', () => {
    const filePath = path.join(__dirname, '../..//references/exports/class.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('exports/functionWithOptionalArgument', () => {
    const filePath = path.join(__dirname, '../..//references/exports/functionWithOptionalArgument.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('exports/functionWithArgument', () => {
    const filePath = path.join(__dirname, '../..//references/exports/functionWithArgument.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
test('exports/functionWithSeveralArguments', () => {
    const filePath = path.join(__dirname, '../..//references/exports/functionWithSeveralArguments.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
        
    