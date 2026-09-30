export type Nested = ReadonlyArray<Nested | string>;

export type Tree = (Tree | number)[];

export type Json = string | number | boolean | null | Json[] | { [key: string]: Json };
