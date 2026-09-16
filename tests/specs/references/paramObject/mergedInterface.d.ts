export interface SourceFileLike {
    readonly text: string;
}

export interface SourceFileLike {
    getLineAndCharacterOfPosition(pos: number): number;
}

export declare function getLine(sourceFile: SourceFileLike): number;
