export type Pair<C, S> = {
    client: C;
    server: S;
    swap(): Pair<S, C>;
};

export type StringNumber = Pair<string, number>;
