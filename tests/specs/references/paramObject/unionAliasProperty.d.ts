export type Value = { name: string } | readonly string[];

export interface Inline {
    inline?: { name: string } | readonly string[];
}

export interface Aliased {
    alias?: Value;
}

export declare function configure(value?: Value): void;
