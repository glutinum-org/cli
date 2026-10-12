export type Reader<T> = {
    tag: T;
};

export declare class Box<T> implements Reader<T> {
    get reader(): Reader<T>;
}

export declare function read<T>(reader: Reader<T>): T;
