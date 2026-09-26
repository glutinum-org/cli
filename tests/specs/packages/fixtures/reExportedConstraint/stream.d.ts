export type Key = readonly unknown[]

export interface Pipeline<TKey extends readonly unknown[]> {
    key: TKey
}

export interface Sink<TName extends string | number> {
    name: TName
}

export interface Store<TKey extends Key> {
    key: TKey
}
