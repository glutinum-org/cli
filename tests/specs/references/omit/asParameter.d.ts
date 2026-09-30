export interface Options {
    a: string;
    b?: number;
    c: boolean;
}

export declare function run(options: Omit<Options, "c">): void;

export declare function runGeneric<T extends Options>(options: Omit<T, "c">): void;

export interface Labeled<T> {
    value: T;
    label: string;
    extra: boolean;
}

export type Picked<T> = Pick<Labeled<T>, "value" | "label">;

export declare function runAlias<T>(options: Picked<T>): void;
