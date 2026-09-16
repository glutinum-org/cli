export { Options, createLogger, LogLevel, Logger } from "./logger";
export { Formatter as OutputFormatter } from "./formatter";
export * from "./helpers";
export * from "dep-lib";

export interface Sink {
    write(entry: string): void;
}
