export interface Options<T> {
    markerClass?: new () => T;
}

export interface Literal<T> {
    factory: { new (): T };
}
