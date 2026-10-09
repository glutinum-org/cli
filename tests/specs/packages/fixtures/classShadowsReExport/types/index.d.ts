import { GrammarState as GrammarState$1 } from "./types.js";
export * from "./types.js";
export declare class GrammarState implements GrammarState$1 {
    get theme(): string;
    get lang(): string;
}
export declare function getLastGrammarState(code: string): GrammarState$1;
