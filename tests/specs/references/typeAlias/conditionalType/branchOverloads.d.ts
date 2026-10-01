export interface ElementHandle<T> {
    element: T;
}

export interface JSHandle<T> {
    value: T;
}

export type Unboxed<Arg> =
    Arg extends ElementHandle<infer T> ? T :
    Arg extends JSHandle<infer T> ? T :
    Arg extends Array<infer T> ? Array<Unboxed<T>> :
    Arg;

export type NonNull<T> = T extends null | undefined ? never : T;

export interface Page {
    evaluate<R, Arg>(pageFunction: (arg: Unboxed<Arg>) => R, arg: Arg): Promise<R>;
    unbox<Arg>(arg: Arg): Unboxed<Arg>;
    nonNull<T>(value: T): NonNull<T>;
}

export declare function unbox<Arg>(arg: Arg): Unboxed<Arg>;
