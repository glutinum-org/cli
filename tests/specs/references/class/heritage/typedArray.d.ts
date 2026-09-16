export declare class Buffer<TArrayBuffer extends ArrayBufferLike = ArrayBufferLike> extends Uint8Array<TArrayBuffer> {
    write(string: string): number;
    subarray(start?: number, end?: number): Buffer<TArrayBuffer>;
}

export interface Callable extends Function {
    name: string;
}
