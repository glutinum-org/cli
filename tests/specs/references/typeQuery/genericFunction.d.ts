export interface Node {
    kind: number;
}

export declare function visitNodes<TIn extends Node>(nodes: TIn[], visitor: (node: TIn) => Node): TIn[];

export declare function visitEachChild<T extends Node>(node: T, nodesVisitor?: typeof visitNodes): T;
