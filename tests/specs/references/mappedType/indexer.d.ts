export type CustomEvents = {
    [Key in string & {} | symbol]: (...args: any[]) => void;
};

export type PartialOptions<T> = {
    [K in keyof T]?: T[K] | undefined;
};
