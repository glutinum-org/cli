declare function RAL(): RAL;

declare namespace RAL {
    interface TextEncoder {
        encode(value: string): Uint8Array;
    }
}

export default RAL;
