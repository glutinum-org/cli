export type Bounds = [number, number, ...number[]];

export type Mixed = [string, ...number[]];

export declare function first<T extends unknown[]>(args: [string, ...T]): void;
