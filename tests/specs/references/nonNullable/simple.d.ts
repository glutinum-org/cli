export interface Node {
    kind: number;
}

export function unwrap(node: NonNullable<Node | undefined>): Node;
export function unwrapNull(node: NonNullable<Node | null | undefined>): Node;
export function unwrapParameter<T>(node: NonNullable<T>): T;
