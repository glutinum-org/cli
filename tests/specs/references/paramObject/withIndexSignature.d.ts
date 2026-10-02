export interface SendOptions {
    [key: string]: any;
    fields?: string;
}

export interface RecordOptions extends SendOptions {
    expand?: string;
}

export interface Bag {
    [key: string]: any;
}

export declare function send(options: SendOptions): void;
export declare function get(options: RecordOptions): void;
export declare function fill(bag: Bag): void;

export interface Sized<T> {
    readonly length: number;
    readonly [n: number]: T;
}

export declare function measure(sized: Sized<string>): void;
