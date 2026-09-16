export interface Options {
    debug?: boolean;
}

export declare const minimatch: {
    (p: string, pattern: string, options?: Options): boolean;
    defaults: (def: Options) => typeof minimatch;
};
