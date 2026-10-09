export declare function instantiate(bytes: BufferSource): Promise<WebAssembly.Instance>;
export interface Loaded {
    exports: WebAssembly.Exports;
}
