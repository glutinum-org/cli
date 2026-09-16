import { ExtglobType } from './ast';

export declare class AST {
    type: ExtglobType | null;
}

export type Local = 'a' | 'b';

export type WithLocal = Local | 'c';
