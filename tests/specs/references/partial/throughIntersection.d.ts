export interface Options<T> {
    value: T;
    label: string;
}

export interface Base<T> {
    opts: Partial<Options<T>>;
    rest: Omit<Options<T>, "label">;
}

export type Config = Base<string> & { extra: number };
