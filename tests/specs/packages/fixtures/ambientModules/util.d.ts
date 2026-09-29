declare module "util" {
    function format(format: string): string;
}
declare module "util/types" {
    function isDate(value: unknown): boolean;
}
declare module "node:util" {
    export * from "util";
}
declare module "node:util/types" {
    export * from "util/types";
}
