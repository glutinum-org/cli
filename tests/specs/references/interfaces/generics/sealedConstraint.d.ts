interface TypeMap {
    "string": string;
    "number": number;
}

export interface Picker<K extends keyof TypeMap> {
    key: K;
}

export interface Holder {
    picker: Picker<"string">;
}
