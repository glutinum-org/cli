import { Formatter } from "./formatter";

export interface Options {
    level?: LogLevel;
    formatter?: Formatter;
}

export enum LogLevel {
    Debug = 0,
    Info = 1
}

export declare class Logger {
    constructor(options?: Options);
    log(message: string): void;
}

export declare function createLogger(options?: Options): Logger;
