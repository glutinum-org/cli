type MyIterable = {
    [Symbol.iterator](): IterableIterator<number>;
};
