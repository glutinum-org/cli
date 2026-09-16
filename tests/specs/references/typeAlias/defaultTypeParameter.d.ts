export interface Options {
    debug?: boolean;
}

export interface Scale<O> {
    options: O;
}

export type CategoryScale<O extends Options = Options> = Scale<O>;

export type Pair<A, B = string> = { first: A; second: B };

export declare const CategoryScale: {
    prototype: CategoryScale;
};
