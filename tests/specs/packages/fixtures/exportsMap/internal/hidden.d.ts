export interface Options {
    verbose: boolean;
}

export declare class Session {
    readonly id: string;
    close(): void;
}

export declare function open(options: Options): Session;

export declare const version: string;

export type mode = number;

export namespace mode {
    let fast: number;
    let slow: number;
}
