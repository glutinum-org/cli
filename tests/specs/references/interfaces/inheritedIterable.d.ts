export interface Node {
    nodeName: string;
}

export interface NodeList {
    length: number;
    [Symbol.iterator](): IterableIterator<Node>;
}

export interface NodeListOf<TNode extends Node> extends NodeList {
    item(index: number): TNode;
    [Symbol.iterator](): IterableIterator<TNode>;
}

export type Logger = (message: string) => void;

export interface DebugLogger extends Logger {
    enabled: boolean;
}
