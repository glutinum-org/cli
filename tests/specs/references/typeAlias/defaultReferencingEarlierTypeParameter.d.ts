export interface Node {
    kind: number;
}

export type Visitor<TIn extends Node = Node, TOut extends Node | undefined = TIn | undefined> = (node: TIn) => TOut;
