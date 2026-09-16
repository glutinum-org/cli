export interface Element {
    tagName: string;
}

export interface ParentNode {
    querySelector<E extends Element = Element>(selectors: string): E | null;
    querySelectorAll<E extends Element = Element>(selectors: string): E[];
    closest<K = string>(): K;
}

export declare class Query {
    find<E extends Element = Element>(selectors: string): E;
}
