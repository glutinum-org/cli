type IfDefaultsTrue<T, IfTrue, IfFalse> = T extends true ? IfTrue : T extends false ? IfFalse : IfTrue;

export interface Config {
    strict?: boolean;
}

export interface Token {
    kind: string;
}

// `Config extends T` holds for the constraint of `T`
type Results<T extends Config> = Config extends T ? { tokens: Token[] } : never;

export declare function parse<T extends Config>(config?: T): Results<T>;

// The checker resolves a conditional applied to concrete arguments
export declare const strict: IfDefaultsTrue<true, string, number>;
export declare const loose: IfDefaultsTrue<false, string, number>;

export interface Events {
    data: string;
    end: number;
}

// `keyof T` and `T[K]` with `T` known
type ValueOf<K, T> = K extends keyof T ? T[K] : never;

export interface Emitter<T = Events> {
    value(): ValueOf<"data", T>;
    missing(): ValueOf<"other", T>;
}

// `[T] extends [X]` compares the types
type Handle<T = Token> = [T] extends [Token] ? Token : string;

export interface Page {
    handle(): Handle;
}
