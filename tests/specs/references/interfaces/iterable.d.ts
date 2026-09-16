export interface NodeListOf<TNode> {
    length: number;
    item(index: number): TNode;
    [Symbol.iterator](): ArrayIterator<TNode>;
}

export interface Headers {
    get(name: string): string | null;
    [Symbol.iterator](): HeadersIterator<[string, string]>;
}
