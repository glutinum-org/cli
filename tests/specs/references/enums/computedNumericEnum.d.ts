export enum FileAccess {
    None,
    Read = 1 << 1,
    Write = 1 << 2,
    ReadWrite = Read | Write,
}
