export interface Options {
    level?: number;
}

export interface Instance {
    level: number;
}

export const Ctor: new (options?: Options) => Instance;
