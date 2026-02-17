declare interface Thenable<T> {}

declare class Log3<T extends string, R1 extends number, R2> {
    info(data: Thenable<T> | number) : R1 | R2;
}
