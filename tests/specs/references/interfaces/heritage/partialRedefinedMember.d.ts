export interface Base {
    value: string | number;
    other: number;
}

export interface Redefines extends Partial<Base> {
    value: string;
}

export declare function get(): Redefines;
