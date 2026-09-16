export interface Options {
    a: string;
    b: number;
}

export type Same = {
    [K in keyof Options]: Options[K];
};

export type Strings = {
    [K in keyof Options]: string;
};
