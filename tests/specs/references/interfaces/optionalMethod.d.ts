export interface Compare {
    // An F# interface has no optional method, both are the same member
    asMethod?(x: string): void;
    asProperty?: (x: string) => void;
}

export interface InParamObject {
    options: { a: string; maybe?(x: string): void };
}
