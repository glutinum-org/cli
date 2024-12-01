export type MyIterable = {
    [Symbol.iterator](): {
        next(): {
            done: boolean;
            // Commented on purpose to make type detection fail
            // value: number;
        };
        return(): {
            done: boolean;
        };
    };
};
