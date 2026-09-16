import type { WriteStream } from "node:tty";

export interface Logger {
    stream: WriteStream;
}
