export type Extension = {
    extension: Extension;
} | readonly Extension[];

export type Names = { name: string } | readonly string[];

export declare function names(value: readonly string[]): void;
