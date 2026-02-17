declare class Log<T extends string, R1 extends number, R2> {
    info(data: [T, number]) : R1 | R2;
}
