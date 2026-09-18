declare class Internal {
    constructor(value: number);
    value: number;
}

export declare class Public {
    constructor();
    inner: Internal;
}

export declare function make(): Internal;
