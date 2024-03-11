
import { expect, test } from 'vitest'
import { generateBindingFile } from '../src/Glutinum.Converter/Generate.fs.js'
// import { sum } from './sum'
import { dirname } from "dirname-filename-esm";
import path from 'node:path';

const __dirname = dirname(import.meta)


test('specs/replacements/date.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/replacements/date.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/partial/simple.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/partial/simple.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/interface.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/interface.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exclude/literalIntEnum.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exclude/literalIntEnum.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exclude/literalStringEnum.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exclude/literalStringEnum.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalStringEnumWithInheritanceAndParenthesized.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalStringEnumWithInheritanceAndParenthesized.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalStringEnumStartingWithDigit.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalStringEnumStartingWithDigit.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalStringEnumWithInheritanceWorksWithDupicates.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalStringEnumWithInheritanceWorksWithDupicates.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalStringEnumWithInheritance.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalStringEnumWithInheritance.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalNumericEnumWithInheritanceAndParenthesized.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalNumericEnumWithInheritanceAndParenthesized.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/namedIntEnum.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/namedIntEnum.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalStringEnum.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalStringEnum.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalNumericEnum.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalNumericEnum.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalNumericEnumWithInheritance.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalNumericEnumWithInheritance.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalNumericEnumWithInheritanceWorksWithDuplicates.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalNumericEnumWithInheritanceWorksWithDuplicates.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/namedIntEnumWithInitialRank.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/namedIntEnumWithInitialRank.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalNumericEnumWithNestedEnums.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalNumericEnumWithNestedEnums.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/namedStringEnum.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/namedStringEnum.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/enums/literalStringEnumWithDash.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/enums/literalStringEnumWithDash.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/functionDeclaration/simple.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/functionDeclaration/simple.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/functionDeclaration/argumentWithGenerics.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/functionDeclaration/argumentWithGenerics.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/functionDeclaration/typeReferenceWithGenerics.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/functionDeclaration/typeReferenceWithGenerics.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/namespace/notExported.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/namespace/notExported.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typePredicate/convertedToBool.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typePredicate/convertedToBool.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exports/functionWithNoArgument.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exports/functionWithNoArgument.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exports/functionWithTypeReferenceInNamespace.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exports/functionWithTypeReferenceInNamespace.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exports/severalFunctions.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exports/severalFunctions.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exports/classInNamespace.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exports/classInNamespace.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exports/variable.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exports/variable.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exports/class.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exports/class.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exports/functionWithOptionalArgument.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exports/functionWithOptionalArgument.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exports/functionWithArgument.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exports/functionWithArgument.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exports/functionWithSeveralArguments.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exports/functionWithSeveralArguments.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/functionType/simple.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/functionType/simple.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/functionType/oneGeneric.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/functionType/oneGeneric.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/naming/Uint8Array.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/naming/Uint8Array.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/naming/promise.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/naming/promise.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/class/members.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/class/members.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/class/memberReturnThis.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/class/memberReturnThis.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/class/membersWithUnkownTypeReference.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/class/membersWithUnkownTypeReference.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/class/simpleConstructors.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/class/simpleConstructors.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/keyof/simpleObject.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/keyof/simpleObject.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/keyof/quotedProperty.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/keyof/quotedProperty.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/keyof/method.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/keyof/method.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/indexedAccessType/interfaceWithSingleMethod.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/indexedAccessType/interfaceWithSingleMethod.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/indexedAccessType/interfaceWithSeveralMethods.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/indexedAccessType/interfaceWithSeveralMethods.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/readonlyProperty.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/readonlyProperty.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/nullUnionIsTranslatedAsOption.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/nullUnionIsTranslatedAsOption.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/quotedProperty.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/quotedProperty.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/readAndWriteProperty.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/readAndWriteProperty.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/undefinedUnionIsTranslatedAsOption.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/undefinedUnionIsTranslatedAsOption.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/method.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/method.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/methodWithArguments.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/methodWithArguments.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/callSignature.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/callSignature.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/exportAssignment/functionReference.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/exportAssignment/functionReference.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeQuery/class.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeQuery/class.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeQuery/defaultToObj.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeQuery/defaultToObj.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/unions/primitives.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/unions/primitives.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/unions/primiteAndTypeReference.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/unions/primiteAndTypeReference.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/unions/primitiveAndTypeReferenceToAnInterface.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/unions/primitiveAndTypeReferenceToAnInterface.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/literal/boolean.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/literal/boolean.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/literal/string.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/literal/string.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/literal/float.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/literal/float.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/literal/int.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/literal/int.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/primitives/boolean.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/primitives/boolean.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/primitives/array.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/primitives/array.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/primitives/number.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/primitives/number.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/typeAlias/primitives/string.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/typeAlias/primitives/string.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/indexSignature/stringParameter.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/indexSignature/stringParameter.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            

test('specs/interfaces/indexSignature/numberParameter.d.ts', () => {
    const filePath = path.join(__dirname, 'specs/interfaces/indexSignature/numberParameter.d.ts');
    let result = generateBindingFile(filePath);
    result += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = filePath.split('.').slice(0, -2).join('.') + '.fsx'

    expect(result).toMatchFileSnapshot(expectedFile)
})
            
    