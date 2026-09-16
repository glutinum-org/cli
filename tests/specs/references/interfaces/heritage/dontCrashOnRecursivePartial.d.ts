// `Cyclic` is invalid TypeScript (ts2310), we only check that it doesn't crash
export interface Cyclic extends Partial<Cyclic> {
    self?: string;
}

export declare function get(): Cyclic;
