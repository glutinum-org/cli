export interface Options {
    step?: number;
}

export declare function withDefault<O extends Options | undefined = undefined>(options?: O): void;
export declare function withoutDefault<O extends Options | undefined>(options?: O): void;
export declare function required<O extends Options | undefined>(options: O): void;
