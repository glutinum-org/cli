declare module "fs" {
    export function readFileSync(path: string, encoding: BufferEncoding): string;
    export function readFileSync(path: string): Buffer;
    export function cwd(): NodeJS.Process;
}
