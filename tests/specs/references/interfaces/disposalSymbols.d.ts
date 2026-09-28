export interface FileHandle {
    read(): Promise<string>;
    [Symbol.dispose](): void;
    [Symbol.asyncDispose](): Promise<void>;
}

export interface Bag {
    [Symbol.iterator](): Iterator<string>;
    [Symbol.toStringTag]: string;
    size: number;
}
