export interface Value {
    raw: string;
}

type Keys = 'Accept' | 'Location';

export type Headers = Partial<{ [Key in Keys]: string }>;

export type WithExtra = Headers & { extra: string };

export type Required = { [Key in Keys]: Value } & { other: number };
