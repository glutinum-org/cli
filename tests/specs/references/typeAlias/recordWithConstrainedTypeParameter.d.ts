type Params<P extends string = any> = Record<P, string | string[]>;

export interface EventContext<P extends string> {
    params: Params<P>;
}
