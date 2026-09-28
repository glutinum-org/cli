// TypeScript resolves `stat(path)` to the first declaration, F# needs the others to ask for
// an argument it does not take
export declare function stat(path: string, options?: { bigint: false }): string;
export declare function stat(path: string, options?: { bigint: true }): number;
export declare function stat(path: string, options?: { bigint: boolean }, extra?: string): boolean;
