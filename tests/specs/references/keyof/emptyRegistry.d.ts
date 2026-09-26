export interface Mutators<S, A> {}

export type MutatorIdentifier = keyof Mutators<unknown, unknown>

export declare function create<M extends MutatorIdentifier>(id: M): void;
