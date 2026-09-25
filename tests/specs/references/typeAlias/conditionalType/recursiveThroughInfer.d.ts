export interface Box<T> {
    value: T;
}

export type DeepPartial<T> = T extends Box<infer Inner> ? Box<DeepPartial<Inner>> : T;

export declare function deepPartial<T>(schema: T): DeepPartial<T>;
