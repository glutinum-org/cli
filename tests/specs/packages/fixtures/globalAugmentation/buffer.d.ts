type ImplicitBlob = string;
declare module "buffer" {
    type ImplicitArrayBuffer = ArrayBuffer;
    global {
        type BufferEncoding = "utf8" | "hex";

        interface BufferConstructor {
            from(data: string, encoding?: BufferEncoding): Buffer;
        }

        interface Buffer extends Uint8Array {
            toString(encoding?: BufferEncoding): string;
        }

        var Buffer: BufferConstructor;

        namespace NodeJS {
            interface Process {
                platform: string;
            }
        }
    }
    export function isUtf8(input: Buffer): boolean;
}
declare module "node:buffer" {
    export * from "buffer";
}
