/** Options controlling the stagger. */
export type StaggerParams = {
    /** Where the stagger starts from. */
    from?: number | "first" | "last";
    reversed?: boolean;
    total: number;
};

export type ReturnedParams = {
    a?: string;
    b?: number;
};

export declare function stagger(value: number, params?: StaggerParams): void;

export declare function describe(): ReturnedParams;
