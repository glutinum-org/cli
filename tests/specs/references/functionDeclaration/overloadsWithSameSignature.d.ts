export interface Options {
    silent?: boolean;
}

export declare function getSession(options: Options & { createIfNone: true }): string;
export declare function getSession(options: Options & { forceNewSession: true }): string;
export declare function getSession(options?: Options): string | undefined;
