export type Reader<T> = {
    tag: T;
};

export type Slot<T> = Reader<T> | "doc";
