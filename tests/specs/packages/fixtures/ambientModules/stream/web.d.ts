type _Helper = string;
declare module "stream/web" {
    interface ReadableStream {
        locked: boolean;
    }
}
declare module "node:stream/web" {
    export * from "stream/web";
}
