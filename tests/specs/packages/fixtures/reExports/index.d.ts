import { Kind, Node } from "./compiler";

export { Options, createLogger, LogLevel, Logger } from "./logger";
export { Formatter as OutputFormatter } from "./formatter";
export * from "./helpers";
export * from "dep-lib";
export { Kind, Node };

export interface Sink {
    write(entry: string): void;
}
