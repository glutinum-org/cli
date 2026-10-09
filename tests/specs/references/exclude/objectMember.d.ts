export interface Reader {
    read<T extends Exclude<BufferSource, ArrayBuffer>>(view: T): T;
}
