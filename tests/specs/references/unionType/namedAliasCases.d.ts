export interface Buffer {
    length: number;
}

export type PathLike = string | Buffer | URL;

export type Value = string | number | boolean;

export type Items = readonly string[] | readonly number[];

export type Shape = { kind: string } | { size: number };

export type Tag = string | Buffer;

export type Maybe = string | Buffer | undefined;

export declare function open(path: PathLike, value?: Value): void;
