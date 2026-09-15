interface TypeMap {
    "string": string;
    "number": number;
    "bool": boolean;
}

type MyType<T extends keyof TypeMap> = TypeMap[T];

export declare const value: MyType<"string">;

export declare function create<T extends keyof TypeMap>(tagName: T): MyType<T>;
