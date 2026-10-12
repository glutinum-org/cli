export interface Reader<T> {
    tag: T;
}

export declare function read<T>(reader: Reader<T>): T;
