export type DateArg = Date | number | string;

export interface Options {
    value: string;
}

export declare function format(date: DateArg & {}, formatStr: string): string;

export declare function run<T>(value: T & {}, options: {} & Options): void;
