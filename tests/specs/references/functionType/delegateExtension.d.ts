export interface MyObject {
    random: (min: number, max: number) => number;
    // an optional callback or a rest parameter has no method form, no extension is generated
    onDone?: (code: number) => void;
    log: (...parts: string[]) => void;
}

export interface Child extends MyObject {
    name: string;
}
