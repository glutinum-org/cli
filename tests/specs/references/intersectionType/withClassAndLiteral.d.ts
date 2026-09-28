export declare class Stream {
    write(chunk: string): boolean;
    write(chunk: string, encoding: string): boolean;
}

export interface Holder {
    // The overloads are kept by inheriting the class, the literal declares its own members
    out: Stream & { fd: 1 };
}
