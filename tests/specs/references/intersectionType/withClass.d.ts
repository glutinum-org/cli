export declare class Stream {
    write(chunk: string): boolean;
}

export interface WithInterface {
    other: string;
}

export interface Holder {
    // A class can't be inherited by an F# interface, its members are flattened instead
    classAndLiteral: Stream & { fd: 1 };
    // Every constituent is inheritable, the overloads are kept
    interfaceAndInterface: WithInterface & { fd: 1 };
}
