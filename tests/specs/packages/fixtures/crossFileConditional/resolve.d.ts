export interface Schema {
    body: unknown;
}

export type ResolveBody<S extends Schema> = S extends { body: infer B } ? B : unknown;
