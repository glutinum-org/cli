//------------------------------------------------------------------------------
//
//      This file is used to manually test the transformation of a .d.ts file to a .fsx file.
//      Running test in debug mode with Vitest is not stable enough right now.
//      So by using this script manually, we avoid avoid to re-start VSCode
//      too often because of a wrong state of the Vitest extension.
//
//------------------------------------------------------------------------------

import { generateBindingFile } from '../../src/Glutinum.Converter/Generate.fs.js';
import path from 'node:path';
import * as Diff from 'diff';
import chalk from 'chalk';
import fs from 'fs';

const log = console.log;

if (process.argv.length < 3) {
    log(chalk.red('Please provide the path to the definition file'));
    log(chalk.red('Example: node --enable-source-maps index.js tests/specs/references/exports/variable.d.ts'));
    process.exit(1);
}

const definitionFile = path.join(process.cwd(), process.argv[2]);

log(chalk.grey(`Reference: file://${definitionFile}`));

try {
    let actual = generateBindingFile(definitionFile);
    actual += `
(***)
#r "nuget: Fable.Core"
(***)
`;

    const expectedFile = definitionFile.split('.').slice(0, -2).join('.') + '.fsx'
    const expectedFileExists = fs.existsSync(expectedFile);
    const expectedContent = expectedFileExists ? fs.readFileSync(expectedFile, 'utf8') : '';

    log(chalk.grey(`Expected: file://${expectedFile}`));

    console.log();
    console.log('Generated content:\n');

    const diff = Diff.diffChars(expectedContent, actual);

    diff.forEach((part) => {
        // green for additions, red for deletions
        let text = part.added ? chalk.bgGreenBright(part.value) :
            part.removed ? chalk.bgRedBright(part.value) :
                part.value;
        process.stderr.write(text);
    });

    console.log();

    if (actual !== expectedContent) {
        console.error(chalk.red('Actual content does not match expected content'));
        process.exit(1);
    } else {
        console.log(chalk.green('Success'));
    }
} catch (error) {
    console.error(chalk.red(error.message));
    process.exit(1);
}
