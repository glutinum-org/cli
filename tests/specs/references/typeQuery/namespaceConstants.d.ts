export declare namespace constants {
    const RTLD_LAZY: number;
    const RTLD_NOW: number;
}

export interface Dl {
    flags: typeof constants;
}

export type Flags = typeof constants;
