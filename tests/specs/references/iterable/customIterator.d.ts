export type MyIterable = {
    [Symbol.iterator](): {
        next(): {
            done: boolean;
            value: number;
        };
        return(): {
            done: boolean;
        };
    };
};
